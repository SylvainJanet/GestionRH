using MiseEnSituation.Exceptions;
using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace MiseEnSituation.Tools.Generic
{
    public abstract class GenericTools
    {
        /// <summary>
        /// Test if a type implements <see cref="IList{}"/> of <typeparamref name="T"/>, and if so, determine <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// Thanks internet for this one.
        /// </remarks>
        /// <param name="type">Type to test : check if it is <see cref="IList{T}"/></param>
        /// <param name="innerType">If <paramref name="type"/> is <see cref="IList{T}"/>, returns the inner type <typeparamref name="T"/></param>
        /// <returns>A boolean representing if <paramref name="type"/> is of type <see cref="IList{T}"/></returns>
        public static bool TryListOfWhat(Type type, out Type innerType)
        {
            Contract.Requires(type != null);

            var interfaceTest = new Func<Type, Type>(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>) ? i.GetGenericArguments().Single() : null);

            innerType = interfaceTest(type);
            if (innerType != null)
            {
                return true;
            }

            foreach (var i in type.GetInterfaces())
            {
                innerType = interfaceTest(i);
                if (innerType != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the names of the properties of <typeparamref name="T"/> that have the annotation 
        /// <see cref="KeyAttribute"/>. The order will be the same as the order in the declaration of <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// If <typeparamref name="T"/> derives from <see cref="BaseEntity"/>, returns <see langword="null"/>.
        /// </remarks>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns>The properties names that are keys, in the same order as in the declaration of <typeparamref name="T"/>. If
        /// <typeparamref name="T"/> derives from <see cref="BaseEntity"/>, returns <see langword="null"/>.</returns>
        private static string[] KeyPropertiesNames<T>()
        {
            if (typeof(T).IsSubclassOf(typeof(BaseEntity)))
                return null;
            return typeof(T).GetProperties().Where(p => p.GetCustomAttribute(typeof(KeyAttribute), false) != null)
                                            .Select(p => p.Name)
                                            .ToArray();
        }

        /// <summary>
        /// Tells if <typeparamref name="T"/> is in a relationship with any other class in DB.
        /// </summary>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns><see langword="true"/> if <typeparamref name="T"/> is in any relationship, <see langword="false"/> otherwise.</returns>
        public static bool HasDynamicDBTypeOrListType<T>()
        {
            return DynamicDBListTypes<T>().Count != 0 || DynamicDBTypes<T>().Count != 0;
        }

        /// <summary>
        /// An object <c>obj</c> of class <typeparamref name="T"/> has properties <c>obj.PropName</c> of
        /// class <see cref="IList"/>&lt;<c>ClassType</c>&gt; where <c>ClassType</c> is in a <see cref="DbSet"/> of the generic repository 
        /// <see cref="DataContext"/>. 
        /// <br/>
        /// This is every { PropName : ClassType }
        /// </summary>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns>The dictionary</returns>
        public static Dictionary<string, Type> DynamicDBListTypes<T>()
        {
            return DynamicDBListTypesForType(typeof(T));
        }

        /// <summary>
        /// An object <c>obj</c> of class <typeparamref name="T"/> has properties <c>obj.PropName</c> of
        /// class <c>ClassType</c> which is in a <see cref="DbSet"/> of the generic repository 
        /// <see cref="DataContext"/>. 
        /// <br/>
        /// This is every { PropName : ClassType }
        /// </summary>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns>The dictionary</returns>
        public static Dictionary<string, Type> DynamicDBTypes<T>()
        {
            return DynamicDBTypesForType(typeof(T));
        }

        /// <summary>
        /// An object <c>obj</c> of class <typeparamref name="T"/> has properties <c>obj.PropName</c> of
        /// class <see cref="IList"/>&lt;<c>ClassType</c>&gt; where <c>ClassType</c> is in a <see cref="DbSet"/> of the generic repository 
        /// <see cref="DataContext"/>. 
        /// <br/>
        /// This is every { PropName : ClassType }
        /// </summary>
        /// <param name="t">The type invistigated.</param>
        /// <returns>The dictionary</returns>
        private static Dictionary<string, Type> DynamicDBListTypesForType(Type t)
        {
            Dictionary<string, Type> res = new Dictionary<string, Type>();
            foreach (PropertyInfo property in t.GetProperties())
            {
                if (TryListOfWhat(property.PropertyType, out Type innerType))
                {
                    if (innerType.IsSubclassOf(typeof(BaseEntity)) ||
                        innerType.GetProperties().Where(p => p.GetCustomAttribute(typeof(KeyAttribute), false) != null)
                                                 .Count() > 0)
                    {
                        res.Add(property.Name, innerType);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// An object <c>obj</c> of class <typeparamref name="T"/> has properties <c>obj.PropName</c> of
        /// class <c>ClassType</c> which is in a <see cref="DbSet"/> of the generic repository 
        /// <see cref="DataContext"/>. 
        /// <br/>
        /// This is every { PropName : ClassType }
        /// </summary>
        /// <param name="t">The type invistigated.</param>
        /// <returns>The dictionary</returns>
        private static Dictionary<string, Type> DynamicDBTypesForType(Type t)
        {
            Dictionary<string, Type> res = new Dictionary<string, Type>();
            foreach (PropertyInfo property in t.GetProperties())
            {
                if (property.PropertyType.IsSubclassOf(typeof(BaseEntity)) ||
                    property.PropertyType.GetProperties().Where(p => p.GetCustomAttribute(typeof(KeyAttribute), false) != null)
                                                         .Count() > 0)
                {
                    res.Add(property.Name, property.PropertyType);
                }
            }
            return res;
        }

        /// <summary>
        /// Setup all possible and necessary Include(propertyname) for <see cref="DbSet"/> queries.
        /// </summary>
        /// <param name="myDbContext">The context used for the query</param>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns>The query. Essentially, context.DbSetName.AsNoTracking().Include(...).Include(...)....Include(...)</returns>
        public static IQueryable<T> QueryTInclude<T>(MyDbContext myDbContext) where T : class
        {
            IQueryable<T> req = myDbContext.Set<T>().AsNoTracking().AsQueryable();
            foreach (string name in DynamicDBListTypes<T>().Keys.ToList())
            {
                req = req.Include(name);
            }
            foreach (string name in DynamicDBTypes<T>().Keys.ToList())
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
            foreach (string name in DynamicDBListTypes<T>().Keys.ToList())
            {
                req = req.Include(name);
            }
            foreach (string name in DynamicDBTypes<T>().Keys.ToList())
            {
                req = req.Include(name);
            }
            return req;
        }

        /// <summary>
        /// Checks if <paramref name="objs"/> is either
        /// <list type="bullet">
        /// <item>
        /// an integer, the Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// an array (of proper length) of objects (of proper types) corresponding to the keys of <typeparamref name="T"/>.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <param name="objs">The array of object tested.</param>
        /// <exception cref="InvalidKeyForClassException"/>
        public static void CheckIfObjectIsKey<T>(object[] objs)
        {
            if (typeof(T).IsSubclassOf(typeof(BaseEntity)))
            {
                if (objs.Length != 1 || !(objs[0] is int))
                    throw new InvalidKeyForClassException(typeof(T));
            }
            else
            {
                if (objs.Length != KeyPropertiesNames<T>().Length)
                    throw new InvalidKeyForClassException(typeof(T));
                int len = KeyPropertiesNames<T>().Length;
                for (int i = 0; i < len; i++)
                {
                    object ototest = objs[i];
                    string propname = KeyPropertiesNames<T>()[i];
                    if (!typeof(T).GetProperty(propname).PropertyType.IsAssignableFrom(ototest.GetType()))
                        throw new InvalidKeyForClassException(typeof(T));
                }
            }
        }

        /// <summary>
        /// From an array of objects, supposed to be either Id or keys, get the Id
        /// </summary>
        /// <typeparam name="T">The type investifated</typeparam>
        /// <param name="objs">The objects to cast to int as an Id</param>
        /// <returns>The Id</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        private static int? ObjectsToId<T>(object[] objs)
        {
            if (!typeof(T).IsSubclassOf(typeof(BaseEntity)))
                throw new InvalidKeyForClassException(typeof(T));
            if ((objs.Length != 1) || !(objs[0] is int))
                throw new InvalidKeyForClassException(typeof(T));
            return (int?)objs[0];
        }

        /// <summary>
        /// Construct the expression tree for a lambda expression (<c>o => o.prop == value</c>) dynamically
        /// <br/>
        /// Purpose : LINQ to SQL interpretation will be successfull. 
        /// Something using reflection such as 
        /// <br/>
        /// <c>o => o.GetType().GetProperty("prop").GetValue(o)==value</c>
        /// <br/>
        /// doesn't work : LINQ to ENTITY won't work.
        /// <br/>
        /// The object is of type <typeparamref name="TItem"/>, value is of type <typeparamref name="TValue"/>, 
        /// the property to check is <paramref name="property"/> and the value is <paramref name="value"/>.
        /// </summary>
        /// <remarks>
        /// Thanks internet for this one (had to modify it a little bit though)
        /// </remarks>
        /// <typeparam name="TItem">The type of the object the lambda expression will test whether or not the 
        /// property <paramref name="property"/> is equal to <paramref name="value"/></typeparam>
        /// <typeparam name="TValue">Tje type of <paramref name="value"/></typeparam>
        /// <param name="property">The property</param>
        /// <param name="value">The value</param>
        /// <returns>The lambda expression properly constructed so that LINQ to SQL interpretation will be
        /// successfull</returns>
        private static Expression<Func<TItem, bool>> PropertyEquals<TItem>(PropertyInfo property, object value)
        {
            var param = Expression.Parameter(typeof(TItem));
            var body = Expression.Equal(Expression.Property(param, property),
                                        Expression.Constant(value, property.PropertyType));
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        /// <summary>
        /// Construct the expression tree for a lambda expression (<c>o => o.prop</c>) dynamically
        /// <br/>
        /// Purpose : LINQ to SQL interpretation will be successfull. 
        /// Something using reflection such as 
        /// <br/>
        /// <c>o => o.GetType().GetProperty("prop").GetValue(o)</c>
        /// <br/>
        /// doesn't work : LINQ to ENTITY won't work.
        /// <br/>
        /// The object is of type <typeparamref name="TItem"/>, 
        /// the property to specify is <paramref name="property"/> and is of type <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of the object the lambda expression will specify the property
        /// <paramref name="property"/>.</typeparam>
        /// <typeparam name="TKey">The type of the property <paramref name="property"/></typeparam>
        /// <param name="property">The property to specify</param>
        /// <returns>The lambda expression properly constructed so that LINQ to SQL interpretation will be
        /// successfull</returns>
        private static Expression<Func<TItem, TKey>> GetKey<TItem, TKey>(PropertyInfo property)
        {
            var param = Expression.Parameter(typeof(TItem));
            var body = Expression.Property(param, property);
            var bodyconverted = Expression.Convert(body, typeof(TKey));
            return Expression.Lambda<Func<TItem, TKey>>(bodyconverted, param);
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
            CheckIfObjectIsKey<T>(objs);
            if (typeof(T).IsSubclassOf(typeof(BaseEntity)))
            {
                int? id = ObjectsToId<T>(objs);
                req = req.Where(
                               PropertyEquals<T>(typeof(T).GetProperty("Id"), id)
                               );
            }
            else
            {
                int i = 0;
                foreach (object obj in objs)
                {
                    req = req.Where(
                                    PropertyEquals<T>(typeof(T).GetProperty(KeyPropertiesNames<T>()[i]), obj)
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
                double test = req.Count();
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

            orderedreq = orderedreq.Skip(start).Take(maxByPage);
            return orderedreq;
        }

        /// <summary>
        /// Get the <see cref="MethodInfo"/> of <see cref="GetKey{TItem, TKey}(PropertyInfo)"/> where
        /// <c>TItem</c> is <typeparamref name="T"/> and <c>TKey</c> is the type of the property of <typeparamref name="T"/>
        /// having the name <paramref name="propertyName"/>.
        /// <br/>
        /// The purpose is to dynamically call the generic method <see cref="GetKey{TItem, TKey}(PropertyInfo)"/>.
        /// </summary>
        /// <typeparam name="T">The type investigated</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The method.</returns>
        private static MethodInfo GetMethodGetKey<T>(string propertyName)
        {
            return typeof(GenericTools).GetMethod("GetKey", BindingFlags.NonPublic | BindingFlags.Static)
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
            return typeof(Queryable).GetMethods().Single(m => m.Name == "OrderBy" &&
                                                              m.GetParameters().Length == 2)
                                                  .MakeGenericMethod(new[] {
                                                                            typeof(T),
                                                                            typeof(T).GetProperty(propertyName).PropertyType
                                                                           });
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
            return (IQueryable<T>)methodOrderBy.Invoke(typeof(IQueryable<T>), new object[] {
                                                                                            req,
                                                                                            methodGetKey.Invoke(typeof(GenericTools), new object[] { typeof(T).GetProperty(propertyName) })
                                                                                           });
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
                defaultPropertyName = KeyPropertiesNames<T>()[0];
            }

            return QueryOrderByKey(req, defaultPropertyName);
        }

        /// <summary>
        /// From an object <paramref name="t"/> of type <typeparamref name="T"/>, get either
        /// <list type="bullet">
        /// <item>
        /// the id of <paramref name="t"/> if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// otherwise the array of keys of <paramref name="t"/>
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="t">The object</param>
        /// <returns>Either the Id of <paramref name="t"/> or its keys values</returns>
        public static object[] GetKeysValues<T>(T t)
        {
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                return new object[] { ((int?)typeof(T).GetProperty("Id").GetValue(t)).Value };
            }
            else
            {
                object[] res = new object[KeyPropertiesNames<T>().Length];
                int i = 0;
                foreach (string propname in KeyPropertiesNames<T>())
                {
                    object value = typeof(T).GetProperty(propname).GetValue(t);
                    res[i] = Convert.ChangeType(value, typeof(T).GetProperty(propname).PropertyType);
                    i++;
                }
                return res;
            }
        }

        /// <summary>
        /// Call the generic method <see cref="GetKeysValues{T}(T)"/> dynamically. 
        /// <br/>
        /// That is, get either the Id or the keys values
        /// of an object <paramref name="item"/> of type <paramref name="typeofElement"/>.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="typeofElement">The type of the object</param>
        /// <returns>Either the Id or the keys values of <paramref name="item"/></returns>
        public static object[] GetKeysValuesForType(object item, Type typeofElement)
        {
            MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                 .MakeGenericMethod(new[] { typeofElement });
            return (object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { item });
        }

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
        public static object FindByKeysInNewContextForType(Type typeofElement, MyDbContext newContext, object[] objs)
        {
            MethodInfo methodCheckIfObjectIsKey = typeof(GenericTools).GetMethod("CheckIfObjectIsKey", BindingFlags.Public | BindingFlags.Static)
                                                                      .MakeGenericMethod(new[] { typeofElement });
            methodCheckIfObjectIsKey.Invoke(typeof(GenericTools), new object[] { objs });
            MethodInfo methodQueryWhereKeysAre = typeof(GenericTools).GetMethod("QueryWhereKeysAre", BindingFlags.Public | BindingFlags.Static)
                                                                     .MakeGenericMethod(new[] { typeofElement });
            object query = methodQueryWhereKeysAre.Invoke(typeof(GenericTools), new object[] { newContext.Set(typeofElement), objs });
            MethodInfo methodSingleOrDefault = typeof(Queryable).GetMethods().Single(m => m.Name == "SingleOrDefault" &&
                                                                                          m.GetParameters().Length == 1)
                                                                .MakeGenericMethod(new[] { typeofElement });
            return methodSingleOrDefault.Invoke(typeof(Queryable), new object[] { query });
        }

        /// <summary>
        /// Assuming <typeparamref name="T"/> does not derive from <see cref="BaseEntity"/>, check
        /// if <paramref name="objs"/> is an array of keys.
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="objs">The array that must be many keys values.</param>
        /// <exception cref="InvalidKeyForClassException"/>
        private static void CheckIfObjsIsManyKeys<T>(object[] objs)
        {
            int nbrkeys = KeyPropertiesNames<T>().Length;
            if (objs.Length == 0 || objs.Length % nbrkeys != 0)
                throw new InvalidKeyForClassException(typeof(T));
            int nbrobjs = objs.Length / nbrkeys;
            for (int i = 0; i < nbrobjs; i++)
            {
                object[] keystotest = new object[nbrkeys];
                for (int j = 0; j < nbrkeys; j++)
                {
                    keystotest[j] = objs[i * nbrkeys + j];
                }
                CheckIfObjectIsKey<T>(objs);
            }
        }

        /// <summary>
        /// From a one dimensionnal array of keys values <paramref name="objs"/>, get an array
        /// of array containing key values.
        /// <br/>
        /// <example>
        /// From <c>{ key1value1, key2value1, key1value2, key2value2 }</c> get
        /// <c>{ { key1value1, key2value1 } , { key1value2, key2value2 } }</c>
        /// </example>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="objs">The array of keys values</param>
        /// <returns>The array of array containing key values.</returns>
        public static object[][] GetManyKeys<T>(object[] objs)
        {
            int nbrkeys = KeyPropertiesNames<T>().Length;
            int nbrobjs = objs.Length / nbrkeys;
            object[][] res = new object[nbrobjs][];
            for (int i = 0; i < nbrobjs; i++)
            {
                res[i] = new object[nbrkeys];
                for (int j = 0; j < nbrkeys; j++)
                {
                    res[i][j] = objs[i * nbrkeys + j];
                }
            }
            return res;
        }

        /// <summary>
        /// Checks if <paramref name="objs"/> is either
        /// <list type="bullet">
        /// <item>
        /// many Ids if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or many keys values.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="objs">The array that must either be many Ids or manys keys values.</param>
        /// <exception cref="IdListEmptyForClassException"/>
        /// <exception cref="InvalidKeyForClassException" />
        public static void CheckIfObjsIsManyKeysOrIds<T>(params object[] objs)
        {
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                if (objs.Length == 0)
                    throw new IdListEmptyForClassException(typeof(T));
                for (int i = 0; i < objs.Length; i++)
                {
                    if (!typeof(int?).IsAssignableFrom(objs[i].GetType()) && !typeof(int?).IsAssignableFrom(objs[i].GetType()) && !int.TryParse(objs[i].ToString(), out _))
                        throw new InvalidKeyForClassException(typeof(T));
                }
            }
            else
            {
                CheckIfObjsIsManyKeys<T>(objs);
            }
        }

        /// <summary>
        /// From an array <paramref name="objs"/> of objects containing many Ids,
        /// get an array <see cref="int"/>?[] containing the Ids.
        /// </summary>
        /// <param name="objs">The array to convert.</param>
        /// <returns>The array of Ids.</returns>
        public static int?[] GetManyIds(params object[] objs)
        {
            int?[] ids = new int?[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                try
                {
                    ids[i] = (int?)objs[i];
                }
                catch
                {
                    int.TryParse((string)objs[i], out int j);
                    ids[i] = (int?)j;
                }
            }
            return ids;
        }

        /// <summary>
        /// Build the expression tree for <c>o => (o.prop1.prop2 == value)</c> where o is of type <typeparamref name="TItem"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of item at the root of the expression tree</typeparam>
        /// <param name="prop1">The first property to access</param>
        /// <param name="prop2">The second property to access</param>
        /// <param name="value">The value</param>
        /// <returns>The expression tree</returns>
        private static Expression<Func<TItem, bool>> ExpressionPropPropEquals<TItem>(PropertyInfo prop1, PropertyInfo prop2, object value)
        {
            var param = Expression.Parameter(typeof(TItem));
            var exp = Expression.Property(param, prop1);
            var body = Expression.Equal(Expression.Property(exp, prop2),
                                        Expression.Constant(value, prop2.PropertyType));
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        /// <summary>
        /// Build the expression tree for <c>o => o.prop != null</c> where o is of type <typeparamref name="TItem"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of item at the root of the expression tree</typeparam>
        /// <param name="prop">The property to access</param>
        /// <returns>The expression tree</returns>
        private static Expression<Func<TItem, bool>> PropertyKeysNotNull<TItem>(PropertyInfo prop)
        {
            var param = Expression.Parameter(typeof(TItem));
            var exp = Expression.Property(param, prop);
            var body = Expression.IsFalse(Expression.Equal(exp, Expression.Constant(null)));
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        /// <summary>
        /// From a list <paramref name="lst"/> of elements of type <typeparamref name="T"/>, get all the elements for which
        /// the property <paramref name="propname"/> is not <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements</typeparam>
        /// <param name="lst">The input list</param>
        /// <param name="propname">The name of the property</param>
        /// <returns>The list restricted to elements for which the property <paramref name="propname"/> is not <see langword="null"/></returns>
        private static List<T> ListWherePropNotNull<T>(List<T> lst, string propname)
        {
            lst = lst.Where(
                            PropertyKeysNotNull<T>(typeof(T).GetProperty(propname)).Compile()
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
            CheckIfObjectIsKey<T>(objs);
            if (typeof(T).IsSubclassOf(typeof(BaseEntity)))
            {
                int? id = ObjectsToId<T>(objs);
                req = req.Where(
                               ExpressionPropPropEquals<Q>(typeof(Q).GetProperty(propname), typeof(T).GetProperty("Id"), id).Compile()
                               ).ToList();
            }
            else
            {
                int i = 0;
                foreach (object obj in objs)
                {
                    req = req.Where(
                                    ExpressionPropPropEquals<Q>(typeof(Q).GetProperty(propname), typeof(T).GetProperty(KeyPropertiesNames<T>()[i]), obj).Compile()
                                    ).ToList();
                    i++;
                }
            }
            return req;
        }

        /// <summary>
        /// Get an expression tree with principal root of type <typeparamref name="T"/>, checking whether or not
        /// the element has either Id or keys equal to <paramref name="objs"/>.
        /// <br/>
        /// Essentially, does either <c>t => t.Id == id</c> or <c>t => t.key1 == key1value &amp;&amp; ... &amp;&amp; t.keyn = keynvalue</c>
        /// </summary>
        /// <typeparam name="T">The type in question</typeparam>
        /// <param name="objs">Either the Id or the keys</param>
        /// <returns>The expression tree</returns>
        private static Expression<Func<T, bool>> ExpressionWhereKeysAre<T>(params object[] objs)
        {
            var param = Expression.Parameter(typeof(T));
            Expression body;
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                int id = (int)objs[0];
                body = Expression.Equal(Expression.Property(param, typeof(T).GetProperty("Id")),
                                        Expression.Constant(id, typeof(T).GetProperty("Id").PropertyType));
            }
            else
            {
                body = Expression.Equal(Expression.Property(param, typeof(T).GetProperty(KeyPropertiesNames<T>()[0])),
                                        Expression.Constant(objs[0], typeof(T).GetProperty(KeyPropertiesNames<T>()[0]).PropertyType));
                for (int i = 1; i < objs.Length; i++)
                {
                    body = Expression.And(body,
                                          Expression.Equal(Expression.Property(param, typeof(T).GetProperty(KeyPropertiesNames<T>()[i])),
                                                           Expression.Constant(objs[i], typeof(T).GetProperty(KeyPropertiesNames<T>()[i]).PropertyType)));
                }
            }
            return Expression.Lambda<Func<T, bool>>(body, param);
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
        private static bool ListWhereKeysAreCountSup1<T>(IList<T> req, params object[] objs)
        {
            Func<T, bool> func = ExpressionWhereKeysAre<T>(objs).Compile();
            return req.Where(func).Count() >= 1;
        }

        /// <summary>
        /// Get an expression tree having root of type <typeparamref name="Q"/>. These elements have a property <paramref name="prop"/>
        /// which is a list of elements of type <typeparamref name="T"/>. 
        /// <br/>
        /// This returns essentially : <c>q => q.prop.Where(t => t.keysorId == objs).Count() >= 1</c>
        /// <br/>
        /// That is to say, for an element of type <typeparamref name="Q"/> so that their property <paramref name="prop"/> is
        /// a <see cref="IList{T}"/>, whether or not it contains an element with either Id or Key given by <paramref name="objs"/>.
        /// </summary>
        /// <typeparam name="T">The most nested type</typeparam>
        /// <typeparam name="Q">The type of the expression tree root</typeparam>
        /// <param name="prop">The property of <typeparamref name="Q"/> in question</param>
        /// <param name="objs">Either the Id or the Key</param>
        /// <returns>The expression tree.</returns>
        private static Expression<Func<Q, bool>> ExpressionListWherePropListCountainsElementWithGivenKeys<T, Q>(PropertyInfo prop, params object[] objs)
        {
            var param = Expression.Parameter(typeof(Q));
            // exp = q => q.prop
            var exp = Expression.Property(param, prop);

            MethodInfo methodExpressionWhereKeysAre = typeof(GenericTools).GetMethod("ExpressionWhereKeysAre", BindingFlags.NonPublic | BindingFlags.Static)
                                                                             .MakeGenericMethod(typeof(T));
            // exp2 = t => t.Id == id OR exp2 = t => t.key1 == key1value && ... && t.keyn == keynvalue
            Expression exp2 = (Expression)methodExpressionWhereKeysAre.Invoke(typeof(GenericTools), new object[] { objs });

            MethodInfo methodWhere = typeof(Enumerable).GetMethods().Where(m => m.Name == "Where").ToList()[0].MakeGenericMethod(typeof(T));
            // body = q => q.prop.Where(exp2)
            Expression body = Expression.Call(methodWhere, exp, exp2);

            MethodInfo methodCount = typeof(Enumerable).GetMethods().Where(m => m.Name == "Count" && m.GetParameters().Length == 1).Single()
                                                       .MakeGenericMethod(typeof(T));
            // body = q => q.prop.Where(exp2).Count()
            body = Expression.Call(methodCount, body);

            // body = q => q.prop.Where(exp2).Count() >= 1
            body = Expression.GreaterThanOrEqual(body, Expression.Constant(1));
            return Expression.Lambda<Func<Q, bool>>(body, param);
        }

        /// <summary>
        /// From a list <paramref name="req"/> of elements <typeparamref name="Q"/>, get specific elements according to
        /// the predicate <see cref="ExpressionListWherePropListCountainsElementWithGivenKeys{T, Q}(PropertyInfo, object[])"/>
        /// applied to the property of <typeparamref name="Q"/> with name <paramref name="propname"/> and either the Id
        /// or the keys given by <paramref name="objs"/>. 
        /// <br/>
        /// Ie the elements of type <typeparamref name="Q"/> have a property <paramref name="prop"/>
        /// which is a list of elements of type <typeparamref name="T"/>. 
        /// <br/>
        /// This returns essentially : <c>req.Where(q => q.prop.Where(t => t.keysorId == objs).Count() >= 1)</c>
        /// <br/>
        /// That is to say the list of elements of type <typeparamref name="Q"/> so that their property <paramref name="prop"/> is
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
        private static List<Q> ListWherePropListCountainsElementWithGivenKeys<T, Q>(List<Q> req, Type q, string propname, params object[] objs)
        {
            Func<Q, bool> func = ExpressionListWherePropListCountainsElementWithGivenKeys<T, Q>(q.GetProperty(propname), objs).Compile();
            var tempreq = req.Where(func);
            if (tempreq.Count() == 0)
                return new List<Q>();
            else
                return tempreq.ToList();
            //req.Where( t => t.propname.Where(...).Count()>=1)
        }

        /// <summary>
        /// Get an expression tree having root of type <typeparamref name="Q"/>. These elements have a property <paramref name="prop"/>
        /// which is a list of elements of type <typeparamref name="T"/>. 
        /// <br/>
        /// This returns essentially : <c>q => q.prop.Where(t => t.keysorId == objs).Count() == 1 &amp;&amp; q.prop.Count() == 1</c>
        /// <br/>
        /// That is to say, for an element of type <typeparamref name="Q"/> so that their property <paramref name="prop"/> is
        /// a <see cref="IList{T}"/>, whether or not it contains only an element with either Id or Key given by <paramref name="objs"/>.
        /// </summary>
        /// <typeparam name="T">The most nested type</typeparam>
        /// <typeparam name="Q">The type of the expression tree root</typeparam>
        /// <param name="prop">The property of <typeparamref name="Q"/> in question</param>
        /// <param name="objs">Either the Id or the Key</param>
        /// <returns>The expression tree.</returns>
        private static Expression<Func<Q, bool>> ExpressionListWherePropListCountainsOnlyElementWithGivenKeys<T, Q>(PropertyInfo prop, params object[] objs)
        {
            var param = Expression.Parameter(typeof(Q));
            // exp = q => q.prop
            var exp = Expression.Property(param, prop);

            MethodInfo methodExpressionWhereKeysAre = typeof(GenericTools).GetMethod("ExpressionWhereKeysAre", BindingFlags.NonPublic | BindingFlags.Static)
                                                                             .MakeGenericMethod(typeof(T));
            // exp2 = t => t.Id == id OR exp2 = t => t.key1 == key1value && ... && t.keyn == keynvalue
            Expression exp2 = (Expression)methodExpressionWhereKeysAre.Invoke(typeof(GenericTools), new object[] { objs });

            MethodInfo methodWhere = typeof(Enumerable).GetMethods().Where(m => m.Name == "Where").ToList()[0].MakeGenericMethod(typeof(T));
            // body = q => q.prop.Where(exp2)
            Expression body = Expression.Call(methodWhere, exp, exp2);

            MethodInfo methodCount = typeof(Enumerable).GetMethods().Where(m => m.Name == "Count" && m.GetParameters().Length == 1).Single()
                                                       .MakeGenericMethod(typeof(T));
            // body = q => q.prop.Where(exp2).Count()
            body = Expression.Call(methodCount, body);

            // body = q => q.prop.Where(exp2).Count() == 1
            body = Expression.Equal(body, Expression.Constant(1));

            body = Expression.And(body,
                                  Expression.Equal(Expression.Call(methodCount, Expression.Property(param, prop)), Expression.Constant(1)));
            return Expression.Lambda<Func<Q, bool>>(body, param);
        }

        /// <summary>
        /// From a list <paramref name="req"/> of elements <typeparamref name="Q"/>, get specific elements according to
        /// the predicate <see cref="ExpressionListWherePropListCountainsOnlyElementWithGivenKeys{T, Q}(PropertyInfo, object[])"/>
        /// applied to the property of <typeparamref name="Q"/> with name <paramref name="propname"/> and either the Id
        /// or the keys given by <paramref name="objs"/>. 
        /// <br/>
        /// Ie the elements of type <typeparamref name="Q"/> have a property <paramref name="prop"/>
        /// which is a list of elements of type <typeparamref name="T"/>. 
        /// <br/>
        /// This returns essentially : <c>req.Where(q => q.prop.Where(t => t.keysorId == objs).Count() == 1 &amp;&amp; q.prop.Count() == 1)</c>
        /// <br/>
        /// That is to say the list of elements of type <typeparamref name="Q"/> so that their property <paramref name="prop"/> is
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
        private static List<Q> ListWherePropListCountainsOnlyElementWithGivenKeys<T, Q>(List<Q> req, Type q, string propname, params object[] objs)
        {
            Func<Q, bool> func = ExpressionListWherePropListCountainsOnlyElementWithGivenKeys<T, Q>(q.GetProperty(propname), objs).Compile();
            var tempreq = req.Where(func);
            if (tempreq.Count() == 0)
                return new List<Q>();
            else
                return tempreq.ToList();
            //req.Where( t => t.propname.Where(...).Count()==1)
        }

        /// <summary>
        /// Create an expression tree with principal root of type <typeparamref name="T"/> so that
        /// one of the keys is different than the given <paramref name="objs"/>
        /// <br/>
        /// Essentially gives <c>t => t.key1 != key1value || ... || t.keyn != keynvalue</c>
        /// </summary>
        /// <remarks>It is assumed that <paramref name="objs"/> are keys and not an Id. See <see cref="ExpressionListRemoveElementWithGivenId{T}(int?)"/>
        /// to see the other case.</remarks>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="objs">Either the Id or the Keys</param>
        /// <returns>The expression tree</returns>
        private static Expression<Func<T, bool>> ExpressionListRemoveElementWithGivenKeys<T>(params object[] objs)
        {
            var param = Expression.Parameter(typeof(T));
            Expression body = Expression.IsFalse(Expression.Equal(Expression.Property(param, typeof(T).GetProperty(KeyPropertiesNames<T>()[0])),
                                                                  Expression.Constant(objs[0], typeof(T).GetProperty(KeyPropertiesNames<T>()[0]).PropertyType)));
            for (int i = 1; i < objs.Length; i++)
            {
                body = Expression.Or(body,
                                    Expression.IsFalse(Expression.Equal(Expression.Property(param, typeof(T).GetProperty(KeyPropertiesNames<T>()[i])),
                                                                    Expression.Constant(objs[i], typeof(T).GetProperty(KeyPropertiesNames<T>()[i]).PropertyType))));
            }
            // t => t.key1 != value1 || t.key2 != value2 ...
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        /// <summary>
        /// Create an expression tree with principal root of type <typeparamref name="T"/> so that the
        /// Id is different from the given <paramref name="id"/> 
        /// <br/>
        /// Essentially gives <c>t => t.Id != id</c>
        /// </summary>
        /// <remarks>It is assumed that the Id is given, not keys. See <see cref="ExpressionListRemoveElementWithGivenKeys{T}(object[])"/>
        /// to see the other case.</remarks>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="objs">Either the Id or the Keys</param>
        /// <returns>The expression tree</returns>
        private static Expression<Func<T, bool>> ExpressionListRemoveElementWithGivenId<T>(int? id)
        {
            var param = Expression.Parameter(typeof(T));
            var body = Expression.IsFalse(Expression.Equal(Expression.Property(param, typeof(T).GetProperty("Id")),
                                                           Expression.Constant(id, typeof(int?))));
            return Expression.Lambda<Func<T, bool>>(body, param);
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
        private static List<T> ListRemoveElementWithGivenKeys<T>(List<T> req, params object[] objs)
        {
            CheckIfObjectIsKey<T>(objs);
            Func<T, bool> func;
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                int id = (int)objs[0];
                func = ExpressionListRemoveElementWithGivenId<T>(id).Compile();
            }
            else
            {
                func = ExpressionListRemoveElementWithGivenKeys<T>(objs).Compile();
            }
            return req.Where(func).ToList();
        }

        /// <summary>
        /// Return <see langword="true"/> if and only if <paramref name="t1"/> has a property of type either <paramref name="t2"/>
        /// or <see cref="IList"/>&lt;<paramref name="t2"/>&gt;. That is to say that <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and that <paramref name="t1"/> has a property concerning this relationship.
        /// </summary>
        /// <param name="t1">The type for which properties have to be invistigated</param>
        /// <param name="t2">The type to check whether or not is is in a relationship with <paramref name="t1"/>
        /// for which <paramref name="t1"/> has a property.</param>
        /// <returns><see langword="true"/> if <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and <paramref name="t1"/> has a property concerning this relationship. <see langword="false"/>
        /// otherwise</returns>
        private static bool HasPropertyRelation(Type t1, Type t2)
        {
            return HasPropertyRelationNotList(t1, t2) || HasPropertyRelationList(t1, t2);
        }

        /// <summary>
        /// Return <see langword="true"/> if and only if <paramref name="t1"/> has a property of type <paramref name="t2"/>
        /// That is to say that <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and that <paramref name="t1"/> has a property concerning this relationship of type <paramref name="t2"/>
        /// </summary>
        /// <param name="t1">The type for which properties have to be invistigated</param>
        /// <param name="t2">The type to check whether or not is is in a relationship with <paramref name="t1"/>
        /// for which <paramref name="t1"/> has a property of type <paramref name="t2"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and <paramref name="t1"/> has a property concerning this relationship of type <paramref name="t2"/>. 
        /// <see langword="false"/> otherwise</returns>
        private static bool HasPropertyRelationNotList(Type t1, Type t2)
        {
            return DynamicDBTypesForType(t1).Values.Contains(t2);
        }

        /// <summary>
        /// Return <see langword="true"/> if and only if <paramref name="t1"/> has a property of type <see cref="IList"/>&lt;<paramref name="t2"/>&gt;
        /// That is to say that <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and that <paramref name="t1"/> has a property concerning this relationship of type 
        /// <see cref="IList"/>&lt;<paramref name="t2"/>&gt;
        /// </summary>
        /// <param name="t1">The type for which properties have to be invistigated</param>
        /// <param name="t2">The type to check whether or not is is in a relationship with <paramref name="t1"/>
        /// for which <paramref name="t1"/> has a property of type <see cref="IList"/>&lt;<paramref name="t2"/>&gt;.</param>
        /// <returns><see langword="true"/> if <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and <paramref name="t1"/> has a property concerning this relationship of type 
        /// <see cref="IList"/>&lt;<paramref name="t2"/>&gt;. <see langword="false"/> otherwise</returns>
        private static bool HasPropertyRelationList(Type t1, Type t2)
        {
            return DynamicDBListTypesForType(t1).Values.Contains(t2);
        }

        /// <summary>
        /// Returns whether or not the type <paramref name="t1"/> has a property representing a relationship
        /// with <paramref name="t2"/>, that is to say of type <paramref name="t2"/> or <see cref="IList"/>&lt;<paramref name="t2"/>&gt;
        /// which has the annotation <see cref="RequiredAttribute"/>.
        /// </summary>
        /// <param name="t1">The type for which properties have to be invistigated</param>
        /// <param name="t2">The type to check whether or not is is in a relationship with <paramref name="t1"/>
        /// for which <paramref name="t1"/> has a property with the annotation <see cref="RequiredAttribute"/>.</param>
        /// <returns></returns>
        private static bool HasPropertyRelationRequired(Type t1, Type t2)
        {
            if (HasPropertyRelationNotList(t1, t2))
            {
                List<string> PropNames = DynamicDBTypesForType(t1).Where((propnametype) => propnametype.Value == t2).Select(propnametype => propnametype.Key).ToList();
                return t1.GetProperties().Where(p => PropNames.Contains(p.Name)).Where(p => p.GetCustomAttribute(typeof(RequiredAttribute), false) != null).Count() != 0;
            }
            if (HasPropertyRelationList(t1, t2))
            {
                List<string> PropNames = DynamicDBListTypesForType(t1).Where((propnametype) => propnametype.Value == t2).Select(propnametype => propnametype.Key).ToList();
                return t1.GetProperties().Where(p => PropNames.Contains(p.Name)).Where(p => p.GetCustomAttribute(typeof(RequiredAttribute), false) != null).Count() != 0;
            }
            return false;
        }

        /// <summary>
        /// Get all types t that are in relation with <typeparamref name="T"/> so that :
        /// <list type="bullet">
        /// <item>
        /// t has a property representing that relationship, that is to say either of type <typeparamref name="T"/> or <see cref="IList{T}"/>
        /// </item>
        /// <item>
        /// <typeparamref name="T"/> has no property representing that relationship.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <returns>The list of types in relationship with <typeparamref name="T"/> that have a property for the relation and
        /// <typeparamref name="T"/> has no such property.</returns>
        private static IEnumerable<Type> GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty<T>()
        {
            List<Type> res = new List<Type>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                                                                 .Where(myType => myType.IsClass && !myType.IsAbstract
                                                                                                 && (myType.IsSubclassOf(typeof(BaseEntity)) || myType.IsSubclassOf(typeof(EntityWithKeys)))
                                                                       ))
            {
                if (HasPropertyRelation(type, typeof(T)))
                    if (!HasPropertyRelation(typeof(T), type))
                        res.Add(type);
            }
            return res;
        }

        /// <summary>
        /// Get all types t that are in relation with <typeparamref name="T"/> so that :
        /// <list type="bullet">
        /// <item>
        /// t has a property representing that relationship, that is to say either of type <typeparamref name="T"/> or <see cref="IList{T}"/>
        /// </item>
        /// <item>
        /// that property is has the annotation <see cref="RequiredAttribute"/>
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <returns>The list of types in relationship with <typeparamref name="T"/> that have a property for the relation 
        /// with the annotation <see cref="RequiredAttribute"/></returns>
        private static IEnumerable<Type> GetTypesInRelationWithTHavingRequiredTProperty<T>()
        {
            List<Type> res = new List<Type>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                                                                 .Where(myType => myType.IsClass && !myType.IsAbstract
                                                                                                 && (myType.IsSubclassOf(typeof(BaseEntity)) || myType.IsSubclassOf(typeof(EntityWithKeys)))
                                                                       ))
            {
                if (HasPropertyRelationRequired(type, typeof(T)))
                    res.Add(type);
            }
            return res;
        }

        /// <summary>
        /// Return <see langword="true"/> if and only if <paramref name="t1"/> has many property of type either <paramref name="t2"/>
        /// or <see cref="IList"/>&lt;<paramref name="t2"/>&gt;. That is to say that <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and that <paramref name="t1"/> has more than one property concerning this relationship.
        /// </summary>
        /// <param name="t1">The type for which properties have to be invistigated</param>
        /// <param name="t2">The type to check whether or not is is in a relationship with <paramref name="t1"/>
        /// for which <paramref name="t1"/> has more than one property.</param>
        /// <returns><see langword="true"/> if <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and <paramref name="t1"/> has more than one property concerning this relationship. <see langword="false"/>
        /// otherwise</returns>
        private static bool HasManyProperties(Type t1, Type t2)
        {
            int countNotList = DynamicDBTypesForType(t1).Values.Where(t => t == t2).Count();
            int countList = DynamicDBListTypesForType(t1).Values.Where(t => t == t2).Count();
            return countNotList + countList > 1;
        }

        /// <summary>
        /// Get the list of types t that are many relationships with <typeparamref name="T"/>, that is to say that
        /// <typeparamref name="T"/> has many properties of types either t or <see cref="IList"/>&lt;t&gt;.
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <returns>The list of types</returns>
        private static IEnumerable<Type> GetTypesForWhichTHasManyProperties<T>()
        {
            List<Type> res = new List<Type>();
            foreach (Type type in DynamicDBTypes<T>().Values.Concat(DynamicDBListTypes<T>().Values))
            {
                if (HasManyProperties(typeof(T), type))
                    res.Add(type);
            }
            return res;
        }

        /// <summary>
        /// Get the list of types t that are in exactly one relationship with <typeparamref name="T"/>, that is to say that
        /// <typeparamref name="T"/> has exactly one property of types either t or <see cref="IList"/>&lt;t&gt;.
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <returns>The list of types</returns>
        private static IEnumerable<Type> GetTypesForWhichTHasOneProperty<T>()
        {
            List<Type> res = new List<Type>();
            foreach (Type type in DynamicDBTypes<T>().Values.Concat(DynamicDBListTypes<T>().Values))
            {
                if (!HasManyProperties(typeof(T), type))
                    res.Add(type);
            }
            return res;
        }

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
        private static dynamic GetServiceFromContext(MyDbContext context, Type t)
        {
            Type typetRepository = Assembly.GetAssembly(t).GetTypes().Single(typ => typ.Name == t.Name + "Repository");
            Type typetService = Assembly.GetAssembly(t).GetTypes().Single(typ => typ.Name == t.Name + "Service");
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
        /// of <see cref="GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty{T}"/></remarks>
        /// <typeparam name="T">The type of the element deleted with either Id or keys <paramref name="objs"/> for which actions 
        /// have to be taken</typeparam>
        /// <param name="context">The context in which to do this operation</param>
        /// <param name="q">The type of the elements to be updated before the deletion of the element of type <typeparamref name="T"/> with 
        /// either Id or keys <paramref name="objs"/></param>
        /// <param name="propname">The name of the property of <paramref name="q"/> having type <typeparamref name="T"/></param>
        /// <param name="objs">Either the Id or keys of the element of type <typeparamref name="T"/> deleted</param>
        private static void SetForTypePropertyWithGivenKeysToNullInNewContext<T>(MyDbContext context, Type q, string propname, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            MethodInfo methodListWherePropNotNull = typeof(GenericTools).GetMethod("ListWherePropNotNull", BindingFlags.NonPublic | BindingFlags.Static)
                                                                                    .MakeGenericMethod(new Type[] { q });
            dynamic req = methodListWherePropNotNull.Invoke(typeof(GenericTools), new object[] { qService.GetAllIncludes(1, int.MaxValue, null, null), propname });

            MethodInfo methodQueryWherePropKeysAre = typeof(GenericTools).GetMethod("ListWherePropKeysAre", BindingFlags.NonPublic | BindingFlags.Static)
                                                                         .MakeGenericMethod(new Type[] { typeof(T), q });
            req = methodQueryWherePropKeysAre.Invoke(typeof(GenericTools), new object[] { req, propname, objs });

            foreach (var qItem in req)
            {
                using (MyDbContext context2 = new MyDbContext())
                {
                    dynamic qService2 = GetServiceFromContext(context2, q);

                    MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                         .MakeGenericMethod(new Type[] { q });

                    if (q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) == null)
                    {
                        var qItem2 = qService2.FindByIdIncludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));
                        qService2.UpdateOne(qItem2, propname, null);
                    }
                    else
                    {
                        var qItem2 = qService2.FindByIdExcludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));
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
        /// <list>
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
        private static void RemoveForTypePropertyListElementWithGivenKeyInNewContext<T>(MyDbContext context, Type q, string propname, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            MethodInfo methodExpressionListWherePropListCountainsElementWithGivenKeys = typeof(GenericTools).GetMethod("ExpressionListWherePropListCountainsElementWithGivenKeys")
                                                                                                            .MakeGenericMethod(new Type[] { typeof(T), q });
            dynamic func = methodExpressionListWherePropListCountainsElementWithGivenKeys.Invoke(typeof(GenericTools), ConcatArrayWithParams(new object[] { q.GetProperty(propname) }, objs));

            dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, func);

            foreach (var qItem in req)
            {
                using (MyDbContext context2 = new MyDbContext())
                {
                    dynamic qService2 = GetServiceFromContext(context2, q);

                    MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                         .MakeGenericMethod(new Type[] { q });
                    var qItem2 = qService2.FindByIdIncludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));

                    var oldValue = qItem2.GetType().GetProperty(propname).GetValue(qItem2);
                    if (q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) != null && ((oldValue as IList).Count == 1))
                    {
                        using (MyDbContext context3 = new MyDbContext())
                        {
                            dynamic qService3 = GetServiceFromContext(context3, q);
                            var qItem3 = qService3.FindByIdIncludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));
                            qService3.Delete(qItem3);
                        }
                    }
                    else
                    {
                        MethodInfo methodListRemoveElementWithGivenKeys = typeof(GenericTools).GetMethod("ListRemoveElementWithGivenKeys", BindingFlags.NonPublic | BindingFlags.Static)
                                                                                              .MakeGenericMethod(new Type[] { typeof(T) });
                        var newValue = methodListRemoveElementWithGivenKeys.Invoke(typeof(GenericTools), new object[] { oldValue, objs });
                        qService2.UpdateOne(qItem2, propname, newValue);
                    }
                }
            }
        }

        /// <summary>
        /// An object of type <typeparamref name="T"/> with either Id or keys <paramref name="objs"/> is deleted.
        /// Every type <paramref name="q"/> in <see cref="GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty{T}"/> has to be updated
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
        private static void DeleteOtherPropInRelationWithTHavingTPropertyTAndTNotHavingProperty<T>(Type q, params object[] objs)
        {
            if (HasPropertyRelationNotList(q, typeof(T)))
            {
                List<string> propnames = DynamicDBTypesForType(q).Where(kv => kv.Value == typeof(T))
                                                                              .Select(kv => kv.Key)
                                                                              .ToList();
                using (MyDbContext context = new MyDbContext())
                {
                    foreach (string propname in propnames)
                    {
                        SetForTypePropertyWithGivenKeysToNullInNewContext<T>(context, q, propname, objs);
                    }
                }
            }
            else
            {
                if (HasPropertyRelationList(q, typeof(T)))
                {
                    List<string> propnames = DynamicDBListTypesForType(q).Where(kv => kv.Value == typeof(T))
                                                                                      .Select(kv => kv.Key)
                                                                                      .ToList();
                    using (MyDbContext context = new MyDbContext())
                    {
                        foreach (string propname in propnames)
                        {
                            RemoveForTypePropertyListElementWithGivenKeyInNewContext<T>(context, q, propname, objs);
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
        /// of type <see cref="IList{T}"/> (an empty <see cref="List"/> is not <see langword="null"/> and the annotation
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
        private static void DeleteItemOfTypeWithRequiredPropertyHavingGivenKeysInNewContext<T>(MyDbContext context, Type q, string propname, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, null);
            MethodInfo methodQueryWherePropKeysAre = typeof(GenericTools).GetMethod("ListWherePropKeysAre", BindingFlags.NonPublic | BindingFlags.Static)
                                                                         .MakeGenericMethod(new Type[] { typeof(T), q });
            req = methodQueryWherePropKeysAre.Invoke(typeof(GenericTools), new object[] { req, propname, objs });
            foreach (dynamic qItem in req)
            {
                using (MyDbContext context2 = new MyDbContext())
                {
                    dynamic qService2 = GetServiceFromContext(context, q);

                    MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                         .MakeGenericMethod(new Type[] { q });
                    var qItem2 = qService2.FindByIdExcludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));
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
        /// of type <see cref="IList{T}"/> (an empty <see cref="List"/> is not <see langword="null"/> and the annotation
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
        /// that <paramref name="q"/> is part of <see cref="GetTypesInRelationWithTHavingRequiredTProperty{T}"/> with a property
        /// of type <typeparamref name="T"/>.</remarks>
        /// <typeparam name="T">The type of the element deleted with either Id or keys <paramref name="objs"/> for which actions 
        /// have to be taken</typeparam>
        /// <param name="context">The context in which to do this operation</param>
        /// <param name="q">The type of the elements to be updated before the deletion of the element of type <typeparamref name="T"/> with 
        /// either Id or keys <paramref name="objs"/></param>
        /// <param name="propname">The name of the property of <paramref name="q"/> having type <typeparamref name="T"/></param>
        /// <param name="objs">Either the Id or keys of the element of type <typeparamref name="T"/> deleted</param>
        private static void DeleteOrUpdateItemOfTypeWithRequiredListPropertyHavingGivenKeysInNewContext<T>(MyDbContext context, Type q, string propname, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, null);

            MethodInfo methodListWherePropListCountainsElementWithGivenKeys = typeof(GenericTools).GetMethod("ListWherePropListCountainsElementWithGivenKeys", BindingFlags.NonPublic | BindingFlags.Static)
                                                                                                  .MakeGenericMethod(new Type[] { typeof(T), q });
            req = methodListWherePropListCountainsElementWithGivenKeys.Invoke(typeof(GenericTools), new object[] { req, q, propname, objs });

            foreach (var qItem in req)
            {
                using (MyDbContext context2 = new MyDbContext())
                {
                    dynamic qService2 = GetServiceFromContext(context2, q);

                    MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                         .MakeGenericMethod(new Type[] { q });
                    var qItem2 = qService2.FindByIdIncludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));

                    var oldValue = qItem2.GetType().GetProperty(propname).GetValue(qItem2);
                    if ((oldValue as IList).Count == 1)
                    {
                        using (MyDbContext context3 = new MyDbContext())
                        {
                            dynamic qService3 = GetServiceFromContext(context3, q);
                            var qItem3 = qService3.FindByIdExcludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));
                            qService3.Delete(qItem3);
                        }

                    }
                    else
                    {
                        MethodInfo methodListRemoveElementWithGivenKeys = typeof(GenericTools).GetMethod("ListRemoveElementWithGivenKeys", BindingFlags.NonPublic | BindingFlags.Static)
                                                                                              .MakeGenericMethod(new Type[] { typeof(T) });
                        var newValue = methodListRemoveElementWithGivenKeys.Invoke(typeof(GenericTools), new object[] { oldValue, objs });
                        qService2.UpdateOne(qItem2, propname, newValue);
                    }
                }
            }
        }

        /// <summary>
        /// An object of type <typeparamref name="T"/> with either Id or keys <paramref name="objs"/> is deleted.
        /// Every type <paramref name="q"/> in <see cref="GetTypesInRelationWithTHavingRequiredTProperty"/> has to be updated
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
        private static void DeleteOtherPropInRelationWithTHavingRequiredTProperty<T>(Type q, params object[] objs)
        {
            // person -> finger
            // person -> action
            if (HasPropertyRelationNotList(q, typeof(T)))
            {
                List<string> propnames = DynamicDBTypesForType(q).Where(kv => kv.Value == typeof(T)).Select(kv => kv.Key)
                                                                              .Where(propname => q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) != null)
                                                                              .ToList();
                using (MyDbContext context = new MyDbContext())
                {
                    foreach (string propname in propnames)
                    {
                        DeleteItemOfTypeWithRequiredPropertyHavingGivenKeysInNewContext<T>(context, q, propname, objs);
                    }
                }
            }
            else
            {
                if (HasPropertyRelationList(q, typeof(T)))
                {
                    List<string> propnames = DynamicDBListTypesForType(q).Where(kv => kv.Value == typeof(T)).Select(kv => kv.Key)
                                                                                      .Where(propname => q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) != null)
                                                                                      .ToList();
                    using (MyDbContext context = new MyDbContext())
                    {
                        foreach (string propname in propnames)
                        {
                            DeleteOrUpdateItemOfTypeWithRequiredListPropertyHavingGivenKeysInNewContext<T>(context, q, propname, objs);
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
        private static List<Q> ListWhereMultiplePropKeysAre<T, Q>(List<Q> req, List<string> propnames, object[] objs)
        {
            foreach (string propname in propnames)
            {
                req = ListWherePropKeysAre<T, Q>(req, propname, objs);
            }
            return req;
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
        /// of <see cref="GetTypesForWhichTHasManyProperties{T}"/></remarks>
        /// <typeparam name="T">The type of the element deleted with either Id or keys <paramref name="objs"/> for which actions 
        /// have to be taken</typeparam>
        /// <param name="context">The context in which to do this operation</param>
        /// <param name="q">The type of the elements to be updated before the deletion of the element of type <typeparamref name="T"/> with 
        /// either Id or keys <paramref name="objs"/></param>
        /// <param name="propname">The name of the property of <paramref name="q"/> having type <typeparamref name="T"/></param>
        /// <param name="objs">Either the Id or keys of the element of type <typeparamref name="T"/> deleted</param>
        private static void SetForMultipleTypePropertyWithGivenKeysToNullInNewContext<T>(MyDbContext context, Type q, List<string> propnames, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            MethodInfo methodListWherePropNotNull = typeof(GenericTools).GetMethod("ListWherePropNotNull", BindingFlags.NonPublic | BindingFlags.Static)
                                                                                    .MakeGenericMethod(new Type[] { q });
            dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, null);

            foreach (string propname in propnames)
            {
                req = methodListWherePropNotNull.Invoke(typeof(GenericTools), new object[] { req, propname });
            }

            MethodInfo methodQueryWhereMultiplePropKeysAre = typeof(GenericTools).GetMethod("ListWhereMultiplePropKeysAre", BindingFlags.NonPublic | BindingFlags.Static)
                                                                         .MakeGenericMethod(new Type[] { typeof(T), q });
            req = methodQueryWhereMultiplePropKeysAre.Invoke(typeof(GenericTools), new object[] { req, propnames, objs });

            foreach (var qItem in req)
            {
                using (MyDbContext context2 = new MyDbContext())
                {
                    dynamic qService2 = GetServiceFromContext(context2, q);

                    MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                         .MakeGenericMethod(new Type[] { q });
                    var qItem2 = qService2.FindByIdIncludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));

                    bool isDeleted = false;
                    foreach (string propname in propnames)
                    {
                        if (q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) != null)
                        {
                            using (MyDbContext context3 = new MyDbContext())
                            {
                                dynamic qService3 = GetServiceFromContext(context3, q);
                                var qItem3 = qService3.FindByIdExcludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));
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
        /// Get an expression tree having root of type <typeparamref name="Q"/>. These elements have properties <paramref name="prop"/>
        /// which is a list of elements of type <typeparamref name="T"/>. 
        /// <br/>
        /// This returns essentially : <c>q => q.prop1.Where(t => t.keysorId == objs).Count() >= 1 &amp;&amp; ... &amp;&amp; q.propn.Where(t => t.keysorId == objs).Count() >= 1</c>
        /// <br/>
        /// That is to say, for an element of type <typeparamref name="Q"/> so that their properties <paramref name="propnames"/> is
        /// a <see cref="IList{T}"/>, whether or not all of them contains an element with either Id or Key given by <paramref name="objs"/>.
        /// </summary>
        /// <typeparam name="T">The most nested type</typeparam>
        /// <typeparam name="Q">The type of the expression tree root</typeparam>
        /// <param name="propnames">The properties names of <typeparamref name="Q"/> in question</param>
        /// <param name="objs">Either the Id or the Key</param>
        /// <returns>The expression tree.</returns>
        private static Expression<Func<Q, bool>> ExpressionListWhereMultiplePropListCountainsElementWithGivenKeys<T, Q>(List<string> propnames, params object[] objs)
        {
            var param = Expression.Parameter(typeof(Q));
            // exp = q => q.prop0
            var exp = Expression.Property(param, typeof(Q).GetProperty(propnames[0]));

            MethodInfo methodExpressionWhereKeysAre = typeof(GenericTools).GetMethod("ExpressionWhereKeysAre", BindingFlags.NonPublic | BindingFlags.Static)
                                                                             .MakeGenericMethod(typeof(T));
            // exp2 = t => t.Id == id OR exp2 = t => t.key1 == key1value && ... && t.keyn == keynvalue
            Expression exp2 = (Expression)methodExpressionWhereKeysAre.Invoke(typeof(GenericTools), new object[] { objs });

            MethodInfo methodWhere = typeof(Enumerable).GetMethods().Where(m => m.Name == "Where").ToList()[0].MakeGenericMethod(typeof(T));
            // body = q => q.prop0.Where(exp2)
            Expression body = Expression.Call(methodWhere, exp, exp2);

            MethodInfo methodCount = typeof(Enumerable).GetMethods().Where(m => m.Name == "Count" && m.GetParameters().Length == 1).Single()
                                                       .MakeGenericMethod(typeof(T));
            // body = q => q.prop0.Where(exp2).Count()
            body = Expression.Call(methodCount, body);

            // body = q => q.prop0.Where(exp2).Count() >= 1
            body = Expression.GreaterThanOrEqual(body, Expression.Constant(1));

            for (int i = 1; i < propnames.Count; i++)
            {
                var exp3 = Expression.Property(param, typeof(Q).GetProperty(propnames[i]));
                Expression exp4 = (Expression)methodExpressionWhereKeysAre.Invoke(typeof(GenericTools), new object[] { objs });
                Expression exp5 = Expression.Call(methodWhere, exp3, exp4);
                Expression exp6 = Expression.Call(methodCount, exp5);
                Expression exp7 = Expression.GreaterThanOrEqual(exp6, Expression.Constant(1));
                body = Expression.AndAlso(body, exp7);

            }
            return Expression.Lambda<Func<Q, bool>>(body, param);
        }

        /// <summary>
        /// In a given context <paramref name="context"/>, for the type <paramref name="q"/> with the properties with name 
        /// <paramref name="propnames"/> of type <typeparamref name="T"/>, prepare the deletion of a given element of type <typeparamref name="T"/>
        /// with either Id or keys <paramref name="objs"/>. 
        /// <br/>
        /// That is to say, for every element of type <paramref name="q"/> such that
        /// their properties <paramref name="propnames"/> is of type <see cref="IList{T}"/> and contains the element of either Id or keys
        /// <paramref name="objs"/>
        /// <list>
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
        private static void RemoveForMultipleTypePropertyListElementWithGivenKeyInNewContext<T>(MyDbContext context, Type q, List<string> propnames, params object[] objs)
        {
            dynamic qService = GetServiceFromContext(context, q);

            MethodInfo methodExpressionListWhereMultiplePropListCountainsElementWithGivenKeys = typeof(GenericTools).GetMethod("ExpressionListWhereMultiplePropListCountainsElementWithGivenKeys", BindingFlags.NonPublic | BindingFlags.Static)
                                                                                                            .MakeGenericMethod(new Type[] { typeof(T), q });
            dynamic func = methodExpressionListWhereMultiplePropListCountainsElementWithGivenKeys.Invoke(typeof(GenericTools), ConcatArrayWithParams(new object[] { propnames }, objs));

            dynamic req = qService.GetAllIncludes(1, int.MaxValue, null, func);

            foreach (var qItem in req)
            {
                using (MyDbContext context2 = new MyDbContext())
                {
                    dynamic qService2 = GetServiceFromContext(context2, q);

                    MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                         .MakeGenericMethod(new Type[] { q });
                    var qItem2 = qService2.FindByIdIncludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));

                    bool isDeleted = false;
                    foreach (string propname in propnames)
                    {
                        var oldValue = q.GetProperty(propname).GetValue(qItem2);
                        if (q.GetProperty(propname).GetCustomAttribute(typeof(RequiredAttribute), false) != null
                            && ((oldValue as IList).Count == 1))
                        {
                            using (MyDbContext context3 = new MyDbContext())
                            {
                                dynamic qService3 = GetServiceFromContext(context3, q);
                                var qItem3 = qService3.FindByIdExcludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));
                                qService3.Delete(qItem3);

                            }
                            isDeleted = true;
                            break;
                        }
                        else
                        {
                            MethodInfo methodListRemoveElementWithGivenKeys = typeof(GenericTools).GetMethod("ListRemoveElementWithGivenKeys", BindingFlags.NonPublic | BindingFlags.Static)
                                                                                              .MakeGenericMethod(new Type[] { typeof(T) });
                            var newValue = methodListRemoveElementWithGivenKeys.Invoke(typeof(GenericTools), new object[] { oldValue, objs });
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
        /// Every type <paramref name="q"/> in <see cref="GetTypesForWhichTHasManyProperties"/> has to be updated
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
        private static void DeleteOtherPropInSeveralRelationshipsWithT<T>(Type q, params object[] objs)
        {
            if (HasPropertyRelationNotList(q, typeof(T)))
            {
                List<string> propnames = DynamicDBTypesForType(q).Where(kv => kv.Value == typeof(T))
                                                                              .Select(kv => kv.Key)
                                                                              .ToList();
                using (MyDbContext context = new MyDbContext())
                {
                    SetForMultipleTypePropertyWithGivenKeysToNullInNewContext<T>(context, q, propnames, objs);
                }
            }
            else
            {
                if (HasPropertyRelationList(q, typeof(T)))
                {
                    List<string> propnames = DynamicDBListTypesForType(q).Where(kv => kv.Value == typeof(T))
                                                                                      .Select(kv => kv.Key)
                                                                                      .ToList();
                    using (MyDbContext context = new MyDbContext())
                    {
                        RemoveForMultipleTypePropertyListElementWithGivenKeyInNewContext<T>(context, q, propnames, objs);
                    }
                }
                else
                {
                    //throw new HasNoPropertyRelationException(q, typeof(T));
                }
            }
        }

        /// <summary>
        /// Every step that has to be taken into account before deleting an object of type <typeparamref name="T"/> having
        /// either Id or keys <paramref name="objs"/>.
        /// <br/>
        /// See <see cref="DeleteOtherPropInRelationWithTHavingTPropertyTAndTNotHavingProperty"/>, 
        /// <see cref="DeleteOtherPropInRelationWithTHavingRequiredTProperty"/>,
        /// <see cref="DeleteOtherPropInSeveralRelationshipsWithT"/> for more details.
        /// <br/> 
        /// In a nutshell :
        /// <list type="bullet">
        /// <item>
        /// Every type <paramref name="q"/> in <see cref="GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty{T}"/> has to be updated
        /// manually. 
        /// <br/>
        /// Indeed, <typeparamref name="T"/> has no property representing that relation, and thus no element of 
        /// type <paramref name="q"/> will be loaded in the context and changed, wich will result in exceptions if not
        /// taken care of.
        /// </item>
        /// <item>
        /// Every type <paramref name="q"/> in <see cref="GetTypesInRelationWithTHavingRequiredTProperty"/> has to be updated
        /// manually. 
        /// <br/>
        /// Indeed, required properties are not handled well in EF in case of relationships, especially if they are
        /// of type <see cref="IList"/> (an empty <see cref="List"/> is not <see langword="null"/> and the annotation
        /// <see cref="RequiredAttribute"/> is interpreted as nullable = <see langword="false"/>)
        /// </item>
        /// <item>
        /// Every type <paramref name="q"/> in <see cref="GetTypesForWhichTHasManyProperties"/> has to be updated
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
        public static void PrepareDelete<T>(params object[] objs)
        {
            foreach (Type type in GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty<T>())
            {
                DeleteOtherPropInRelationWithTHavingTPropertyTAndTNotHavingProperty<T>(type, objs);
            }
            foreach (Type type in GetTypesInRelationWithTHavingRequiredTProperty<T>())
            {
                DeleteOtherPropInRelationWithTHavingRequiredTProperty<T>(type, objs);
            }
            foreach (Type type in GetTypesForWhichTHasManyProperties<T>())
            {
                DeleteOtherPropInSeveralRelationshipsWithT<T>(type, objs);
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
            IEnumerable<Type> TypesForWhichTHasManyProperties = GetTypesForWhichTHasManyProperties<T>();
            Dictionary<string, Type> dynamicDBTypes = DynamicDBTypes<T>();
            Dictionary<string, Type> dynamicDBListTypes = DynamicDBListTypes<T>();
            object[] res = new object[dynamicDBTypes.Count + dynamicDBListTypes.Count];
            var i = 0;
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (DynamicDBTypes<T>().Keys.Contains(prop.Name))
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
                    if (DynamicDBListTypes<T>().Keys.Contains(prop.Name))
                    {
                        if (TypesForWhichTHasManyProperties.Contains(DynamicDBListTypes<T>()[prop.Name]))
                        {
                            if (prop.GetValue(t) == null || (prop.GetValue(t) as IList).Count == 0)
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
        /// An object of type <typeparamref name="T"/> with either Id or keys <paramref name="objs"/> is about to be updated to value <paramref name="newItem"/>.
        /// The type <paramref name="q"/> has a required property of name <paramref name="propname"/> and of type <typeparamref name="T"/>, 
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
        /// <param name="qpropname">The name of the property for q</param>
        /// <param name="tpropname">The name of the property for t</param>
        /// <param name="newItem">The new value to be updated</param>
        /// <param name="objs">Either the id or keys of <paramref name="newItem"/></param>
        private static void UpdateItemOfTypeWithRequiredPropOfTypeInNewContext<T>(MyDbContext context, Type q, string qpropname, string tpropname, T newItem, params object[] objs)
        {

            if (typeof(T).GetProperty(tpropname).PropertyType == q)
            {
                dynamic tService = GetServiceFromContext(context, typeof(T));

                T oldItem = tService.FindByIdIncludes(objs);

                var oldItemProp = typeof(T).GetProperty(tpropname).GetValue(oldItem);

                var newItemProp = typeof(T).GetProperty(tpropname).GetValue(newItem);

                if (oldItemProp != newItemProp)
                {
                    using (MyDbContext context2 = new MyDbContext())
                    {
                        dynamic qService = GetServiceFromContext(context2, q);

                        MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                                    .MakeGenericMethod(new Type[] { q });
                        var qItem = qService.FindByIdExcludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { oldItemProp }));

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
                            using (MyDbContext context2 = new MyDbContext())
                            {
                                dynamic qService = GetServiceFromContext(context2, q);

                                MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                                            .MakeGenericMethod(new Type[] { q });
                                var qItem = qService.FindByIdExcludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { item }));

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
        /// Get an expression tree having principal root of type <typeparamref name="Q"/>, for testing whether or not an element
        /// <paramref name="newItem"/> of type <typeparamref name="T"/> with property <paramref name="tPropName"/> of type <see cref="IList{Q}"/>
        /// does not contain the element tested.
        /// <br/>
        /// Essentially, does <c>q => !newitem.tpropname.Where(qq => qq.Id == q.Id).Count() == 1)</c>
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="newItem"/></typeparam>
        /// <typeparam name="Q">The type of the elements invistigated</typeparam>
        /// <param name="newItem">The object</param>
        /// <param name="tPropName">The name of the property of <typeparamref name="T"/> in question</param>
        /// <param name="nbr">The number of keys for objects of type <typeparamref name="Q"/> if appropriate</param>
        /// <returns>The expression tree</returns>
        private static Expression<Func<Q, bool>> ExpressionListWhereOtherTypePropListNotContains<T, Q>(T newItem, string tPropName, int nbr = 1)
        {
            var param = Expression.Parameter(typeof(Q));
            var newItemcst = Expression.Constant(newItem, typeof(T));
            Expression body = Expression.Property(newItemcst, typeof(T).GetProperty(tPropName));

            var param2 = Expression.Parameter(typeof(Q));
            Expression exp2;
            if (typeof(BaseEntity).IsAssignableFrom(typeof(Q)))
            {
                exp2 = Expression.Equal(Expression.Property(param2, typeof(Q).GetProperty("Id")),
                                        Expression.Property(param, typeof(Q).GetProperty("Id")));
            }
            else
            {
                exp2 = Expression.Equal(Expression.Property(param2, typeof(Q).GetProperty(KeyPropertiesNames<Q>()[0])),
                                        Expression.Property(param, typeof(Q).GetProperty(KeyPropertiesNames<Q>()[0])));
                for (int i = 1; i < nbr; i++)
                {
                    exp2 = Expression.And(exp2,
                                          Expression.Equal(Expression.Property(param2, typeof(Q).GetProperty(KeyPropertiesNames<Q>()[i])),
                                                           Expression.Property(param, typeof(Q).GetProperty(KeyPropertiesNames<Q>()[i]))));
                }
            }

            MethodInfo methodWhere = typeof(Enumerable).GetMethods().Where(m => m.Name == "Where").ToList()[0].MakeGenericMethod(typeof(Q));


            body = Expression.Call(methodWhere, body, Expression.Lambda<Func<Q, bool>>(exp2, param2));

            MethodInfo methodCount = typeof(Enumerable).GetMethods().Where(m => m.Name == "Count" && m.GetParameters().Length == 1).Single()
                                                       .MakeGenericMethod(typeof(Q));

            body = Expression.Call(methodCount, body);

            body = Expression.Equal(body, Expression.Constant(1));

            body = Expression.Not(body);

            return Expression.Lambda<Func<Q, bool>>(body, param);
        }

        /// <summary>
        /// Get an expression tree having principal root of type <typeparamref name="Q"/>, for testing whether or not an element
        /// <paramref name="newItem"/> of type <typeparamref name="T"/> with property <paramref name="tPropName"/> of type <see cref="IList{Q}"/>
        /// does not contain the element tested.
        /// <br/>
        /// Essentially, does <c>req => req.Where(q => !newitem.tpropname.Where(qq => qq.Id == q.Id).Count() == 1))</c>
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="newItem"/></typeparam>
        /// <typeparam name="Q">The type of the elements invistigated</typeparam>
        /// <param name="newItem">The object</param>
        /// <param name="tPropName">The name of the property of <typeparamref name="T"/> in question</param>
        /// <param name="nbr">The number of keys for objects of type <typeparamref name="Q"/> if appropriate</param>
        /// <returns>The expression tree</returns>
        private static List<Q> ListWhereOtherTypePropListNotContains<T, Q>(List<Q> req, T newitem, string tpropname, int nbr = 1)
        {
            Func<Q, bool> func = ExpressionListWhereOtherTypePropListNotContains<T, Q>(newitem, tpropname, nbr).Compile();
            var tempreq = req.Where(func);
            if (tempreq.Count() == 0)
                return new List<Q>();
            else
                return tempreq.ToList();
            //req.Where( q => !newitem.tpropname.Where(qq => qq.Id == q.Id).Count() == 1 )
        }

        /// <summary>
        /// Get the number of keys of type <paramref name="q"/> if appropriate
        /// </summary>
        /// <param name="q">The type invistigated</param>
        /// <returns>The number of keys of type <paramref name="q"/>, 1 if it has an Id, and 0 otherwise.</returns>
        private static int NbrOfKeys(Type q)
        {
            if (q.IsSubclassOf(typeof(BaseEntity)))
                return 1;
            if (q.IsSubclassOf(typeof(EntityWithKeys)))
            {
                return q.GetProperties().Where(p => p.GetCustomAttribute(typeof(KeyAttribute), false) != null)
                                        .Count();
            }
            return 0;
        }

        /// <summary>
        /// An object of type <typeparamref name="T"/> with either Id or keys <paramref name="objs"/> is about to be updated to value <paramref name="newItem"/>.
        /// The type <paramref name="q"/> has a required property of name <paramref name="propname"/> and of type <see cref="IList{T}"/>, 
        /// and therefore if the relationship between those types changes, some items of type <paramref name="q"/> may be removed.
        /// <br/>
        /// In more details, elements of type <paramref name="q"/> to be removed are such that :
        /// <list type="bullet">
        /// <item>
        /// q.qpropname contains only oldvalue (before update)
        /// </item>
        /// <item>
        /// newItem.tpropname changes value
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
        private static void UpdateItemOfTypeWithRequiredPropOfListTypeInNewContext<T>(MyDbContext context, Type q, string qpropname, string tpropname, T newItem, params object[] objs)
        {
            if (typeof(T).GetProperty(tpropname).PropertyType == q)
            {
                dynamic tService = GetServiceFromContext(context, typeof(T));

                T oldItem = tService.FindByIdIncludes(objs);

                var oldItemProp = typeof(T).GetProperty(tpropname).GetValue(oldItem);

                var newItemProp = typeof(T).GetProperty(tpropname).GetValue(newItem);

                if (oldItemProp != newItemProp)
                {
                    if (oldItemProp == null || ((oldItemProp) as IList).Count == 1)
                    {
                        using (MyDbContext context2 = new MyDbContext())
                        {
                            dynamic qService = GetServiceFromContext(context, q);

                            MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                                            .MakeGenericMethod(new Type[] { q });
                            var qItem = qService.FindByIdExcludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { oldItemProp }));

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

                    MethodInfo methodListWherePropListCountainsOnlyElementWithGivenKeys = typeof(GenericTools).GetMethod("ListWherePropListCountainsOnlyElementWithGivenKeys", BindingFlags.NonPublic | BindingFlags.Static)
                                                                                                          .MakeGenericMethod(new Type[] { typeof(T), q });
                    req = methodListWherePropListCountainsOnlyElementWithGivenKeys.Invoke(typeof(GenericTools), ConcatArrayWithParams(new object[] { req, q, qpropname }, objs));

                    if (typeof(T).GetProperty(tpropname).GetValue(newItem) != null && (typeof(T).GetProperty(tpropname).GetValue(newItem) as IList).Count != 0)
                    {
                        MethodInfo methodListWhereOtherTypePropListNotContains = typeof(GenericTools).GetMethod("ListWhereOtherTypePropListNotContains", BindingFlags.NonPublic | BindingFlags.Static)
                                                                                                                 .MakeGenericMethod(new Type[] { typeof(T), q });
                        req = methodListWhereOtherTypePropListNotContains.Invoke(typeof(GenericTools), new object[] { req, newItem, tpropname, NbrOfKeys(q) });
                    }

                    foreach (var qItem in req)
                    {
                        using (MyDbContext context2 = new MyDbContext())
                        {
                            dynamic qService2 = GetServiceFromContext(context2, q);

                            MethodInfo methodGetKeysValues = typeof(GenericTools).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                                        .MakeGenericMethod(new Type[] { q });
                            var qItem2 = qService2.FindByIdExcludes((object[])methodGetKeysValues.Invoke(typeof(GenericTools), new object[] { qItem }));

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
        private static void UpdateOtherPropInRelationWithTHavingRequiredTProperty<T>(Type q, T newItem, string tpropname, string qpropname)
        {
            if (q.GetProperty(qpropname).PropertyType == typeof(T))
            {
                using (MyDbContext context = new MyDbContext())
                {
                    UpdateItemOfTypeWithRequiredPropOfTypeInNewContext(context, q, qpropname, tpropname, newItem, GetKeysValues(newItem));
                }
            }
            else
            {
                if (q.GetProperty(qpropname).PropertyType == typeof(IList<>).MakeGenericType(typeof(T)))
                {
                    using (MyDbContext context = new MyDbContext())
                    {
                        UpdateItemOfTypeWithRequiredPropOfListTypeInNewContext(context, q, qpropname, tpropname, newItem, GetKeysValues(newItem));
                    }
                }
                else
                {
                    //throw new HasNoPropertyRelationException(q, typeof(T));
                }
            }
        }

        /// <summary>
        /// For two types <paramref name="t1"/>, <paramref name="t2"/>, get a Dictionnary of property names (key, value) such that :
        /// <list type="bullet">
        /// <item>
        /// There is a relationship between <paramref name="t1"/> and <paramref name="t2"/> 
        /// </item>
        /// <item>
        /// The corresponding property of <paramref name="t1"/> has name key
        /// </item>
        /// <item>
        /// The corresponding property of <paramref name="t2"/> is required and has name value
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="t1">The first type</param>
        /// <param name="t2">The second type</param>
        /// <returns>The dictionnary</returns>
        private static Dictionary<string, string> PropNamesForRelationWithTWithRequired(Type t1, Type t2)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            int jchecked = -1;
            for (int i = 0; i < t1.GetProperties().Length; i++)
            {
                if (t1.GetProperties()[i].PropertyType == t2 || t1.GetProperties()[i].PropertyType == typeof(IList<>).MakeGenericType(t2))
                {
                    for (int j = jchecked + 1; j < t2.GetProperties().Length; j++)
                    {
                        if (t2.GetProperties()[j].PropertyType == t1 || t2.GetProperties()[j].PropertyType == typeof(IList<>).MakeGenericType(t1))
                        {
                            if (t2.GetProperties()[j].GetCustomAttribute(typeof(RequiredAttribute), false) != null)
                            {
                                res.Add(t1.GetProperties()[i].Name, t2.GetProperties()[j].Name);
                            }
                            jchecked = j;
                            break;
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Prepares update for object <paramref name="t"/> of type <typeparamref name="T"/>. Every type <paramref name="q"/> in 
        /// <see cref="GetTypesInRelationWithTHavingRequiredTProperty"/> has to be updated
        /// manually. Indeed, required properties are not handled well in EF in case of relationships, especially if they are
        /// of type <see cref="IList"/> (an empty <see cref="List"/> is not <see langword="null"/> and the annotation
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
        public static object[] PrepareUpdate<T>(T t)
        {
            foreach (Type type in GetTypesInRelationWithTHavingRequiredTProperty<T>())
            {
                Dictionary<string, string> propnames = PropNamesForRelationWithTWithRequired(typeof(T), type);
                if (propnames.Count() != 0)
                {
                    for (int j = 0; j < propnames.Count(); j++)
                    {
                        UpdateOtherPropInRelationWithTHavingRequiredTProperty<T>(type, t, propnames.Keys.ToList()[j], propnames[propnames.Keys.ToList()[j]]);
                    }
                }
            }
            IEnumerable<Type> TypesForWhichTHasManyProperties = GetTypesForWhichTHasManyProperties<T>();
            Dictionary<string, Type> dynamicDBTypes = DynamicDBTypes<T>();
            Dictionary<string, Type> dynamicDBListTypes = DynamicDBListTypes<T>();
            object[] res = new object[dynamicDBTypes.Count + dynamicDBListTypes.Count];
            var i = 0;
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (DynamicDBTypes<T>().Keys.Contains(prop.Name))
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
                    if (DynamicDBListTypes<T>().Keys.Contains(prop.Name))
                    {
                        if (TypesForWhichTHasManyProperties.Contains(DynamicDBListTypes<T>()[prop.Name]))
                        {
                            if (prop.GetValue(t) == null || (prop.GetValue(t) as IList).Count == 0)
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
        /// <see cref="GetTypesInRelationWithTHavingRequiredTProperty"/> has to be updated
        /// manually. Indeed, required properties are not handled well in EF in case of relationships, especially if they are
        /// of type <see cref="IList"/> (an empty <see cref="List"/> is not <see langword="null"/> and the annotation
        /// <see cref="RequiredAttribute"/> is interpreted as nullable = <see langword="false"/>).
        /// </summary>
        /// <remarks>
        /// Only the property of <paramref name="t"/> with name <paramref name="propertyName"/> will be updated.
        /// </remarks>
        /// <typeparam name="T">The type of the element to update</typeparam>
        /// <param name="t">The element to update</param>
        public static void PrepareUpdateOne<T>(T t, string propertyName)
        {
            foreach (Type type in GetTypesInRelationWithTHavingRequiredTProperty<T>())
            {
                Dictionary<string, string> propnames = PropNamesForRelationWithTWithRequired(typeof(T), type);
                if (propnames.Count() != 0)
                {
                    for (int j = 0; j < propnames.Count(); j++)
                    {
                        if (propnames.Keys.ToList()[j] == propertyName)
                            UpdateOtherPropInRelationWithTHavingRequiredTProperty(type, t, propnames.Keys.ToList()[j], propnames[propnames.Keys.ToList()[j]]);
                    }
                }
            }
        }
    }
}