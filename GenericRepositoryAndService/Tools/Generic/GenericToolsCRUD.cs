using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GenericRepositoryAndService.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using GenericRepositoryAndService.Repository;

namespace GenericRepositoryAndService.Tools.Generic
{
    public abstract class GenericToolsCRUD
    {
        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUD.FindByKeysInNewContextForType(System.Type,System.Data.Entity.DbContext,System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUD.PrepareDelete``1(System.Data.Entity.DbContext,System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUD.PrepareSave``1(``0)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUD.PrepareUpdate``1(System.Data.Entity.DbContext,``0)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUD.PrepareUpdateOne``1(System.Data.Entity.DbContext,``0,System.String)"]/*'/>
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