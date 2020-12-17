using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GenericRepositoryAndService.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace GenericRepositoryAndService.Tools.Generic
{
    public abstract class GenericToolsCRUD
    {
        /// <summary>
        /// Find an object of type <paramref name="typeofElement"/> with either Id or Keys <paramref name="objs"/>
        /// in context <paramref name="newContext"/>.
        /// <br/>
        /// Essentially, this is a dynamic call to the generic method <see cref="GenericRepository{T}.FindByIdIncludesTrackedInNewContext(MyDbContext, object[])"/>
        /// where <c>T</c> is <paramref name="typeofElement"/>.
        /// </summary>
        /// <remarks>
        /// Other properties will not be included, and element will be tracked.
        /// </remarks>
        /// <param name="typeofElement">The type of the the object searched</param>
        /// <param name="newContext">The context in which the search has to be done</param>
        /// <param name="objs">Either the Id or the keys values of the object searched</param>
        /// <returns>The object if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public static object FindByKeysInNewContextForType(Type typeofElement, DbContext newContext, object[] objs)
        {
            MethodInfo methodCheckIfObjectIsKey = typeof(GenericToolsTypeAnalysis).GetMethod(
                                                                                                "CheckIfObjectIsKey", 
                                                                                                BindingFlags.Public | BindingFlags.Static
                                                                                            )
                                                                                  .MakeGenericMethod(new[] { 
                                                                                                                typeofElement 
                                                                                                           }
                                                                                                    );
            methodCheckIfObjectIsKey.Invoke(
                                            typeof(GenericToolsTypeAnalysis), 
                                            new object[] { 
                                                            objs 
                                                         }
                                           );
            MethodInfo methodQueryWhereKeysAre = typeof(GenericToolsQueriesAndLists).GetMethod(
                                                                                                "QueryWhereKeysAre", 
                                                                                                BindingFlags.Public | BindingFlags.Static
                                                                                              )
                                                                                    .MakeGenericMethod(new[] { 
                                                                                                                typeofElement 
                                                                                                             }
                                                                                                      );
            object query = methodQueryWhereKeysAre.Invoke(
                                                            typeof(GenericToolsQueriesAndLists), 
                                                            new object[] { 
                                                                            newContext.Set(typeofElement), 
                                                                            objs 
                                                                         }
                                                        );
            MethodInfo methodSingleOrDefault = typeof(Queryable).GetMethods()
                                                                .Single(m => 
                                                                        m.Name == "SingleOrDefault" &&
                                                                        m.GetParameters().Length == 1
                                                                        )
                                                                .MakeGenericMethod(new[] { 
                                                                                            typeofElement 
                                                                                         }
                                                                                   );
            return methodSingleOrDefault.Invoke(
                                                typeof(Queryable), 
                                                new object[] { 
                                                                query 
                                                             }
                                               );
        }

        /// <summary>
        /// Every step that has to be taken into account before deleting an object of type <typeparamref name="T"/> having
        /// either Id or keys <paramref name="objs"/>.
        /// <br/>
        /// See <see cref="GenericToolsCRUDPrep.DeleteOtherPropInRelationWithTHavingTPropertyTAndTNotHavingProperty"/>, 
        /// <see cref="GenericToolsCRUDPrep.DeleteOtherPropInRelationWithTHavingRequiredTProperty"/>,
        /// <see cref="GenericToolsCRUDPrep.DeleteOtherPropInSeveralRelationshipsWithT"/> for more details.
        /// <br/> 
        /// In a nutshell :
        /// <list type="bullet">
        /// <item>
        /// Every type <paramref name="q"/> in <see cref="GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty{T}"/> has to be updated
        /// manually. 
        /// <br/>
        /// Indeed, <typeparamref name="T"/> has no property representing that relation, and thus no element of 
        /// type <paramref name="q"/> will be loaded in the context and changed, wich will result in exceptions if not
        /// taken care of.
        /// </item>
        /// <item>
        /// Every type <paramref name="q"/> in <see cref="GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingRequiredTProperty"/> has to be updated
        /// manually. 
        /// <br/>
        /// Indeed, required properties are not handled well in EF in case of relationships, especially if they are
        /// of type <see cref="IList"/> (an empty <see cref="List"/> is not <see langword="null"/> and the annotation
        /// <see cref="RequiredAttribute"/> is interpreted as nullable = <see langword="false"/>)
        /// </item>
        /// <item>
        /// Every type <paramref name="q"/> in <see cref="GenericToolsTypeAnalysis.GetTypesForWhichTHasManyProperties"/> has to be updated
        /// manually. 
        /// <br/>
        /// Indeed, if we try to remove the object of type <typeparamref name="T"/> using EF, it will load
        /// in the context all the properties relating to relationship with those types. The point being, there will be many
        /// properties loaded. 
        /// <br/>
        /// What can happen is an object of type <paramref name="q"/> might appear multiple times and therefore
        /// EF will load it multiple times. Thus, an element of type <paramref name="q"/> with the same primary key (or keys) will be loaded in the context,
        /// which will throw an exception if we simply do db.Set.Delete(item). Therefore, we have to manage those
        /// separately.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type of the object to delete</typeparam>
        /// <param name="objs">Either the Id or the keys of the object to delete</param>
        public static void PrepareDelete<T>(DbContext context, params object[] objs)
        {
            foreach (Type type in GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty<T>())
            {
                GenericToolsCRUDPrep.DeleteOtherPropInRelationWithTHavingTPropertyTAndTNotHavingProperty<T>(context, type, objs);
            }
            foreach (Type type in GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingRequiredTProperty<T>())
            {
                GenericToolsCRUDPrep.DeleteOtherPropInRelationWithTHavingRequiredTProperty<T>(context, type, objs);
            }
            foreach (Type type in GenericToolsTypeAnalysis.GetTypesForWhichTHasManyProperties<T>())
            {
                GenericToolsCRUDPrep.DeleteOtherPropInSeveralRelationshipsWithT<T>(context, type, objs);
            }
        }

        /// <summary>
        /// Creates the array of all the values of properties of the element <paramref name="t"/> of type <typeparamref name="T"/>
        /// to save that represent a relationship involving <typeparamref name="T"/>.
        /// <br/>
        /// Furthermore, for types appearing multiple times as properties in <typeparamref name="T"/>, set those to
        /// <see cref="PropToNull"/> if necessary. See <see cref="GenericRepository{T}.Save(T, object[])"/> for further details.
        /// </summary>
        /// <typeparam name="T">The type of the element to save</typeparam>
        /// <param name="t">The element to save</param>
        /// <returns>The array of all the values of properties of <paramref name="t"/> representing relationships involving <typeparamref name="T"/>,
        /// with values set to <see cref="PropToNull"/> if necessary.</returns>
        public static object[] PrepareSave<T>(T t)
        {
            IEnumerable<Type> TypesForWhichTHasManyProperties = GenericToolsTypeAnalysis.GetTypesForWhichTHasManyProperties<T>();
            Dictionary<string, Type> dynamicDBTypes = GenericToolsTypeAnalysis.DynamicDBTypes<T>();
            Dictionary<string, Type> dynamicDBListTypes = GenericToolsTypeAnalysis.DynamicDBListTypes<T>();
            object[] res = new object[dynamicDBTypes.Count + dynamicDBListTypes.Count];
            var i = 0;
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (GenericToolsTypeAnalysis.DynamicDBTypes<T>().Keys.Contains(prop.Name))
                {
                    if (TypesForWhichTHasManyProperties.Contains(prop.PropertyType))
                    {
                        if (prop.GetValue(t) == null)
                        {
                            res[i] = new PropToNull(prop.Name);
                        }
                        else
                        {
                            res[i] = prop.GetValue(t);
                        }
                    }
                    else
                    {
                        res[i] = prop.GetValue(t);
                    }
                }
                else
                {
                    if (GenericToolsTypeAnalysis.DynamicDBListTypes<T>().Keys.Contains(prop.Name))
                    {
                        if (TypesForWhichTHasManyProperties.Contains(GenericToolsTypeAnalysis.DynamicDBListTypes<T>()[prop.Name]))
                        {
                            if (prop.GetValue(t) == null ||
                                (prop.GetValue(t) as IList).Count == 0)
                            {
                                res[i] = new PropToNull(prop.Name);
                            }
                            else
                            {
                                res[i] = prop.GetValue(t);
                            }
                        }
                        else
                        {
                            res[i] = prop.GetValue(t);
                        }
                    }
                    else
                    {
                        i--;
                    }
                }
                i++;
            }
            return res;
        }

        /// <summary>
        /// Prepares update for object <paramref name="t"/> of type <typeparamref name="T"/>. Every type <paramref name="q"/> in 
        /// <see cref="GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingRequiredTProperty"/> has to be updated
        /// manually. Indeed, required properties are not handled well in EF in case of relationships, especially if they are
        /// of type <see cref="IList"/> (an empty <see cref="IList"/> is not <see langword="null"/> and the annotation
        /// <see cref="RequiredAttribute"/> is interpreted as nullable = <see langword="false"/>).
        /// <br/>
        /// Creates the array of all the values of properties of the element <paramref name="t"/> of type <typeparamref name="T"/>
        /// to update that represent a relationship involving <typeparamref name="T"/>.
        /// <br/>
        /// Furthermore, for types appearing multiple times as properties in <typeparamref name="T"/>, set those to
        /// <see cref="PropToNull"/> if necessary. See <see cref="GenericRepository{T}.Save(T, object[])"/> for further details.
        /// </summary>
        /// <typeparam name="T">The type of the element to update</typeparam>
        /// <param name="t">The element to update</param>
        /// <returns>The array of all the values of properties of <paramref name="t"/> representing relationships involving <typeparamref name="T"/>,
        /// with values set to <see cref="PropToNull"/> if necessary.</returns>
        public static object[] PrepareUpdate<T>(DbContext context, T t)
        {
            foreach (Type type in GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingRequiredTProperty<T>())
            {
                Dictionary<string, string> propnames = GenericToolsTypeAnalysis.PropNamesForRelationWithTWithRequired(typeof(T), type);
                if (propnames.Count() != 0)
                {
                    for (int j = 0; j < propnames.Count(); j++)
                    {
                        GenericToolsCRUDPrep.UpdateOtherPropInRelationWithTHavingRequiredTProperty<T>(context,type, t, propnames.Keys.ToList()[j], propnames[propnames.Keys.ToList()[j]]);
                    }
                }
            }
            IEnumerable<Type> TypesForWhichTHasManyProperties = GenericToolsTypeAnalysis.GetTypesForWhichTHasManyProperties<T>();
            Dictionary<string, Type> dynamicDBTypes = GenericToolsTypeAnalysis.DynamicDBTypes<T>();
            Dictionary<string, Type> dynamicDBListTypes = GenericToolsTypeAnalysis.DynamicDBListTypes<T>();
            object[] res = new object[dynamicDBTypes.Count + dynamicDBListTypes.Count];
            var i = 0;
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (GenericToolsTypeAnalysis.DynamicDBTypes<T>().Keys.Contains(prop.Name))
                {
                    if (TypesForWhichTHasManyProperties.Contains(prop.PropertyType))
                    {
                        if (prop.GetValue(t) == null)
                        {
                            res[i] = new PropToNull(prop.Name);
                        }
                        else
                        {
                            res[i] = prop.GetValue(t);
                        }
                    }
                    else
                    {
                        res[i] = prop.GetValue(t);
                    }
                }
                else
                {
                    if (GenericToolsTypeAnalysis.DynamicDBListTypes<T>().Keys.Contains(prop.Name))
                    {
                        if (TypesForWhichTHasManyProperties.Contains(GenericToolsTypeAnalysis.DynamicDBListTypes<T>()[prop.Name]))
                        {
                            if (prop.GetValue(t) == null || 
                                (prop.GetValue(t) as IList).Count == 0)
                            {
                                res[i] = new PropToNull(prop.Name);
                            }
                            else
                            {
                                res[i] = prop.GetValue(t);
                            }
                        }
                        else
                        {
                            res[i] = prop.GetValue(t);
                        }
                    }
                    else
                    {
                        i--;
                    }
                }
                i++;
            }
            return res;
        }

        /// <summary>
        /// Prepares update for object <paramref name="t"/> of type <typeparamref name="T"/>. Every type <paramref name="q"/> in 
        /// <see cref="GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingRequiredTProperty"/> has to be updated
        /// manually. Indeed, required properties are not handled well in EF in case of relationships, especially if they are
        /// of type <see cref="IList"/> (an empty <see cref="IList"/> is not <see langword="null"/> and the annotation
        /// <see cref="RequiredAttribute"/> is interpreted as nullable = <see langword="false"/>).
        /// </summary>
        /// <remarks>
        /// Only the property of <paramref name="t"/> with name <paramref name="propertyName"/> will be updated.
        /// </remarks>
        /// <typeparam name="T">The type of the element to update</typeparam>
        /// <param name="t">The element to update</param>
        public static void PrepareUpdateOne<T>(DbContext context, T t, string propertyName)
        {
            foreach (Type type in GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingRequiredTProperty<T>())
            {
                Dictionary<string, string> propnames = GenericToolsTypeAnalysis.PropNamesForRelationWithTWithRequired(typeof(T), type);
                if (propnames.Count() != 0)
                {
                    for (int j = 0; j < propnames.Count(); j++)
                    {
                        if (propnames.Keys.ToList()[j] == propertyName)
                            GenericToolsCRUDPrep.UpdateOtherPropInRelationWithTHavingRequiredTProperty(context, type, t, propnames.Keys.ToList()[j], propnames[propnames.Keys.ToList()[j]]);
                    }
                }
            }
        }
    }
}