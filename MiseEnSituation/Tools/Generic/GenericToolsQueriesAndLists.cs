using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using MiseEnSituation.Exceptions;

namespace MiseEnSituation.Tools.Generic
{
    public abstract class GenericToolsQueriesAndLists
    {
        /// <summary>
        /// Setup all possible and necessary Include(propertyname) for <see cref="DbSet"/> queries.
        /// </summary>
        /// <param name="myDbContext">The context used for the query</param>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns>The query. Essentially, context.DbSetName.AsNoTracking().Include(...).Include(...)....Include(...)</returns>
        public static IQueryable<T> QueryTInclude<T>(MyDbContext myDbContext) where T : class
        {
            IQueryable<T> req = myDbContext.Set<T>().AsNoTracking()
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

        /// <summary>
        /// Setup all possible and necessary Include(propertyname) for <see cref="DbSet"/> queries.
        /// </summary>
        /// <param name="myDbContext">The context used for the query</param>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns>The query. Essentially, context.DbSetName.Include(...).Include(...)....Include(...)</returns>
        public static IQueryable<T> QueryTIncludeTracked<T>(MyDbContext myDbContext) where T : class
        {
            IQueryable<T> req = myDbContext.Set<T>().AsQueryable();
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

        /// <summary>
        /// From a query <paramref name="req"/>, specify either :
        /// <list type="bullet">
        /// <item>
        /// <c>req.Where(o => o.Id == Id)</c> if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>. In
        /// that case, <paramref name="objs"/> is the Id.
        /// </item>
        /// <item> 
        /// otherwise, <c>req.Where(o => o.Key1 = KeyValue1)....Where(o => o.Keyn = KeyValuen)</c>. In that case, 
        /// <paramref name="objs"/> is the array containing <c>{ KeyValue1, ..., KeyValuen }</c>
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type of the objects of the query</typeparam>
        /// <param name="req">The query</param>
        /// <param name="objs">Either the Id or the keys</param>
        /// <returns>The query specified</returns>
        /// <exception cref="InvalidKeyForClassException"/>
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

        /// <summary>
        /// From a query <paramref name="req"/>, specify that elements must satisfy a predicate <paramref name="predicateWhere"/>.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be interpreted from LINQ to SQL, the predicate will be ignored.
        /// </summary>
        /// <remarks>TODO(?) : log the error ? Throw custom exception ?</remarks>
        /// <typeparam name="T">The type of the objects of the query</typeparam>
        /// <param name="req">The query</param>
        /// <param name="predicateWhere">The predicate</param>
        /// <returns>The query specified</returns>
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

        /// <summary>
        /// Specify a query <paramref name="orderedreq"/> (ordered) to get elements following the condition <paramref name="predicateWhere"/> from element <paramref name="start"/> with at most <paramref name="maxByPage"/> elements
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be interpreted from LINQ to SQL, the predicate will be ignored.
        /// </summary>
        /// <param name="orderedreq">The ordered query to specify</param>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="predicateWhere">Conidition</param>
        /// <returns>The query</returns>
        public static IQueryable<T> WhereSkipTake<T>(IQueryable<T> orderedreq, int start, int maxByPage, Expression<Func<T, bool>> predicateWhere)
        {
            if (predicateWhere != null)
                orderedreq = QueryTryPredicateWhere(orderedreq, predicateWhere);

            orderedreq = orderedreq.Skip(start)
                                   .Take(maxByPage);
            return orderedreq;
        }

        /// <summary>
        /// Get the <see cref="MethodInfo"/> of <see cref="GenericToolsExpressionTrees.GetKey{TItem, TKey}(PropertyInfo)"/> where
        /// <c>TItem</c> is <typeparamref name="T"/> and <c>TKey</c> is the type of the property of <typeparamref name="T"/>
        /// having the name <paramref name="propertyName"/>.
        /// <br/>
        /// The purpose is to dynamically call the generic method <see cref="GenericToolsExpressionTrees.GetKey{TItem, TKey}(PropertyInfo)"/>.
        /// </summary>
        /// <typeparam name="T">The type investigated</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The method.</returns>
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

        /// <summary>
        /// Get the <see cref="MethodInfo"/> of <see cref="Queryable.OrderBy{TSource, TKey}(IQueryable{TSource}, Expression{Func{TSource, TKey}})"/>
        /// where <c>TSource</c> is <typeparamref name="T"/> and <c>TKey</c> is the type of the property of <typeparamref name="T"/>
        /// having the name <paramref name="propertyName"/>.
        /// </summary>
        /// <typeparam name="T">The type investigated</typeparam>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The method.</returns>
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

        /// <summary>
        /// From the methods <paramref name="methodOrderBy"/> and <paramref name="methodGetKey"/>, representing
        /// respectively the <see cref="MethodInfo"/> given by <see cref="GetMethodOrderBy{T}(string)"/> and
        /// <see cref="GetMethodGetKey{T}(string)"/>, specify the query <paramref name="req"/> appropriately.
        /// <br/>
        /// Intent : to specify the query : <c>req.OrderBy(o => o.</c><paramref name="propertyName"/><c>)</c>
        /// dynamically, so that LINQ to SQL interpretation will be successfull.
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="req">The query to specify</param>
        /// <param name="methodOrderBy">The method OrderBy</param>
        /// <param name="methodGetKey">The method GetKey</param>
        /// <param name="propertyName">The name of the property</param>
        /// <returns>The specified query</returns>
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

        /// <summary>
        /// Specify the query <paramref name="req"/> ordering elements by <paramref name="defaultPropertyName"/>, 
        /// essentially do <c>req.OrderBy(o => o.</c><paramref name="defaultPropertyName"/><c>)</c> dynamically.
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="req">The query to specify</param>
        /// <param name="defaultPropertyName">The name of the property</param>
        /// <returns>The specified query</returns>
        private static IQueryable<T> QueryOrderByKey<T>(IQueryable<T> req, string defaultPropertyName)
        {
            MethodInfo methodGetKey = GetMethodGetKey<T>(defaultPropertyName);
            MethodInfo methodOrderBy = GetMethodOrderBy<T>(defaultPropertyName);

            return OrderByCustomKeyFromMethods(req, methodOrderBy, methodGetKey, defaultPropertyName);
        }

        /// <summary>
        /// Specifies a query <paramref name="req"/> with a default ordering, ordering by either :
        /// <list type="bullet">
        /// <item>
        /// Id if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// otherwise, the first key (the first declared in the class <typeparamref name="T"/>).
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type investigated</typeparam>
        /// <param name="req">The query to specify</param>
        /// <returns>The specified query.</returns>
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

        /// <summary>
        /// From a list <paramref name="lst"/> of elements of type <typeparamref name="T"/>, get all the elements for which
        /// the property <paramref name="propname"/> is not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements</typeparam>
        /// <param name="lst">The input list</param>
        /// <param name="propname">The name of the property</param>
        /// <returns>The list restricted to elements for which the property <paramref name="propname"/> is not <see langword="null"/></returns>
        public static List<T> ListWherePropNotNull<T>(List<T> lst, string propname)
        {
            lst = lst.Where(
                            GenericToolsExpressionTrees.PropertyKeysNotNull<T>(typeof(T).GetProperty(propname)).Compile()
                            ).ToList();
            return lst;
        }

        /// <summary>
        /// From a list <paramref name="req"/> of elements of class <typeparamref name="Q"/> having a property of type <typeparamref name="T"/>
        /// with name <paramref name="propname"/>, get the elements for which this property's Id or Keys are given.
        /// <br/>
        /// Essentially, does <c>req.Where(q => q.propname.Id == id)</c> or <c>req.Where(q => q.propname.Key1 == keyValue1 &amp;&amp; ... &amp;&amp; q.propname.Keyn == keyValuen)</c>
        /// </summary>
        /// <remarks>
        /// Assumes q.propname is not <see langword="null"/> (otherwise q.propname.something would throw exception)</remarks>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <typeparam name="Q">The type of the list's elements</typeparam>
        /// <param name="req">The initial list</param>
        /// <param name="propname">The property</param>
        /// <param name="objs">Either the Id or the keys</param>
        /// <returns>The list specified</returns>
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

        /// <summary>
        /// From a list <paramref name="req"/> of elements of type <typeparamref name="T"/>, 
        /// return <see langword="true"/> if the list contains an element with either Id or keys <paramref name="objs"/>,
        /// and return <see langword="false"/> otherwise.
        /// </summary>
        /// <typeparam name="T">The type of elements</typeparam>
        /// <param name="req">The initial list</param>
        /// <param name="objs">Either the Id or the keys.</param>
        /// <returns>A boolean indicating whether or not the list <paramref name="req"/> contains an element
        /// with either Id or keys <paramref name="objs"/></returns>
        public static bool ListWhereKeysAreCountSup1<T>(IList<T> req, params object[] objs)
        {
            Func<T, bool> func = GenericToolsExpressionTrees.ExpressionWhereKeysAre<T>(objs).Compile();
            return req.Where(func).Count() >= 1;
        }

        /// <summary>
        /// From a list <paramref name="req"/> of elements <typeparamref name="Q"/>, get specific elements according to
        /// the predicate <see cref="GenericToolsExpressionTrees.ExpressionListWherePropListCountainsElementWithGivenKeys{T, Q}(PropertyInfo, object[])"/>
        /// applied to the property of <typeparamref name="Q"/> with name <paramref name="propname"/> and either the Id
        /// or the keys given by <paramref name="objs"/>. 
        /// <br/>
        /// Ie the elements of type <typeparamref name="Q"/> have a property with name <paramref name="propname"/>
        /// which is a list of elements of type <typeparamref name="T"/>. 
        /// <br/>
        /// This returns essentially : <c>req.Where(q => q.prop.Where(t => t.keysorId == objs).Count() >= 1)</c>
        /// <br/>
        /// That is to say the list of elements of type <typeparamref name="Q"/> so that their property with name <paramref name="propname"/> is
        /// a <see cref="IList{T}"/> which contains an element with either Id or Key given by <paramref name="objs"/>.
        /// </summary>
        /// <remarks>Note that <paramref name="q"/> must be the same as <typeparamref name="Q"/>.</remarks>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <typeparam name="Q">The type of the elements in the initial list <paramref name="req"/></typeparam>
        /// <param name="req">The initial list</param>
        /// <param name="q">The type of the elements in <paramref name="req"/></param>
        /// <param name="propname">The name of the property of <typeparamref name="Q"/> which is of type <typeparamref name="T"/></param>
        /// <param name="objs">Either the Id or keys</param>
        /// <returns>The restricted list.</returns>
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

        /// <summary>
        /// From a list <paramref name="req"/> of elements <typeparamref name="Q"/>, get specific elements according to
        /// the predicate <see cref="GenericToolsExpressionTrees.ExpressionListWherePropListCountainsOnlyElementWithGivenKeys{T, Q}(PropertyInfo, object[])"/>
        /// applied to the property of <typeparamref name="Q"/> with name <paramref name="propname"/> and either the Id
        /// or the keys given by <paramref name="objs"/>. 
        /// <br/>
        /// Ie the elements of type <typeparamref name="Q"/> have a property with name <paramref name="propname"/>
        /// which is a list of elements of type <typeparamref name="T"/>. 
        /// <br/>
        /// This returns essentially : <c>req.Where(q => q.prop.Where(t => t.keysorId == objs).Count() == 1 &amp;&amp; q.prop.Count() == 1)</c>
        /// <br/>
        /// That is to say the list of elements of type <typeparamref name="Q"/> so that their property with name <paramref name="propname"/> is
        /// a <see cref="IList{T}"/> which contains only an element with either Id or Key given by <paramref name="objs"/>.
        /// </summary>
        /// <remarks>Note that <paramref name="q"/> must be the same as <typeparamref name="Q"/>.</remarks>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <typeparam name="Q">The type of the elements in the initial list <paramref name="req"/></typeparam>
        /// <param name="req">The initial list</param>
        /// <param name="q">The type of the elements in <paramref name="req"/></param>
        /// <param name="propname">The name of the property of <typeparamref name="Q"/> which is of type <typeparamref name="T"/></param>
        /// <param name="objs">Either the Id or keys</param>
        /// <returns>The restricted list.</returns>
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

        /// <summary>
        /// From a list of elements of type <typeparamref name="T"/> and either a given Id or given keys
        /// <paramref name="objs"/>, get all the elements that does not either have the Id or one of the given keys.
        /// <br/>
        /// Essentially, do <c>req => req.Where(t => t.Id != id)</c> or
        /// <c>req => req.Where(t => t.key1 != key1value || ... || t.keyn != keynvalue</c>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="req">The initial list</param>
        /// <param name="objs">Either the Id or the keys</param>
        /// <returns>The restricted list</returns>
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

        /// <summary>
        /// From a list <paramref name="req"/> of elements of class <typeparamref name="Q"/> having mutiple properties
        /// of type <typeparamref name="T"/> with names <paramref name="propnames"/>, get the elements for which these
        /// property's Id or Keys are given.
        /// <br/>
        /// Essentially, does <c>req.Where(q => q.propname1.Id == id &amp;&amp; ... &amp;&amp; q.propnamen.Id == id)</c>
        /// or <c>req.Where(q => q.propname1.Key1 == key1Value &amp;&amp; ... &amp;&amp; q.propname1.Keym == keymValue
        /// &amp;&amp; ... &amp;&amp;  q.propnamen.Key1 == key1Value &amp;&amp; ... &amp;&amp; q.propnamen.Keym == keymValue)</c>
        /// </summary>
        /// <remarks>
        /// Assumes q.propnames are not <see langword="null"/> (otherwise q.propname.something would throw exception)</remarks>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <typeparam name="Q">The type of the list's elements</typeparam>
        /// <param name="req">The initial list</param>
        /// <param name="propnames">The property</param>
        /// <param name="objs">Either the Id or the keys</param>
        /// <returns>The restricted list</returns>
        public static List<Q> ListWhereMultiplePropKeysAre<T, Q>(List<Q> req, List<string> propnames, object[] objs)
        {
            foreach (string propname in propnames)
            {
                req = ListWherePropKeysAre<T, Q>(req, propname, objs);
            }
            return req;
        }

        /// <summary>
        /// Get an expression tree having principal root of type <typeparamref name="Q"/>, for testing whether or not an element
        /// <paramref name="newitem"/> of type <typeparamref name="T"/> with property <paramref name="tpropname"/> of type <see cref="IList{Q}"/>
        /// does not contain the element tested.
        /// <br/>
        /// Essentially, does <c>req => req.Where(q => !newitem.tpropname.Where(qq => qq.Id == q.Id).Count() == 1))</c>
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="newitem"/></typeparam>
        /// <typeparam name="Q">The type of the elements invistigated</typeparam>
        /// <param name="req">The list</param>
        /// <param name="newitem">The object</param>
        /// <param name="tpropname">The name of the property of <typeparamref name="T"/> in question</param>
        /// <param name="nbr">The number of keys for objects of type <typeparamref name="Q"/> if appropriate</param>
        /// <returns>The restricted list</returns>
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