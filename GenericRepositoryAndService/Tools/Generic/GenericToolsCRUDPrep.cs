using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace GenericRepositoryAndService.Tools.Generic
{
    public abstract class GenericToolsCRUDPrep
    {
        /// <summary>
        /// Get an instance of the service of type <paramref name="t"/> using <paramref name="context"/>.
        /// </summary>
        /// <remarks>
        /// There are some restrictions on the names of the classes, the repositories and the services : 
        /// <list type="bullet">
        /// <item>
        /// For a class t with name "TName", the corresponding repository must be named "TNameRepository"
        /// </item>
        /// <item>
        /// For a class t with name "TName", the corresponding service must be named "TNameService"
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="context">The context</param>
        /// <param name="t">The type for which the service has to be instanciated</param>
        /// <returns>A new instance of the service of <paramref name="t"/></returns>
        private static dynamic GetServiceFromContext(DbContext context, Type t)
        {
            Type typetRepository = Assembly.GetAssembly(t)
                                           .GetTypes()
                                           .Single(typ => 
                                                   typ.Name == t.Name + "Repository"
                                                   );
            Type typetService = Assembly.GetAssembly(t)
                                        .GetTypes()
                                        .Single(typ => 
                                                typ.Name == t.Name + "Service"
                                                );
            dynamic tRepository = Activator.CreateInstance(typetRepository, context);
            dynamic tService = Activator.CreateInstance(typetService, tRepository);
            return tService;
        }

        /// <summary>
        /// For the type <paramref name="q"/>, using a new context <paramref name="context"/>, update the elements in case
        /// the element of type <typeparamref name="T"/> having either Id or keys <paramref name="objs"/> has to be deleted. That is
        /// to say, for elements of type <paramref name="q"/> such that their property <paramref name="propname"/> of type <typeparamref name="T"/>
        /// is not <see langword="null"/>,
        /// <list type="bullet">
        /// <item>
        /// If the property doesn't have an annotation <see cref="RequiredAttribute"/>, just set it to <see langword="null"/>. 
        /// (EF won't do it by itself using TRepository since <typeparamref name="T"/> has no property for that relationship)
        /// </item>
        /// <item>
        /// If the property has an annotation <see cref="RequiredAttribute"/>, delete the item (EF won't do it by itself for
        /// the same reason)
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>It is assumed that the property of <paramref name="q"/> with name <paramref name="propname"/> is of
        /// type <typeparamref name="T"/> (and NOT <see cref="IList{T}"/>) and that <typeparamref name="T"/> has no
        /// property representing that relationship. In other words it is assumed that <paramref name="q"/> is part
        /// of <see cref="GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty{T}"/></remarks>
        /// <typeparam name="T">The type of the element deleted with either Id or keys <paramref name="objs"/> for which actions 
        /// have to be taken</typeparam>
        /// <param name="context">The context in which to do this operation</param>
        /// <param name="q">The type of the elements to be updated before the deletion of the element of type <typeparamref name="T"/> with 
        /// either Id or keys <paramref name="objs"/></param>
        /// <param name="propname">The name of the property of <paramref name="q"/> having type <typeparamref name="T"/></param>
        /// <param name="objs">Either the Id or keys of the element of type <typeparamref name="T"/> deleted</param>
        private static void SetForTypePropertyWithGivenKeysToNullInNewContext<T>(DbContext context, Type q, string propname, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            MethodInfo methodListWherePropNotNull = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                    "ListWherePropNotNull", 
                                                                                                    BindingFlags.Public | BindingFlags.Static
                                                                                                 )
                                                                                       .MakeGenericMethod(new Type[] { 
                                                                                                                        q 
                                                                                                                     }
                                                                                                         );
            dynamic req = methodListWherePropNotNull.Invoke(
                                                            typeof(GenericToolsQueriesAndLists), 
                                                            new object[] { 
                                                                            qService.GetAllIncludes(1, int.MaxValue, null, null),
                                                                            propname 
                                                                         }
                                                           );

            MethodInfo methodQueryWherePropKeysAre = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                    "ListWherePropKeysAre", 
                                                                                                    BindingFlags.NonPublic | BindingFlags.Static
                                                                                                   )
                                                                                        .MakeGenericMethod(new Type[] { 
                                                                                                                        typeof(T), 
                                                                                                                        q 
                                                                                                                      }
                                                                                                          );
            req = methodQueryWherePropKeysAre.Invoke(
                                                        typeof(GenericToolsQueriesAndLists), 
                                                        new object[] { 
                                                                        req, 
                                                                        propname, 
                                                                        objs 
                                                                     }
                                                    );

            foreach (var qItem in req)
            {
                using (DbContext context2 = (DbContext)Activator.CreateInstance(context.GetType()))
                {
                    dynamic qService2 = GetServiceFromContext(context2, q);

                    MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                "GetKeysValues",
                                                                                                BindingFlags.Public | BindingFlags.Static
                                                                                                )
                                                                                     .MakeGenericMethod(new Type[] {
                                                                                                                    q
                                                                                                                   }
                                                                                                       );

                    if (q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) == null)
                    {
                        var qItem2 = qService2.FindByIdIncludes(
                                                                (object[])methodGetKeysValues.Invoke(
                                                                                                        typeof(GenericToolsTypeAnalysis),
                                                                                                        new object[] {
                                                                                                                        qItem
                                                                                                                     }
                                                                                                    )
                                                               );
                        qService2.UpdateOne(qItem2, propname, null);
                    }
                    else
                    {
                        var qItem2 = qService2.FindByIdExcludes(
                                                                    (object[])methodGetKeysValues.Invoke(
                                                                                                            typeof(GenericToolsTypeAnalysis),
                                                                                                            new object[] {
                                                                                                                            qItem
                                                                                                                         }
                                                                                                        )
                                                               );
                        qService2.Delete(qItem2);
                    }
                }
            }
        }

        /// <summary>
        /// Get a new array with elements :
        /// <list type="bullet">
        /// <item>
        /// the elements of <paramref name="objs"/>
        /// </item>
        /// <item>
        /// the array <paramref name="paramsobjects"/>
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>Used to a dynamic call to a generic function having arguments 
        /// (obj arg1, ... obj argn, params object[] paramsobject). 
        /// <br/>
        /// <paramref name="objs"/> will contain {arg1, ... , argn}
        /// <br/>
        /// The dynamic call will use <see cref="ConcatArrayWithParams"/> with arguments (<paramref name="objs"/>,<paramref name="paramsobjects"/>)</remarks>
        /// <param name="objs">The objects</param>
        /// <param name="paramsobjects">The array of objects used in params argument.</param>
        /// <returns>The array.</returns>
        private static object[] ConcatArrayWithParams(object[] objs, object[] paramsobjects)
        {
            object[] res = new object[objs.Length + 1];
            for (int i = 0; i < objs.Length; i++)
            {
                res[i] = objs[i];
            }
            res[objs.Length] = paramsobjects;
            return res;
        }

        /// <summary>
        /// In a given context <paramref name="context"/>, for the type <paramref name="q"/> with the property with name 
        /// <paramref name="propname"/> of type <typeparamref name="T"/>, prepare the deletion of a given element of type <typeparamref name="T"/>
        /// with either Id or keys <paramref name="objs"/>. 
        /// <br/>
        /// That is to say, for every element of type <paramref name="q"/> such that
        /// their property <paramref name="propname"/> is of type <see cref="IList{T}"/> and contains the element of either Id or keys
        /// <paramref name="objs"/>
        /// <list type="bullet">
        /// <item>
        /// if the property <paramref name="propname"/> of <paramref name="q"/> has the annotation <see cref="RequiredAttribute"/> and the
        /// list has only the item to delete remaining, remove the element of type <paramref name="q"/>
        /// </item>
        /// <item>
        /// otherwise just remove the element from the list.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type of the element to be deleted</typeparam>
        /// <param name="context">The context</param>
        /// <param name="q">The type of the elements to be updated</param>
        /// <param name="propname">The name of the property of q of type <see cref="IList{T}"/></param>
        /// <param name="objs">Either the Id or the keys</param>
        private static void RemoveForTypePropertyListElementWithGivenKeyInNewContext<T>(DbContext context, Type q, string propname, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            MethodInfo methodExpressionListWherePropListCountainsElementWithGivenKeys = typeof(GenericToolsExpressionTrees).GetMethod(
                                                                                                                                        "ExpressionListWherePropListCountainsElementWithGivenKeys", 
                                                                                                                                        BindingFlags.Public | BindingFlags.Static
                                                                                                                                     )
                                                                                                                           .MakeGenericMethod(new Type[] { 
                                                                                                                                                            typeof(T), 
                                                                                                                                                            q 
                                                                                                                                                          }
                                                                                                                                              );
            dynamic func = methodExpressionListWherePropListCountainsElementWithGivenKeys.Invoke(
                                                                                                    typeof(GenericToolsExpressionTrees), 
                                                                                                    ConcatArrayWithParams(
                                                                                                                            new object[] { 
                                                                                                                                            q.GetProperty(propname) 
                                                                                                                                         },
                                                                                                                            objs
                                                                                                                         )
                                                                                                );

            dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, func);

            foreach (var qItem in req)
            {
                using (DbContext context2 = (DbContext)Activator.CreateInstance(context.GetType()))
                {
                    dynamic qService2 = GetServiceFromContext(context2, q);

                    MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                "GetKeysValues", 
                                                                                                BindingFlags.Public | BindingFlags.Static
                                                                                               )
                                                                                     .MakeGenericMethod(new Type[] {    
                                                                                                                        q 
                                                                                                                    }
                                                                                                       );
                    var qItem2 = qService2.FindByIdIncludes(
                                                            (object[])methodGetKeysValues.Invoke(
                                                                                                    typeof(GenericToolsTypeAnalysis), 
                                                                                                    new object[] { 
                                                                                                                    qItem 
                                                                                                                 }
                                                                                                )
                                                           );

                    var oldValue = qItem2.GetType().GetProperty(propname).GetValue(qItem2);
                    if (q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) != null && ((oldValue as IList).Count == 1))
                    {
                        using (DbContext context3 = (DbContext)Activator.CreateInstance(context.GetType()))
                        {
                            dynamic qService3 = GetServiceFromContext(context3, q);
                            var qItem3 = qService3.FindByIdIncludes(
                                                                    (object[])methodGetKeysValues.Invoke(
                                                                                                            typeof(GenericToolsTypeAnalysis), 
                                                                                                            new object[] { 
                                                                                                                            qItem 
                                                                                                                         }
                                                                                                        )
                                                                   );
                            qService3.Delete(qItem3);
                        }
                    }
                    else
                    {
                        MethodInfo methodListRemoveElementWithGivenKeys = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                                            "ListRemoveElementWithGivenKeys", 
                                                                                                                            BindingFlags.Public | BindingFlags.Static
                                                                                                                       )
                                                                                                             .MakeGenericMethod(new Type[] { 
                                                                                                                                            typeof(T) 
                                                                                                                                           }
                                                                                                                               );
                        var newValue = methodListRemoveElementWithGivenKeys.Invoke(
                                                                                    typeof(GenericToolsQueriesAndLists), 
                                                                                    new object[] { 
                                                                                                    oldValue, 
                                                                                                    objs 
                                                                                                 }
                                                                                  );
                        qService2.UpdateOne(qItem2, propname, newValue);
                    }
                }
            }
        }

        /// <summary>
        /// An object of type <typeparamref name="T"/> with either Id or keys <paramref name="objs"/> is deleted.
        /// Every type <paramref name="q"/> in <see cref="GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty{T}"/> has to be updated
        /// manually. Indeed, <typeparamref name="T"/> has no property representing that relation, and thus no element of 
        /// type <paramref name="q"/> will be loaded in the context and changed, wich will result in exceptions if not
        /// taken care of. This is such treatment.
        /// <br/>
        /// If the property representing that relation in type <paramref name="q"/> is of type <typeparamref name="T"/>,
        /// this will set, for the appropriate elements of type <paramref name="q"/>, either the property to <see langword="null"/>,
        /// or if it has annotation <see cref="RequiredAttribute"/> it will delete it.
        /// <br/>
        /// if the property representing that relation in type <paramref name="q"/> is of type <see cref="IList{T}"/>,
        /// this will set, for the appropriate elements of type <paramref name="q"/>, the property to the list without the element
        /// with either Id or keys <paramref name="objs"/>. Furthermore, if such property has annotation <see cref="RequiredAttribute"/>,
        /// and the item of type <typeparamref name="T"/> to delete was the only remaining element of the list, it will delete
        /// the element of type <paramref name="q"/> in question.
        /// </summary>
        /// <typeparam name="T">The type of the element we wish to delete</typeparam>
        /// <param name="q">The type to update</param>
        /// <param name="objs">Either the Id or the Keys of the item we wish to delete</param>
        public static void DeleteOtherPropInRelationWithTHavingTPropertyTAndTNotHavingProperty<T>(DbContext context, Type q, params object[] objs)
        {
            if (GenericToolsTypeAnalysis.HasPropertyRelationNotList(q, typeof(T)))
            {
                List<string> propnames = GenericToolsTypeAnalysis.DynamicDBTypesForType(q).Where(kv => 
                                                                                                 kv.Value == typeof(T)
                                                                                                 )
                                                                                          .Select(kv => kv.Key)
                                                                                          .ToList();
                using (DbContext newcontext = (DbContext)Activator.CreateInstance(context.GetType()))
                {
                    foreach (string propname in propnames)
                    {
                        SetForTypePropertyWithGivenKeysToNullInNewContext<T>(newcontext, q, propname, objs);
                    }
                }
            }
            else
            {
                if (GenericToolsTypeAnalysis.HasPropertyRelationList(q, typeof(T)))
                {
                    List<string> propnames = GenericToolsTypeAnalysis.DynamicDBListTypesForType(q).Where(kv => 
                                                                                                         kv.Value == typeof(T)
                                                                                                         )
                                                                                                  .Select(kv => kv.Key)
                                                                                                  .ToList();
                    using (DbContext newcontext = (DbContext)Activator.CreateInstance(context.GetType()))
                    {
                        foreach (string propname in propnames)
                        {
                            RemoveForTypePropertyListElementWithGivenKeyInNewContext<T>(newcontext, q, propname, objs);
                        }
                    }
                }
                else
                {
                    //throw new HasNoPropertyRelationException(q, typeof(T));
                }
            }
        }

        /// <summary>
        /// For the type <paramref name="q"/>, using a new context <paramref name="context"/>, update the elements in case
        /// the element of type <typeparamref name="T"/> having either Id or keys <paramref name="objs"/> has to be deleted
        /// and the property of <paramref name="q"/> of type either <typeparamref name="T"/> or <see cref="IList{T}"/> has
        /// the annotation <see cref="RequiredAttribute"/>.
        /// <br/>
        /// Indeed, required properties are not handled well in EF in case of relationships, especially if they are
        /// of type <see cref="IList{T}"/> (an empty <see cref="IList"/> is not <see langword="null"/> and the annotation
        /// <see cref="RequiredAttribute"/> is interpreted as nullable = <see langword="false"/> and empty list are 
        /// thusly accepted)
        /// <br/> That is to say, for elements of type <paramref name="q"/>, if the property has an annotation 
        /// <see cref="RequiredAttribute"/>, and :
        /// <list type="bullet">
        /// <item>
        /// it is of type <typeparamref name="T"/>, delete the item
        /// </item>
        /// <item>
        /// it is of type <see cref="IList{T}"/>, remove the item from the list and delete the element if it were
        /// the only element remaining in the property.
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>It is assumed that the property of <paramref name="q"/> with name <paramref name="propname"/> is of
        /// type <typeparamref name="T"/> (and NOT <see cref="IList{T}"/>, 
        /// see <see cref="DeleteOrUpdateItemOfTypeWithRequiredListPropertyHavingGivenKeysInNewContext"/> for that case) 
        /// and that <typeparamref name="T"/> has no property representing that relationship. In other words it is assumed 
        /// that <paramref name="q"/> is part of <see cref="GetTypesInRelationWithTHavingRequiredTProperty{T}"/> with a property
        /// of type <typeparamref name="T"/>.</remarks>
        /// <typeparam name="T">The type of the element deleted with either Id or keys <paramref name="objs"/> for which actions 
        /// have to be taken</typeparam>
        /// <param name="context">The context in which to do this operation</param>
        /// <param name="q">The type of the elements to be updated before the deletion of the element of type <typeparamref name="T"/> with 
        /// either Id or keys <paramref name="objs"/></param>
        /// <param name="propname">The name of the property of <paramref name="q"/> having type <typeparamref name="T"/></param>
        /// <param name="objs">Either the Id or keys of the element of type <typeparamref name="T"/> deleted</param>
        private static void DeleteItemOfTypeWithRequiredPropertyHavingGivenKeysInNewContext<T>(DbContext context, Type q, string propname, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, null);
            MethodInfo methodQueryWherePropKeysAre = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                    "ListWherePropKeysAre", 
                                                                                                    BindingFlags.NonPublic | BindingFlags.Static
                                                                                                   )
                                                                                        .MakeGenericMethod(new Type[] { 
                                                                                                                        typeof(T), 
                                                                                                                        q 
                                                                                                                      }
                                                                                                          );
            req = methodQueryWherePropKeysAre.Invoke(
                                                        typeof(GenericToolsQueriesAndLists), 
                                                        new object[] { 
                                                                        req, 
                                                                        propname, 
                                                                        objs 
                                                                     }
                                                    );
            foreach (dynamic qItem in req)
            {
                using (DbContext context2 = (DbContext)Activator.CreateInstance(context.GetType()))
                {
                    dynamic qService2 = GetServiceFromContext(context, q);

                    MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                "GetKeysValues", 
                                                                                                BindingFlags.Public | BindingFlags.Static
                                                                                                )
                                                                                     .MakeGenericMethod(new Type[] { 
                                                                                                                    q 
                                                                                                                   }
                                                                                                       );
                    var qItem2 = qService2.FindByIdExcludes(
                                                            (object[])methodGetKeysValues.Invoke(
                                                                                                    typeof(GenericToolsTypeAnalysis), 
                                                                                                    new object[] { 
                                                                                                                    qItem 
                                                                                                                 }
                                                                                                )
                                                           );
                    qService2.Delete(qItem2);
                }
            }
        }

        /// <summary>
        /// For the type <paramref name="q"/>, using a new context <paramref name="context"/>, update the elements in case
        /// the element of type <typeparamref name="T"/> having either Id or keys <paramref name="objs"/> has to be deleted
        /// and the property of <paramref name="q"/> of type either <typeparamref name="T"/> or <see cref="IList{T}"/> has
        /// the annotation <see cref="RequiredAttribute"/>.
        /// <br/>
        /// Indeed, required properties are not handled well in EF in case of relationships, especially if they are
        /// of type <see cref="IList{T}"/> (an empty <see cref="IList"/> is not <see langword="null"/> and the annotation
        /// <see cref="RequiredAttribute"/> is interpreted as nullable = <see langword="false"/> and empty list are 
        /// thusly accepted)
        /// <br/> That is to say, for elements of type <paramref name="q"/>, if the property has an annotation 
        /// <see cref="RequiredAttribute"/>, and :
        /// <list type="bullet">
        /// <item>
        /// it is of type <typeparamref name="T"/>, delete the item
        /// </item>
        /// <item>
        /// it is of type <see cref="IList{T}"/>, remove the item from the list and delete the element if it were
        /// the only element remaining in the property.
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>It is assumed that the property of <paramref name="q"/> with name <paramref name="propname"/> is of
        /// type <see cref="IList{T}"/> (and NOT <typeparamref name="T"/>, 
        /// see <see cref="DeleteItemOfTypeWithRequiredPropertyHavingGivenKeysInNewContext"/> for that case) 
        /// and that <typeparamref name="T"/> has no property representing that relationship. In other words it is assumed 
        /// that <paramref name="q"/> is part of <see cref="GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingRequiredTProperty{T}"/> with a property
        /// of type <typeparamref name="T"/>.</remarks>
        /// <typeparam name="T">The type of the element deleted with either Id or keys <paramref name="objs"/> for which actions 
        /// have to be taken</typeparam>
        /// <param name="context">The context in which to do this operation</param>
        /// <param name="q">The type of the elements to be updated before the deletion of the element of type <typeparamref name="T"/> with 
        /// either Id or keys <paramref name="objs"/></param>
        /// <param name="propname">The name of the property of <paramref name="q"/> having type <typeparamref name="T"/></param>
        /// <param name="objs">Either the Id or keys of the element of type <typeparamref name="T"/> deleted</param>
        private static void DeleteOrUpdateItemOfTypeWithRequiredListPropertyHavingGivenKeysInNewContext<T>(DbContext context, Type q, string propname, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, null);

            MethodInfo methodListWherePropListCountainsElementWithGivenKeys = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                                            "ListWherePropListCountainsElementWithGivenKeys", 
                                                                                                                            BindingFlags.Public | BindingFlags.Static
                                                                                                                            )
                                                                                                                 .MakeGenericMethod(new Type[] {
                                                                                                                                                typeof(T), 
                                                                                                                                                q 
                                                                                                                                               }
                                                                                                                                   );
            req = methodListWherePropListCountainsElementWithGivenKeys.Invoke(
                                                                                typeof(GenericToolsQueriesAndLists), 
                                                                                new object[] { 
                                                                                                req, 
                                                                                                q, 
                                                                                                propname, 
                                                                                                objs 
                                                                                              }
                                                                             );

            foreach (var qItem in req)
            {
                using (DbContext context2 = (DbContext)Activator.CreateInstance(context.GetType()))
                {
                    dynamic qService2 = GetServiceFromContext(context2, q);

                    MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                    "GetKeysValues", 
                                                                                                    BindingFlags.Public | BindingFlags.Static
                                                                                               )
                                                                                     .MakeGenericMethod(new Type[] {    
                                                                                                                    q 
                                                                                                                   }
                                                                                                       );
                    var qItem2 = qService2.FindByIdIncludes(
                                                            (object[])methodGetKeysValues.Invoke(
                                                                                                    typeof(GenericToolsTypeAnalysis), 
                                                                                                    new object[] { 
                                                                                                                    qItem 
                                                                                                                 }
                                                                                                )
                                                           );

                    var oldValue = qItem2.GetType().GetProperty(propname).GetValue(qItem2);
                    if ((oldValue as IList).Count == 1)
                    {
                        using (DbContext context3 = (DbContext)Activator.CreateInstance(context.GetType()))
                        {
                            dynamic qService3 = GetServiceFromContext(context3, q);
                            var qItem3 = qService3.FindByIdExcludes(
                                                                    (object[])methodGetKeysValues.Invoke(
                                                                                                            typeof(GenericToolsTypeAnalysis), 
                                                                                                            new object[] { 
                                                                                                                            qItem 
                                                                                                                         }
                                                                                                        )
                                                                   );
                            qService3.Delete(qItem3);
                        }

                    }
                    else
                    {
                        MethodInfo methodListRemoveElementWithGivenKeys = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                                        "ListRemoveElementWithGivenKeys", 
                                                                                                                        BindingFlags.Public | BindingFlags.Static
                                                                                                                        )
                                                                                                             .MakeGenericMethod(new Type[] { 
                                                                                                                                            typeof(T)   
                                                                                                                                           }
                                                                                                                               );
                        var newValue = methodListRemoveElementWithGivenKeys.Invoke(
                                                                                    typeof(GenericToolsQueriesAndLists), 
                                                                                    new object[] { 
                                                                                                    oldValue, 
                                                                                                    objs 
                                                                                                 }
                                                                                  );
                        qService2.UpdateOne(qItem2, propname, newValue);
                    }
                }
            }
        }

        /// <summary>
        /// An object of type <typeparamref name="T"/> with either Id or keys <paramref name="objs"/> is deleted.
        /// Every type <paramref name="q"/> in <see cref="GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingRequiredTProperty"/> has to be updated
        /// manually. Indeed, required properties are not handled well in EF in case of relationships, especially if they are
        /// of type <see cref="IList"/> (an empty <see cref="List"/> is not <see langword="null"/> and the annotation
        /// <see cref="RequiredAttribute"/> is interpreted as nullable = <see langword="false"/>)
        /// <br/>
        /// If the property representing that relation in type <paramref name="q"/> is of type <typeparamref name="T"/>,
        /// this will set, for the appropriate elements of type <paramref name="q"/>, either the property to <see langword="null"/>,
        /// or if it has annotation <see cref="RequiredAttribute"/> it will delete it.
        /// <br/>
        /// if the property representing that relation in type <paramref name="q"/> is of type <see cref="IList{T}"/>,
        /// this will set, for the appropriate elements of type <paramref name="q"/>, the property to the list without the element
        /// with either Id or keys <paramref name="objs"/>. Furthermore, if such property has annotation <see cref="RequiredAttribute"/>,
        /// and the item of type <typeparamref name="T"/> to delete was the only remaining element of the list, it will delete
        /// the element of type <paramref name="q"/> in question.
        /// </summary>
        /// <typeparam name="T">The type of the object we wish to delete</typeparam>
        /// <param name="q">The type being handled</param>
        /// <param name="objs">The Id or Keys of the object of type <typeparamref name="T"/> to delete</param>
        public static void DeleteOtherPropInRelationWithTHavingRequiredTProperty<T>(DbContext context, Type q, params object[] objs)
        {
            // person -> finger
            // person -> action
            if (GenericToolsTypeAnalysis.HasPropertyRelationNotList(q, typeof(T)))
            {
                List<string> propnames = GenericToolsTypeAnalysis.DynamicDBTypesForType(q).Where(kv => 
                                                                                                 kv.Value == typeof(T)
                                                                                                )
                                                                                          .Select(kv => 
                                                                                                  kv.Key
                                                                                                 )
                                                                                          .Where(propname => 
                                                                                                 q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) != null
                                                                                                 )
                                                                                          .ToList();
                using (DbContext newcontext = (DbContext)Activator.CreateInstance(context.GetType()))
                {
                    foreach (string propname in propnames)
                    {
                        DeleteItemOfTypeWithRequiredPropertyHavingGivenKeysInNewContext<T>(newcontext, q, propname, objs);
                    }
                }
            }
            else
            {
                if (GenericToolsTypeAnalysis.HasPropertyRelationList(q, typeof(T)))
                {
                    List<string> propnames = GenericToolsTypeAnalysis.DynamicDBListTypesForType(q).Where(kv => 
                                                                                                         kv.Value == typeof(T)
                                                                                                        )
                                                                                                  .Select(kv => kv.Key)
                                                                                                  .Where(propname => 
                                                                                                         q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) != null
                                                                                                         )
                                                                                                  .ToList();
                    using (DbContext newcontext = (DbContext)Activator.CreateInstance(context.GetType()))
                    {
                        foreach (string propname in propnames)
                        {
                            DeleteOrUpdateItemOfTypeWithRequiredListPropertyHavingGivenKeysInNewContext<T>(newcontext, q, propname, objs);
                        }
                    }
                }
                else
                {
                    //throw new HasNoPropertyRelationException(q, typeof(T));
                }
            }
        }

        /// <summary>
        /// For the type <paramref name="q"/>, using a new context <paramref name="context"/>, update the elements in case
        /// the element of type <typeparamref name="T"/> having either Id or keys <paramref name="objs"/> has to be deleted. That is
        /// to say, for elements of type <paramref name="q"/> such that their properties <paramref name="propnames"/> of type <typeparamref name="T"/>
        /// is not <see langword="null"/>,
        /// <list type="bullet">
        /// <item>
        /// If the property doesn't have an annotation <see cref="RequiredAttribute"/>, just set it to <see langword="null"/>. 
        /// (EF won't do it by itself using TRepository since <typeparamref name="T"/> has no property for that relationship)
        /// </item>
        /// <item>
        /// If the property has an annotation <see cref="RequiredAttribute"/>, delete the item (EF won't do it by itself for
        /// the same reason)
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>It is assumed that the properties of <paramref name="q"/> with name <paramref name="propnames"/> is of
        /// type <typeparamref name="T"/> (and NOT <see cref="IList{T}"/>) and that <typeparamref name="T"/> has no
        /// property representing that relationship. In other words it is assumed that <paramref name="q"/> is part
        /// of <see cref="GenericToolsTypeAnalysis.GetTypesForWhichTHasManyProperties{T}"/></remarks>
        /// <typeparam name="T">The type of the element deleted with either Id or keys <paramref name="objs"/> for which actions 
        /// have to be taken</typeparam>
        /// <param name="context">The context in which to do this operation</param>
        /// <param name="q">The type of the elements to be updated before the deletion of the element of type <typeparamref name="T"/> with 
        /// either Id or keys <paramref name="objs"/></param>
        /// <param name="propnames">The names of the properties of <paramref name="q"/> having type <typeparamref name="T"/></param>
        /// <param name="objs">Either the Id or keys of the element of type <typeparamref name="T"/> deleted</param>
        private static void SetForMultipleTypePropertyWithGivenKeysToNullInNewContext<T>(DbContext context, Type q, List<string> propnames, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            MethodInfo methodListWherePropNotNull = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                    "ListWherePropNotNull", 
                                                                                                    BindingFlags.Public | BindingFlags.Static
                                                                                                  )
                                                                                       .MakeGenericMethod(new Type[] { 
                                                                                                                        q 
                                                                                                                      }
                                                                                                         );
            dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, null);

            foreach (string propname in propnames)
            {
                req = methodListWherePropNotNull.Invoke(
                                                            typeof(GenericToolsQueriesAndLists), 
                                                            new object[] { 
                                                                            req, 
                                                                            propname 
                                                                         }
                                                       );
            }

            MethodInfo methodQueryWhereMultiplePropKeysAre = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                            "ListWhereMultiplePropKeysAre", 
                                                                                                            BindingFlags.Public | BindingFlags.Static
                                                                                                          )
                                                                                                .MakeGenericMethod(new Type[] { 
                                                                                                                                typeof(T), 
                                                                                                                                q 
                                                                                                                              }
                                                                                                                  );
            req = methodQueryWhereMultiplePropKeysAre.Invoke(
                                                                typeof(GenericToolsQueriesAndLists), 
                                                                new object[] { 
                                                                                req, 
                                                                                propnames, 
                                                                                objs 
                                                                             }
                                                            );

            foreach (var qItem in req)
            {
                using (DbContext context2 = (DbContext)Activator.CreateInstance(context.GetType()))
                {
                    dynamic qService2 = GetServiceFromContext(context2, q);

                    MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                    "GetKeysValues", 
                                                                                                    BindingFlags.Public | BindingFlags.Static
                                                                                                )
                                                                                     .MakeGenericMethod(new Type[] { 
                                                                                                                    q 
                                                                                                                   }
                                                                                                       );
                    var qItem2 = qService2.FindByIdIncludes(
                                                            (object[])methodGetKeysValues.Invoke(
                                                                                                    typeof(GenericToolsTypeAnalysis), 
                                                                                                    new object[] { 
                                                                                                                    qItem 
                                                                                                                 }
                                                                                                )
                                                           );

                    bool isDeleted = false;
                    foreach (string propname in propnames)
                    {
                        if (q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) != null)
                        {
                            using (DbContext context3 = (DbContext)Activator.CreateInstance(context.GetType()))
                            {
                                dynamic qService3 = GetServiceFromContext(context3, q);
                                var qItem3 = qService3.FindByIdExcludes(
                                                                        (object[])methodGetKeysValues.Invoke(
                                                                                                                typeof(GenericToolsTypeAnalysis), 
                                                                                                                new object[] { 
                                                                                                                                qItem 
                                                                                                                             }
                                                                                                             )
                                                                       );
                                qService3.Delete(qItem3);
                            }
                            isDeleted = true;
                            break;
                        }
                        else
                        {
                            q.GetProperty(propname).SetValue(qItem2, null);
                        }
                    }
                    if (!isDeleted)
                        qService2.Update(qItem2);
                }
            }
        }

        /// <summary>
        /// In a given context <paramref name="context"/>, for the type <paramref name="q"/> with the properties with name 
        /// <paramref name="propnames"/> of type <typeparamref name="T"/>, prepare the deletion of a given element of type <typeparamref name="T"/>
        /// with either Id or keys <paramref name="objs"/>. 
        /// <br/>
        /// That is to say, for every element of type <paramref name="q"/> such that
        /// their properties <paramref name="propnames"/> is of type <see cref="IList{T}"/> and contains the element of either Id or keys
        /// <paramref name="objs"/>
        /// <list type="bullet">
        /// <item>
        /// if one of the property <paramref name="propnames"/> of <paramref name="q"/> has the annotation <see cref="RequiredAttribute"/> and the
        /// list has only the item to delete remaining, remove the element of type <paramref name="q"/>
        /// </item>
        /// <item>
        /// otherwise just remove the element from the list.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type of the element to be deleted</typeparam>
        /// <param name="context">The context</param>
        /// <param name="q">The type of the elements to be updated</param>
        /// <param name="propnames">The names of the properties of q of type <see cref="IList{T}"/></param>
        /// <param name="objs">Either the Id or the keys</param>
        private static void RemoveForMultipleTypePropertyListElementWithGivenKeyInNewContext<T>(DbContext context, Type q, List<string> propnames, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            MethodInfo methodExpressionListWhereMultiplePropListCountainsElementWithGivenKeys = typeof(GenericToolsExpressionTrees).GetMethod(
                                                                                                                                                "ExpressionListWhereMultiplePropListCountainsElementWithGivenKeys", 
                                                                                                                                                BindingFlags.Public | BindingFlags.Static
                                                                                                                                             )
                                                                                                                                   .MakeGenericMethod(new Type[] { 
                                                                                                                                                                    typeof(T), 
                                                                                                                                                                    q 
                                                                                                                                                                 }
                                                                                                                                                     );
            dynamic func = methodExpressionListWhereMultiplePropListCountainsElementWithGivenKeys.Invoke(
                                                                                                            typeof(GenericToolsExpressionTrees), 
                                                                                                            ConcatArrayWithParams(
                                                                                                                                    new object[] { propnames }, 
                                                                                                                                    objs
                                                                                                                                 )
                                                                                                        );

            dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, func);

            foreach (var qItem in req)
            {
                using (DbContext context2 = (DbContext)Activator.CreateInstance(context.GetType()))
                {
                    dynamic qService2 = GetServiceFromContext(context2, q);

                    MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                    "GetKeysValues", 
                                                                                                    BindingFlags.Public | BindingFlags.Static
                                                                                               )
                                                                                     .MakeGenericMethod(new Type[] { 
                                                                                                                    q 
                                                                                                                   }
                                                                                                       );
                    var qItem2 = qService2.FindByIdIncludes(
                                                            (object[])methodGetKeysValues.Invoke(
                                                                                                    typeof(GenericToolsTypeAnalysis), 
                                                                                                    new object[] { 
                                                                                                                    qItem 
                                                                                                                 }
                                                                                                )
                                                           );

                    bool isDeleted = false;
                    foreach (string propname in propnames)
                    {
                        var oldValue = q.GetProperty(propname).GetValue(qItem2);
                        if (q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) != null
                            && ((oldValue as IList).Count == 1))
                        {
                            using (DbContext context3 = (DbContext)Activator.CreateInstance(context.GetType()))
                            {
                                dynamic qService3 = GetServiceFromContext(context3, q);
                                var qItem3 = qService3.FindByIdExcludes(
                                                                        (object[])methodGetKeysValues.Invoke(
                                                                                                                typeof(GenericToolsTypeAnalysis), 
                                                                                                                new object[] { 
                                                                                                                                qItem 
                                                                                                                             }
                                                                                                            )
                                                                       );
                                qService3.Delete(qItem3);

                            }
                            isDeleted = true;
                            break;
                        }
                        else
                        {
                            MethodInfo methodListRemoveElementWithGivenKeys = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                                            "ListRemoveElementWithGivenKeys", 
                                                                                                                            BindingFlags.Public | BindingFlags.Static
                                                                                                                           )
                                                                                                                 .MakeGenericMethod(new Type[] { 
                                                                                                                                                    typeof(T) 
                                                                                                                                               }
                                                                                                                                   );
                            var newValue = methodListRemoveElementWithGivenKeys.Invoke(
                                                                                        typeof(GenericToolsQueriesAndLists), 
                                                                                        new object[] { 
                                                                                                        oldValue, 
                                                                                                        objs 
                                                                                                     }
                                                                                      );
                            q.GetProperty(propname).SetValue(qItem2, newValue);
                        }
                    }
                    if (!isDeleted)
                        qService2.Update(qItem2);
                }
            }
        }

        /// <summary>
        /// An object of type <typeparamref name="T"/> with either Id or keys <paramref name="objs"/> is deleted.
        /// Every type <paramref name="q"/> in <see cref="GenericToolsTypeAnalysis.GetTypesForWhichTHasManyProperties"/> has to be updated
        /// manually. Indeed, if we try to remove the object of type <typeparamref name="T"/> using EF, it will load
        /// in the context all the properties relating to relationship with those types. The point being, there will be many
        /// properties loaded. What can happen is an object of type <paramref name="q"/> might appear multiple times and therefore
        /// EF will load it multiple times. Thus, an element of type <paramref name="q"/> with the same primary key (or keys) will be loaded in the context,
        /// which will throw an exception if we simply do db.Set.Delete(item). Therefore, we have to manage those
        /// separately.
        /// <br/>
        /// For now this does not work.
        /// </summary>
        /// <typeparam name="T">The type of the object we wish to delete</typeparam>
        /// <param name="q">The type being handled</param>
        /// <param name="objs">The Id or Keys of the object of type <typeparamref name="T"/> to delete</param>
        public static void DeleteOtherPropInSeveralRelationshipsWithT<T>(DbContext context, Type q, params object[] objs)
        {
            if (GenericToolsTypeAnalysis.HasPropertyRelationNotList(q, typeof(T)))
            {
                List<string> propnames = GenericToolsTypeAnalysis.DynamicDBTypesForType(q).Where(kv => 
                                                                                                 kv.Value == typeof(T)
                                                                                                )
                                                                                          .Select(kv => 
                                                                                                  kv.Key
                                                                                                 )
                                                                                          .ToList();
                using (DbContext newcontext = (DbContext)Activator.CreateInstance(context.GetType()))
                {
                    SetForMultipleTypePropertyWithGivenKeysToNullInNewContext<T>(newcontext, q, propnames, objs);
                }
            }
            else
            {
                if (GenericToolsTypeAnalysis.HasPropertyRelationList(q, typeof(T)))
                {
                    List<string> propnames = GenericToolsTypeAnalysis.DynamicDBListTypesForType(q).Where(kv => 
                                                                                                         kv.Value == typeof(T)
                                                                                                         )
                                                                                                  .Select(kv => 
                                                                                                          kv.Key
                                                                                                          )
                                                                                                  .ToList();
                    using (DbContext newcontext = (DbContext)Activator.CreateInstance(context.GetType()))
                    {
                        RemoveForMultipleTypePropertyListElementWithGivenKeyInNewContext<T>(newcontext, q, propnames, objs);
                    }
                }
                else
                {
                    //throw new HasNoPropertyRelationException(q, typeof(T));
                }
            }
        }

        /// <summary>
        /// An object of type <typeparamref name="T"/> with either Id or keys <paramref name="objs"/> is about to be updated to value <paramref name="newItem"/>.
        /// The type <paramref name="q"/> has a required property of type <typeparamref name="T"/>, 
        /// and therefore if the relationship between those types changes, some items of type <paramref name="q"/> may be removed.
        /// <br/>
        /// In more details, elements of type <paramref name="q"/> to be removed are such that :
        /// <list type="bullet">
        /// <item>
        /// q.qpropname = olditem (before update)
        /// </item>
        /// <item>
        /// newItem.tpropname changes value
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type of object to be updated</typeparam>
        /// <param name="context">The context</param>
        /// <param name="q">the type of objects to remove if necessary</param>
        /// <param name="tpropname">The name of the property for t</param>
        /// <param name="newItem">The new value to be updated</param>
        /// <param name="objs">Either the id or keys of <paramref name="newItem"/></param>
        private static void UpdateItemOfTypeWithRequiredPropOfTypeInNewContext<T>(DbContext context, Type q, string tpropname, T newItem, params object[] objs)
        {

            if (typeof(T).GetProperty(tpropname).PropertyType == q)
            {
                dynamic tService = GetServiceFromContext(context, typeof(T));

                T oldItem = tService.FindByIdIncludes(objs);

                var oldItemProp = typeof(T).GetProperty(tpropname).GetValue(oldItem);

                var newItemProp = typeof(T).GetProperty(tpropname).GetValue(newItem);

                if (oldItemProp != newItemProp)
                {
                    using (DbContext context2 = (DbContext)Activator.CreateInstance(context.GetType()))
                    {
                        dynamic qService = GetServiceFromContext(context2, q);

                        MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                    "GetKeysValues", 
                                                                                                    BindingFlags.Public | BindingFlags.Static
                                                                                                   )
                                                                                         .MakeGenericMethod(new Type[] { 
                                                                                                                        q 
                                                                                                                       }
                                                                                                           );
                        var qItem = qService.FindByIdExcludes(
                                                                (object[])methodGetKeysValues.Invoke(
                                                                                                        typeof(GenericToolsTypeAnalysis), 
                                                                                                        new object[] { 
                                                                                                                        oldItemProp 
                                                                                                                     }
                                                                                                    )
                                                             );

                        qService.Delete(qItem);
                    }
                }
            }
            else
            {
                if (typeof(T).GetProperty(tpropname).PropertyType == typeof(IList<>).MakeGenericType(q))
                {
                    dynamic tService = GetServiceFromContext(context, typeof(T));

                    T oldItem = tService.FindByIdIncludes(objs);

                    var oldItemProp = typeof(T).GetProperty(tpropname).GetValue(oldItem) as IList;

                    var newItemProp = typeof(T).GetProperty(tpropname).GetValue(newItem) as IList;

                    foreach (var item in oldItemProp)
                    {
                        if (!newItemProp.Contains(item))
                        {
                            using (DbContext context2 = (DbContext)Activator.CreateInstance(context.GetType()))
                            {
                                dynamic qService = GetServiceFromContext(context2, q);

                                MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                            "GetKeysValues", 
                                                                                                            BindingFlags.Public | BindingFlags.Static
                                                                                                            )
                                                                                                 .MakeGenericMethod(new Type[] { 
                                                                                                                                q 
                                                                                                                               }
                                                                                                                   );
                                var qItem = qService.FindByIdExcludes(
                                                                        (object[])methodGetKeysValues.Invoke(
                                                                                                                typeof(GenericToolsTypeAnalysis), 
                                                                                                                new object[] { 
                                                                                                                                item 
                                                                                                                             }
                                                                                                            )
                                                                     );

                                qService.Delete(qItem);
                            }
                        }
                    }
                }
                else
                {
                    //throw new HasNoPropertyRelationException(q, typeof(T));
                }
            }
        }

        /// <summary>
        /// An object of type <typeparamref name="T"/> with either Id or keys <paramref name="objs"/> is about to be updated to value <paramref name="newItem"/>.
        /// The type <paramref name="q"/> has a required property of name <paramref name="qpropname"/> and of type <see cref="IList{T}"/>, 
        /// and therefore if the relationship between those types changes, some items of type <paramref name="q"/> may be removed.
        /// <br/>
        /// In more details, elements of type <paramref name="q"/> to be removed are such that :
        /// <list type="bullet">
        /// <item>
        /// q.qpropname contains only oldvalue (before update)
        /// </item>
        /// <item>
        /// newItem.<paramref name="tpropname"/> changes value
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type of object to be updated</typeparam>
        /// <param name="context">The context</param>
        /// <param name="q">the type of objects to remove if necessary</param>
        /// <param name="qpropname">The name of the property for q</param>
        /// <param name="tpropname">The name of the property for t</param>
        /// <param name="newItem">The new value to be updated</param>
        /// <param name="objs">Either the id or keys of <paramref name="newItem"/></param>
        private static void UpdateItemOfTypeWithRequiredPropOfListTypeInNewContext<T>(DbContext context, Type q, string qpropname, string tpropname, T newItem, params object[] objs)
        {
            if (typeof(T).GetProperty(tpropname).PropertyType == q)
            {
                dynamic tService = GetServiceFromContext(context, typeof(T));

                T oldItem = tService.FindByIdIncludes(objs);

                var oldItemProp = typeof(T).GetProperty(tpropname).GetValue(oldItem);

                var newItemProp = typeof(T).GetProperty(tpropname).GetValue(newItem);

                if (oldItemProp != newItemProp)
                {
                    if (oldItemProp == null || 
                        ((oldItemProp) as IList).Count == 1)
                    {
                        using (DbContext context2 = (DbContext)Activator.CreateInstance(context.GetType()))
                        {
                            dynamic qService = GetServiceFromContext(context, q);

                            MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                        "GetKeysValues", 
                                                                                                        BindingFlags.Public | BindingFlags.Static
                                                                                                       )
                                                                                             .MakeGenericMethod(new Type[] { 
                                                                                                                            q 
                                                                                                                           }
                                                                                                               );
                            var qItem = qService.FindByIdExcludes(
                                                                    (object[])methodGetKeysValues.Invoke(
                                                                                                            typeof(GenericToolsTypeAnalysis), 
                                                                                                            new object[] { 
                                                                                                                            oldItemProp 
                                                                                                                         }
                                                                                                        )
                                                                 );

                            qService.Delete(qItem);
                        }
                    }
                }
            }
            else
            {
                if (typeof(T).GetProperty(tpropname).PropertyType == typeof(IList<>).MakeGenericType(q))
                {
                    dynamic qService = GetServiceFromContext(context, q);

                    dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, null);

                    MethodInfo methodListWherePropListCountainsOnlyElementWithGivenKeys = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                                                        "ListWherePropListCountainsOnlyElementWithGivenKeys", 
                                                                                                                                        BindingFlags.Public | BindingFlags.Static
                                                                                                                                        )
                                                                                                                             .MakeGenericMethod(new Type[] { 
                                                                                                                                                            typeof(T), 
                                                                                                                                                            q 
                                                                                                                                                           }
                                                                                                                                               );
                    req = methodListWherePropListCountainsOnlyElementWithGivenKeys.Invoke(
                                                                                            typeof(GenericToolsQueriesAndLists), 
                                                                                            ConcatArrayWithParams(
                                                                                                                    new object[] { 
                                                                                                                                    req, 
                                                                                                                                    q, 
                                                                                                                                    qpropname 
                                                                                                                                 }, 
                                                                                                                    objs)
                                                                                         );

                    if (typeof(T).GetProperty(tpropname).GetValue(newItem) != null && 
                        (typeof(T).GetProperty(tpropname).GetValue(newItem) as IList).Count != 0)
                    {
                        MethodInfo methodListWhereOtherTypePropListNotContains = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                                                "ListWhereOtherTypePropListNotContains", 
                                                                                                                                BindingFlags.Public | BindingFlags.Static
                                                                                                                               )
                                                                                                                    .MakeGenericMethod(new Type[] { 
                                                                                                                                                    typeof(T), 
                                                                                                                                                    q 
                                                                                                                                                  }
                                                                                                                                      );
                        req = methodListWhereOtherTypePropListNotContains.Invoke(
                                                                                    typeof(GenericToolsQueriesAndLists), 
                                                                                    new object[] { 
                                                                                                    req, 
                                                                                                    newItem, 
                                                                                                    tpropname, 
                                                                                                    GenericToolsTypeAnalysis.NbrOfKeys(q) 
                                                                                                 }
                                                                                );
                    }

                    foreach (var qItem in req)
                    {
                        using (DbContext context2 = (DbContext)Activator.CreateInstance(context.GetType()))
                        {
                            dynamic qService2 = GetServiceFromContext(context2, q);

                            MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                        "GetKeysValues", 
                                                                                                        BindingFlags.Public | BindingFlags.Static
                                                                                                       )
                                                                                             .MakeGenericMethod(new Type[] {
                                                                                                                            q 
                                                                                                                           }
                                                                                                               );
                            var qItem2 = qService2.FindByIdExcludes(
                                                                    (object[])methodGetKeysValues.Invoke(
                                                                                                            typeof(GenericToolsTypeAnalysis), 
                                                                                                            new object[] { 
                                                                                                                            qItem 
                                                                                                                         }
                                                                                                        )
                                                                   );

                            qService2.Delete(qItem2);
                        }
                    }
                }
                else
                {
                    //throw new HasNoPropertyRelationException(q, typeof(T));
                }
            }

        }

        /// <summary>
        /// When an element of type <typeparamref name="T"/> get updated, some action have to be taken for types with
        /// a relationship with <typeparamref name="T"/> and a property of type either <typeparamref name="T"/> or <see cref="IList{T}"/> that is
        /// required. 
        /// <br/>
        /// Indeed, required properties are not handled well in EF in case of relationships, especially if they are
        /// of type <see cref="IList"/> (an empty <see cref="List"/> is not <see langword="null"/> and the annotation
        /// <see cref="RequiredAttribute"/> is interpreted as nullable = <see langword="false"/>)
        /// <br/>
        /// This handles it and removes the items of type <paramref name="q"/> that either :
        /// <list type="bullet">
        /// <item>
        /// have a property of type <typeparamref name="T"/> that is required, and that <paramref name="newItem"/> is no longer
        /// linked to that element
        /// </item>
        /// <item>
        /// have a property of type <see cref="IList{T}"/> that is required, and that <paramref name="newItem"/> was the last
        /// element of the list but is no longer linked to that element
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type of the item being updated</typeparam>
        /// <param name="q">The type of the elements being removed if necessary</param>
        /// <param name="newItem">The new item to be updated</param>
        public static void UpdateOtherPropInRelationWithTHavingRequiredTProperty<T>(DbContext context, Type q, T newItem, string tpropname, string qpropname)
        {
            if (q.GetProperty(qpropname).PropertyType == typeof(T))
            {
                using (DbContext newcontext = (DbContext)Activator.CreateInstance(context.GetType()))
                {
                    UpdateItemOfTypeWithRequiredPropOfTypeInNewContext(newcontext, q, tpropname, newItem, GenericToolsTypeAnalysis.GetKeysValues(newItem));
                }
            }
            else
            {
                if (q.GetProperty(qpropname).PropertyType == typeof(IList<>).MakeGenericType(typeof(T)))
                {
                    using (DbContext newcontext = (DbContext)Activator.CreateInstance(context.GetType()))
                    {
                        UpdateItemOfTypeWithRequiredPropOfListTypeInNewContext(newcontext, q, qpropname, tpropname, newItem, GenericToolsTypeAnalysis.GetKeysValues(newItem));
                    }
                }
                else
                {
                    //throw new HasNoPropertyRelationException(q, typeof(T));
                }
            }
        }
    }
}