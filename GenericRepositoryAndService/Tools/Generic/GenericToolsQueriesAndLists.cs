using GenericRepositoryAndService.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GenericRepositoryAndService.Exceptions;

namespace GenericRepositoryAndService.Tools.Generic
{
    public abstract class GenericToolsQueriesAndLists
    {
        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.QueryTInclude``1(System.Data.Entity.DbContext)"]/*'/>
        public static IQueryable<T> QueryTInclude<T>(DbContext dbContext) where T : class
        {
            IQueryable<T> req = dbContext.Set<T>().AsNoTracking()
                                                    .AsQueryable();
            foreach (string name in GenericToolsTypeAnalysis.DynamicDBListTypes<T>().Keys.ToList())
            {
                req = req.Include(name);
            }
            foreach (string name in GenericToolsTypeAnalysis.DynamicDBTypes<T>().Keys.ToList())
            {
                req = req.Include(name);
            }
            return req;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.QueryTIncludeTracked``1(System.Data.Entity.DbContext)"]/*'/>
        public static IQueryable<T> QueryTIncludeTracked<T>(DbContext dbContext) where T : class
        {
            IQueryable<T> req = dbContext.Set<T>().AsQueryable();
            foreach (string name in GenericToolsTypeAnalysis.DynamicDBListTypes<T>().Keys.ToList())
            {
                req = req.Include(name);
            }
            foreach (string name in GenericToolsTypeAnalysis.DynamicDBTypes<T>().Keys.ToList())
            {
                req = req.Include(name);
            }
            return req;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.QueryWhereKeysAre``1(System.Linq.IQueryable{``0},System.Object[])"]/*'/>
        public static IQueryable<T> QueryWhereKeysAre<T>(IQueryable<T> req, params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            if (typeof(T).IsSubclassOf(typeof(BaseEntity)))
            {
                int? id = GenericToolsTypeAnalysis.ObjectsToId<T>(objs);
                req = req.Where(
                               GenericToolsExpressionTrees.PropertyEquals<T>(
                                                                                typeof(T).GetProperty("Id"), 
                                                                                id
                                                                             )
                               );
            }
            else
            {
                int i = 0;
                foreach (object obj in objs)
                {
                    req = req.Where(
                                    GenericToolsExpressionTrees.PropertyEquals<T>(
                                                                                    typeof(T).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[i]), 
                                                                                    obj
                                                                                  )
                                    );
                    i++;
                }
            }
            return req;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.QueryTryPredicateWhere``1(System.Linq.IQueryable{``0},System.Linq.Expressions.Expression{System.Func{``0,System.Boolean}})"]/*'/>
        public static IQueryable<T> QueryTryPredicateWhere<T>(IQueryable<T> req, Expression<Func<T, bool>> predicateWhere = null)
        {
            if (predicateWhere == null)
                return req;
            IQueryable<T> req2;
            try
            {
                req2 = req.Where(predicateWhere);
                double test = req.Count(); //will throw exception if the predicate fails to be interpreted by LINQ to entities
            }
            catch
            {
                //Linq to entity cannot interpret the predicate predicateWhere
                //-> ignore the restriction "where"
                //-> TODO(?) : log the error ?
                req2 = req;
            }
            return req2;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.WhereSkipTake``1(System.Linq.IQueryable{``0},System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{``0,System.Boolean}})"]/*'/>
        public static IQueryable<T> WhereSkipTake<T>(IQueryable<T> orderedreq, int start, int maxByPage, Expression<Func<T, bool>> predicateWhere)
        {
            if (predicateWhere != null)
                orderedreq = QueryTryPredicateWhere(orderedreq, predicateWhere);

            orderedreq = orderedreq.Skip(start)
                                   .Take(maxByPage);
            return orderedreq;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.GetMethodGetKey``1(System.String)"]/*'/>
        private static MethodInfo GetMethodGetKey<T>(string propertyName)
        {
            return typeof(GenericToolsExpressionTrees).GetMethod(
                                                                    "GetKey", 
                                                                    BindingFlags.Public | BindingFlags.Static
                                                                 )
                                                      .MakeGenericMethod(new[] {
                                                                                typeof(T),
                                                                                typeof(T).GetProperty(propertyName).PropertyType
                                                                               });
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.GetMethodOrderBy``1(System.String)"]/*'/>
        private static MethodInfo GetMethodOrderBy<T>(string propertyName)
        {
            return typeof(Queryable).GetMethods().Single(m => 
                                                         m.Name == "OrderBy" &&
                                                         m.GetParameters().Length == 2
                                                         )
                                                  .MakeGenericMethod(new[] {
                                                                            typeof(T),
                                                                            typeof(T).GetProperty(propertyName).PropertyType
                                                                           }
                                                                     );
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.OrderByCustomKeyFromMethods``1(System.Linq.IQueryable{``0},System.Reflection.MethodInfo,System.Reflection.MethodInfo,System.String)"]/*'/>
        private static IQueryable<T> OrderByCustomKeyFromMethods<T>(IQueryable<T> req, MethodInfo methodOrderBy, MethodInfo methodGetKey, string propertyName)
        {
            return (IQueryable<T>)methodOrderBy.Invoke(
                                                        typeof(IQueryable<T>), 
                                                        new object[] {
                                                                        req,
                                                                        methodGetKey.Invoke(
                                                                                            typeof(GenericToolsExpressionTrees), 
                                                                                            new object[] { 
                                                                                                            typeof(T).GetProperty(propertyName)
                                                                                                          }
                                                                                            )
                                                                      }
                                                       );
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.QueryOrderByKey``1(System.Linq.IQueryable{``0},System.String)"]/*'/>
        private static IQueryable<T> QueryOrderByKey<T>(IQueryable<T> req, string defaultPropertyName)
        {
            MethodInfo methodGetKey = GetMethodGetKey<T>(defaultPropertyName);
            MethodInfo methodOrderBy = GetMethodOrderBy<T>(defaultPropertyName);

            return OrderByCustomKeyFromMethods(req, methodOrderBy, methodGetKey, defaultPropertyName);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.QueryDefaultOrderBy``1(System.Linq.IQueryable{``0})"]/*'/>
        public static IQueryable<T> QueryDefaultOrderBy<T>(IQueryable<T> req)
        {
            string defaultPropertyName;

            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                defaultPropertyName = "Id";
            }
            else
            {
                defaultPropertyName = GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[0];
            }

            return QueryOrderByKey(req, defaultPropertyName);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.ListWherePropNotNull``1(System.Collections.Generic.List{``0},System.String)"]/*'/>
        public static List<T> ListWherePropNotNull<T>(List<T> lst, string propname)
        {
            lst = lst.Where(
                            GenericToolsExpressionTrees.PropertyKeysNotNull<T>(typeof(T).GetProperty(propname)).Compile()
                            ).ToList();
            return lst;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.ListWherePropKeysAre``2(System.Collections.Generic.List{``1},System.String,System.Object[])"]/*'/>
        private static List<Q> ListWherePropKeysAre<T, Q>(List<Q> req, string propname, object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            if (typeof(T).IsSubclassOf(typeof(BaseEntity)))
            {
                int? id = GenericToolsTypeAnalysis.ObjectsToId<T>(objs);
                req = req.Where(
                               GenericToolsExpressionTrees.ExpressionPropPropEquals<Q>(
                                                                                        typeof(Q).GetProperty(propname), 
                                                                                        typeof(T).GetProperty("Id"), 
                                                                                        id
                                                                                       ).Compile()
                               ).ToList();
            }
            else
            {
                int i = 0;
                foreach (object obj in objs)
                {
                    req = req.Where(
                                    GenericToolsExpressionTrees.ExpressionPropPropEquals<Q>(
                                                                                                typeof(Q).GetProperty(propname), 
                                                                                                typeof(T).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[i]),
                                                                                                obj
                                                                                            ).Compile()
                                    ).ToList();
                    i++;
                }
            }
            return req;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.ListWhereKeysAreCountSup1``1(System.Collections.Generic.IList{``0},System.Object[])"]/*'/>
        public static bool ListWhereKeysAreCountSup1<T>(IList<T> req, params object[] objs)
        {
            Func<T, bool> func = GenericToolsExpressionTrees.ExpressionWhereKeysAre<T>(objs).Compile();
            return req.Where(func).Count() >= 1;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.ListWherePropListCountainsElementWithGivenKeys``2(System.Collections.Generic.List{``1},System.Type,System.String,System.Object[])"]/*'/>
        public static List<Q> ListWherePropListCountainsElementWithGivenKeys<T, Q>(List<Q> req, Type q, string propname, params object[] objs)
        {
            Func<Q, bool> func = GenericToolsExpressionTrees.ExpressionListWherePropListCountainsElementWithGivenKeys<T, Q>(q.GetProperty(propname), objs).Compile();
            var tempreq = req.Where(func);
            if (tempreq.Count() == 0)
                return new List<Q>();
            else
                return tempreq.ToList();
            //req.Where( t => t.propname.Where(...).Count()>=1)
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.ListWherePropListCountainsOnlyElementWithGivenKeys``2(System.Collections.Generic.List{``1},System.Type,System.String,System.Object[])"]/*'/>
        public static List<Q> ListWherePropListCountainsOnlyElementWithGivenKeys<T, Q>(List<Q> req, Type q, string propname, params object[] objs)
        {
            Func<Q, bool> func = GenericToolsExpressionTrees.ExpressionListWherePropListCountainsOnlyElementWithGivenKeys<T, Q>(q.GetProperty(propname), objs).Compile();
            var tempreq = req.Where(func);
            if (tempreq.Count() == 0)
                return new List<Q>();
            else
                return tempreq.ToList();
            //req.Where( t => t.propname.Where(...).Count()==1)
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.ListRemoveElementWithGivenKeys``1(System.Collections.Generic.List{``0},System.Object[])"]/*'/>
        public static List<T> ListRemoveElementWithGivenKeys<T>(List<T> req, params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            Func<T, bool> func;
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                int id = (int)objs[0];
                func = GenericToolsExpressionTrees.ExpressionListRemoveElementWithGivenId<T>(id).Compile();
            }
            else
            {
                func = GenericToolsExpressionTrees.ExpressionListRemoveElementWithGivenKeys<T>(objs).Compile();
            }
            return req.Where(func).ToList();
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.ListWhereMultiplePropKeysAre``2(System.Collections.Generic.List{``1},System.Collections.Generic.List{System.String},System.Object[])"]/*'/>
        public static List<Q> ListWhereMultiplePropKeysAre<T, Q>(List<Q> req, List<string> propnames, object[] objs)
        {
            foreach (string propname in propnames)
            {
                req = ListWherePropKeysAre<T, Q>(req, propname, objs);
            }
            return req;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsQueriesAndLists.ListWhereOtherTypePropListNotContains``2(System.Collections.Generic.List{``1},``0,System.String,System.Int32)"]/*'/>
        public static List<Q> ListWhereOtherTypePropListNotContains<T, Q>(List<Q> req, T newitem, string tpropname, int nbr = 1)
        {
            Func<Q, bool> func = GenericToolsExpressionTrees.ExpressionListWhereOtherTypePropListNotContains<T, Q>(newitem, tpropname, nbr).Compile();
            var tempreq = req.Where(func);
            if (tempreq.Count() == 0)
                return new List<Q>();
            else
                return tempreq.ToList();
            //req.Where( q => !newitem.tpropname.Where(qq => qq.Id == q.Id).Count() == 1 )
        }
    }
}