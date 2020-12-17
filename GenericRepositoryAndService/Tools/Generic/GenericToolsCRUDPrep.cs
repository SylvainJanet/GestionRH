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
        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.GetServiceFromContext(System.Data.Entity.DbContext,System.Type)"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.SetForTypePropertyWithGivenKeysToNullInNewContext``1(System.Data.Entity.DbContext,System.Type,System.String,System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.ConcatArrayWithParams(System.Object[],System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.RemoveForTypePropertyListElementWithGivenKeyInNewContext``1(System.Data.Entity.DbContext,System.Type,System.String,System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.DeleteOtherPropInRelationWithTHavingTPropertyTAndTNotHavingProperty``1(System.Data.Entity.DbContext,System.Type,System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.DeleteItemOfTypeWithRequiredPropertyHavingGivenKeysInNewContext``1(System.Data.Entity.DbContext,System.Type,System.String,System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.DeleteOrUpdateItemOfTypeWithRequiredListPropertyHavingGivenKeysInNewContext``1(System.Data.Entity.DbContext,System.Type,System.String,System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.DeleteOtherPropInRelationWithTHavingRequiredTProperty``1(System.Data.Entity.DbContext,System.Type,System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.SetForMultipleTypePropertyWithGivenKeysToNullInNewContext``1(System.Data.Entity.DbContext,System.Type,System.Collections.Generic.List{System.String},System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.RemoveForMultipleTypePropertyListElementWithGivenKeyInNewContext``1(System.Data.Entity.DbContext,System.Type,System.Collections.Generic.List{System.String},System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.DeleteOtherPropInSeveralRelationshipsWithT``1(System.Data.Entity.DbContext,System.Type,System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.UpdateItemOfTypeWithRequiredPropOfTypeInNewContext``1(System.Data.Entity.DbContext,System.Type,System.String,``0,System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.UpdateItemOfTypeWithRequiredPropOfListTypeInNewContext``1(System.Data.Entity.DbContext,System.Type,System.String,System.String,``0,System.Object[])"]/*'/>
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

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDPrep.UpdateOtherPropInRelationWithTHavingRequiredTProperty``1(System.Data.Entity.DbContext,System.Type,``0,System.String,System.String)"]/*'/>
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