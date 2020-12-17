using GenericRepositoryAndService.Exceptions;
using GenericRepositoryAndService.Models;
using GenericRepositoryAndService.Tools.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GenericRepositoryAndService.Repository
{
    /// <summary>
    /// Generic Repository for class <typeparamref name="T"/> using context 
    /// type <see cref="DbContext"/>.
    /// <remark>
    /// Assumes every class that either derives from <see cref="BaseEntity"/> 
    /// or has at least one property with annotation <see cref="KeyAttribute"/> 
    /// has a <see cref="DbSet"/> in <see cref="DbContext"/>.
    /// <br/>
    /// And that reciprocally, every class having a <see cref="DbSet"/> in 
    /// <see cref="DbContext"/> either derives from <see cref="BaseEntity"/>
    /// or has at least one property with annotation <see cref="KeyAttribute"/>.
    /// </remark>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        internal DbContext DataContext;
        protected DbSet<T> dbSet;

        /// <summary>
        /// An object <c>obj</c> of class <typeparamref name="T"/> has properties <c>obj.PropName</c> of
        /// class <see cref="IList"/>&lt;<c>ClassType</c>&gt; where <c>ClassType</c> is in a <see cref="DbSet"/> of the generic repository 
        /// <see cref="DataContext"/>. 
        /// <br/>
        /// This is every { PropName : ClassType }
        /// </summary>
        private readonly Dictionary<string, Type> _DynamicDBListTypes;

        /// <summary>
        /// An object <c>obj</c> of class <typeparamref name="T"/> has properties <c>obj.PropName</c> of
        /// class <c>ClassType</c> which is in a <see cref="DbSet"/> of the generic repository 
        /// <see cref="DataContext"/>. 
        /// <br/>
        /// This is every { PropName : ClassType }
        /// </summary>
        private readonly Dictionary<string, Type> _DynamicDBTypes;

        public GenericRepository(DbContext dataContext)
        {
            DataContext = dataContext;
            dbSet = DataContext.Set<T>();
            _DynamicDBListTypes = GenericToolsTypeAnalysis.DynamicDBListTypes<T>();
            _DynamicDBTypes = GenericToolsTypeAnalysis.DynamicDBTypes<T>();
        }

        /// <summary>
        /// Custom class to handle types in a relationship with <typeparamref name="T"/>
        /// <br/>
        /// Specifically to store the type of another class in a many-to-many or one-to-many or one-to-one
        /// relationship with <typeparamref name="T"/>
        /// <br/>
        /// <para>i.e. store the type <see cref="TypeofElement"/> such that :
        /// <list type="bullet">
        /// <item>
        /// <description>the class <typeparamref name="T"/> has a property of type either:
        /// <list type="bullet">
        /// <item>
        /// <description><see cref="TypeofElement"/></description>
        /// </item>
        /// <item>
        /// <description>or <see cref="IList"/>&lt;<see cref="TypeofElement"/>&gt;</description>
        /// </item>
        /// </list></description>
        /// </item>
        /// <item>
        /// <description><see cref="TypeofElement"/> is in a <see cref="DbSet"/> of the generic repository <see cref="DataContext"/></description>
        /// </item>
        /// </list> </para>
        /// <remark>
        /// Code could be refactored and dismiss this class, since <see cref="GenericToolsTypeAnalysis.TryListOfWhat(Type, out Type)"/> does the job <br/>
        /// I didn't know it was possible when I coded the handling of relationships  and found out about that possibility when I was about to finish
        /// a huge chunk of this code
        /// </remark>
        /// </summary>
        private class CustomParam
        {
            ///<summary> 
            ///The value of the property of <typeparamref name="T"/>
            ///</summary>
            public object Value { get; set; }
            /// <summary>
            /// The property of <typeparamref name="T"/> is of type <see cref="IList"/>&lt;<see cref="TypeofElement"/>&gt;
            /// </summary>
            public Type TypeofElement { get; set; }
            /// <summary>
            /// The name of the property. The property <see cref="Prop"/> of an object <c>obj</c> of class <typeparamref name="T"/> is <c>obj.PropertyName</c> 
            /// </summary>
            public string PropertyName { get; set; }
            /// <summary>
            /// The property
            /// </summary>
            public PropertyInfo Prop { get; set; }
            /// <summary>
            /// boolean to indicate if the property <see cref="Prop"/> of class <typeparamref name="T"/> is a <see cref="IList"/> or not.
            /// </summary>
            public bool IsList { get; set; }

            /// <summary>
            /// Construc a new CustomParam, storing type information for properties representing relationships in DB with <typeparamref name="T"/>.
            /// </summary>
            /// <param name="value">The value of the property of <typeparamref name="T"/></param>
            /// <param name="typeofElement">The property of <typeparamref name="T"/> is of type <see cref="IList"/>&lt;<see cref="TypeofElement"/>&gt;</param>
            /// <param name="propertyName">The name of the property. The property <see cref="Prop"/> of an object <c>obj</c> of class <typeparamref name="T"/> is <c>obj.PropertyName</c> </param>
            /// <param name="prop">The property</param>
            /// <param name="isList">boolean to indicate if the property <see cref="Prop"/> of class <typeparamref name="T"/> is a <see cref="IList"/> or not.</param>
            public CustomParam(object value, Type typeofElement, string propertyName, PropertyInfo prop, bool isList)
            {
                Value = value;
                TypeofElement = typeofElement;
                PropertyName = propertyName;
                Prop = prop;
                IsList = isList;
            }
        }

        /// <summary>
        /// Adds an element <paramref name="t"/> in DB of type <typeparamref name="T"/>
        /// <br/>
        /// Throws exception <see cref="CascadeCreationInDBException"/> if <typeparamref name="T"/> is in a relationship with a class in a <see cref="DbSet"/> of <see cref="DataContext"/>. 
        /// These elements could be dublicated in DB otherwise, they could be loaded in the context.
        /// </summary>
        /// <param name="t">Element to add</param>
        /// <exception cref="CascadeCreationInDBException"/>
        public void Add(T t)
        {
            if (GenericToolsTypeAnalysis.HasDynamicDBTypeOrListType<T>())
                throw new CascadeCreationInDBException(typeof(T));
            dbSet.Add(t);
        }

        /// <summary>
        /// Get the IQueryable collection. Specify if all other types in relationship with <typeparamref name="T"/>
        /// have to be included in the query, and if the elements have to be tracked.
        /// </summary>
        /// <param name="isIncludes">Whether or not other types in relationship with <typeparamref name="T"/>
        /// have to be included in the query</param>
        /// <param name="isTracked">Whether or not elements have to be tracked</param>
        /// <returns>The IQueryable collection</returns>
        public IQueryable<T> Collection(bool isIncludes, bool isTracked)
        {
            if (isIncludes)
            {
                if (isTracked)
                {
                    return GenericToolsQueriesAndLists.QueryTIncludeTracked<T>(DataContext);
                }
                else
                {
                    return GenericToolsQueriesAndLists.QueryTInclude<T>(DataContext);
                }
            }
            else
            {
                if (isTracked)
                {
                    return dbSet;
                }
                else
                {
                    return dbSet.AsNoTracking();
                }
            }
        }

        /// <summary>
        /// Get the IQueryable collection, other types in relationship with <typeparamref name="T"/> excluded, elements not tracked.
        /// </summary>
        /// <returns>The query</returns>
        public IQueryable<T> CollectionExcludes()
        {
            return Collection(false, false);
        }

        /// <summary>
        /// Get the IQueryable collection, other types in relationship with <typeparamref name="T"/> excluded, elements tracked.
        /// </summary>
        /// <returns>The query</returns>
        public IQueryable<T> CollectionExcludesTracked()
        {
            return Collection(false, true);
        }

        /// <summary>
        /// Get the IQueryable collection, other types in relationship with <typeparamref name="T"/> included, elements not tracked.
        /// </summary>
        /// <returns>The query</returns>
        public IQueryable<T> CollectionIncludes()
        {
            return Collection(true, false);
        }

        /// <summary>
        /// Get the IQueryable collection, other types in relationship with <typeparamref name="T"/> included, elements tracked.
        /// </summary>
        /// <returns>The query</returns>
        public IQueryable<T> CollectionIncludesTracked()
        {
            return Collection(true, true);
        }

        /// <summary>
        /// Commit the changes in DB
        /// </summary>
        public void Commit()
        {
            DataContext.SaveChanges();
        }

        /// <summary>
        /// Counts the elements in DB for which the predicate <paramref name="predicateWhere"/> is <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </remarks>
        /// <param name="predicateWhere"></param>
        /// <returns>The number of elements in DB satisfying <paramref name="predicateWhere"/></returns>
        public long Count(Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req = CollectionIncludes();

            if (predicateWhere != null)
                req = GenericToolsQueriesAndLists.QueryTryPredicateWhere(req, predicateWhere);

            return req.Count();
        }

        /// <summary>
        /// Deletes an object from DB having
        /// <list type="bullet">
        /// <item>
        /// either a specific Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or have specific key values otherwise.
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="objs">Either the Id of the object to delete, or its keys values.</param>
        /// <exception cref="InvalidKeyForClassException"/>
        public void Delete(params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            Remove(objs);
            Commit();
        }

        /// <summary>
        /// Deletes a specific object <paramref name="t"/> of type <typeparamref name="T"/> from DB
        /// </summary>
        /// <param name="t">The object to delete</param>
        public void Delete(T t)
        {
            Remove(t);
            Commit();
        }

        /// <summary>
        /// Finds an object from DB having
        /// <list type="bullet">
        /// <item>
        /// either a specific Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or have specific key values otherwise.
        /// </item>
        /// </list>
        /// Specify if all other types in relationship with <typeparamref name="T"/>
        /// have to be included in the query, and if the elements have to be tracked
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="isIncludes">Whether or not other types in relationship with <typeparamref name="T"/>
        /// have to be included in the query</param>
        /// <param name="isTracked">Whether or not elements have to be tracked</param>
        /// <param name="objs">Either the Id of the object to find, or its keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public T FindById(bool isIncludes, bool isTracked, params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            return GenericToolsQueriesAndLists.QueryWhereKeysAre(
                                                                    Collection(isIncludes, isTracked), 
                                                                    objs
                                                                 ).SingleOrDefault();
        }

        /// <summary>
        /// Finds an object from DB having
        /// <list type="bullet">
        /// <item>
        /// either a specific Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or have specific key values otherwise.
        /// </item>
        /// </list>
        /// Other types in relationship with <typeparamref name="T"/> excluded, elements not tracked.
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="objs">Either the Id of the object to find, or its keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public T FindByIdExcludes(params object[] objs)
        {
            return FindById(false, false, objs);
        }

        /// <summary>
        /// Finds an object from DB having
        /// <list type="bullet">
        /// <item>
        /// either a specific Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or have specific key values otherwise.
        /// </item>
        /// </list>
        /// Other types in relationship with <typeparamref name="T"/> excluded, elements tracked.
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="objs">Either the Id of the object to find, or its keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public T FindByIdExcludesTracked(params object[] objs)
        {
            return FindById(false, true, objs);
        }

        /// <summary>
        /// Finds an object from DB having
        /// <list type="bullet">
        /// <item>
        /// either a specific Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or have specific key values otherwise.
        /// </item>
        /// </list>
        /// Other types in relationship with <typeparamref name="T"/> included, elements not tracked.
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="objs">Either the Id of the object to find, or its keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public T FindByIdIncludes(params object[] objs)
        {
            return FindById(true, false, objs);
        }

        /// <summary>
        /// Finds an object from DB having
        /// <list type="bullet">
        /// <item>
        /// either a specific Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or have specific key values otherwise.
        /// </item>
        /// </list>
        /// Other types in relationship with <typeparamref name="T"/> included, elements tracked.
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="objs">Either the Id of the object to find, or its keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public T FindByIdIncludesTracked(params object[] objs)
        {
            return FindById(true, true, objs);
        }

        /// <summary>
        /// Get a list of elements following condition <paramref name="predicateWhere"/>.
        /// <br/>
        /// Every other property will be excluded if and only if <paramref name="isIncludes"/> is <see langword="true"/>,
        /// otherwise every other property will be included.
        /// <br/>
        /// Elements will be tracked if and only if <paramref name="isTracked"/> is <see langword="true"/>.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="isIncludes">Will all other properties be included</param>
        /// <param name="isTracked">Will the element be tracked</param>
        /// <param name="predicateWhere">Condition/param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllBy(bool isIncludes, bool isTracked, Expression<Func<T, bool>> predicateWhere)
        {
            return GetAll(isIncludes, isTracked, 0, int.MaxValue, null, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements following condition <paramref name="predicateWhere"/>.
        /// <br/>
        /// Every other property will be included, elements will not be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllByIncludes(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(true, false, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements following condition <paramref name="predicateWhere"/>.
        /// <br/>
        /// Every other property will be included, elements will be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllByIncludesTracked(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(true, true, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements following condition <paramref name="predicateWhere"/>.
        /// <br/>
        /// Every other property will be excluded, elements will not be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllByExcludes(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(false, false, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements following condition <paramref name="predicateWhere"/>.
        /// <br/>
        /// Every other property will be excluded, elements will be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllByExcludesTracked(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(false, true, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// starting at index <paramref name="start"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be excluded if and only if <paramref name="isIncludes"/> is <see langword="true"/>,
        /// otherwise every other property will be included.
        /// <br/>
        /// Elements will be tracked if and only if <paramref name="isTracked"/> is <see langword="true"/>.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="isIncludes">Will all other properties be included</param>
        /// <param name="isTracked">Will the element be tracked</param>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAll(bool isIncludes, bool isTracked, int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req;
            IQueryable<T> reqorigin = Collection(isIncludes, isTracked);

            if (orderreq != null)
            {
                req = orderreq.Compile().Invoke(reqorigin);
            }
            else
            {
                req = GenericToolsQueriesAndLists.QueryDefaultOrderBy(reqorigin);

            }

            req = GenericToolsQueriesAndLists.WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// starting at index <paramref name="start"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be excluded, elements will not be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllExcludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(false, false, start, maxByPage, orderreq, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// starting at index <paramref name="start"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be excluded, elements will be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllExcludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(false, true, start, maxByPage, orderreq, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// starting at index <paramref name="start"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be included, elements will not be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllIncludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(true, false, start, maxByPage, orderreq, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// starting at index <paramref name="start"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be included, elements will be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllIncludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(true, true, start, maxByPage, orderreq, predicateWhere);
        }

        /// <summary>
        /// Get the collection as a <see cref="List{T}"/>. Specify if all other types in relationship with <typeparamref name="T"/>
        /// have to be included in the query, and if the elements have to be tracked.
        /// </summary>
        /// <param name="isIncludes">Whether or not other types in relationship with <typeparamref name="T"/>
        /// have to be included in the query</param>
        /// <param name="isTracked">Whether or not elements have to be tracked</param>
        /// <returns>The list</returns>
        public List<T> List(bool isIncludes, bool isTracked)
        {
            return Collection(isIncludes, isTracked).ToList();
        }

        /// <summary>
        /// Get the collection as a <see cref="List{T}"/>, other types in relationship with <typeparamref name="T"/> excluded, elements not tracked.
        /// </summary>
        /// <returns>The list</returns>
        public List<T> ListExcludes()
        {
            return List(false, false);
        }

        /// <summary>
        /// Get the collection as a <see cref="List{T}"/>, other types in relationship with <typeparamref name="T"/> excluded, elements tracked.
        /// </summary>
        /// <returns>The list</returns>
        public List<T> ListExcludesTracked()
        {
            return List(false, true);
        }

        /// <summary>
        /// Get the collection as a <see cref="List{T}"/>, other types in relationship with <typeparamref name="T"/> included, elements not tracked.
        /// </summary>
        /// <returns>The list</returns>
        public List<T> ListIncludes()
        {
            return List(true, false);
        }

        /// <summary>
        /// Get the collection as a <see cref="List{T}"/>, other types in relationship with <typeparamref name="T"/> included, elements tracked.
        /// </summary>
        /// <returns>The list</returns>
        public List<T> ListIncludesTracked()
        {
            return List(true, true);
        }

        /// <summary>
        /// Modifies an element <paramref name="t"/> in DB of type <typeparamref name="T"/>
        /// <br/>
        /// Throws exception <see cref="CascadeCreationInDBException"/> if <typeparamref name="T"/> is in a relationship with a class in a <see cref="DbSet"/> of <see cref="DataContext"/>. 
        /// These elements could be dublicated in DB otherwise, since they could be loaded in the context.
        /// </summary>
        /// <param name="t">Element to modify</param>
        /// <exception cref="CascadeCreationInDBException"/>
        public void Modify(T t)
        {
            if (GenericToolsTypeAnalysis.HasDynamicDBTypeOrListType<T>())
                throw new CascadeCreationInDBException(typeof(T));
            if (DataContext.Entry(t).State == EntityState.Detached)
            {
                dbSet.Attach(t);
            }
            DataContext.Entry(t).State = EntityState.Modified;
        }

        /// <summary>
        /// Removes an object from DB (without committing) having
        /// <list type="bullet">
        /// <item>
        /// either a specific Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or have specific key values otherwise.
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="objs">Either the Id of the object to delete, or its keys values.</param>
        /// <exception cref="InvalidKeyForClassException"/>
        public void Remove(params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            Remove(FindByIdIncludes(objs));
        }

        /// <summary>
        /// Removes a specific object <paramref name="t"/> of type <typeparamref name="T"/> from DB (without comitting)
        /// </summary>
        /// <param name="t">The object to delete</param>
        public void Remove(T t)
        {
            if (DataContext.Entry(t).State == EntityState.Detached)
            {
                dbSet.Attach(t);
            }
            dbSet.Remove(t);
        }

        /// <summary>
        /// Saves an element <paramref name="t"/> in DB of type <typeparamref name="T"/>.
        /// <br/>
        /// <remark><paramref name="objs"/> are properties of <typeparamref name="T"/> in a relationship
        /// with <typeparamref name="T"/>.</remark>
        /// <br/>
        /// <remark>Objects not mentionned will be set to either <see langword="null"/> or <see langword="new"/> <c>List&lt;Class&gt;()</c></remark>
        /// <br/>
        /// Objects in <paramref name="objs"/> with values <see langword="null"/> will be ignored.
        /// <br/>
        /// Order is not important, unless properties are of the same type. In that case, they will be assigned
        /// in the same order as they are declared in the class <typeparamref name="T"/>.
        /// <br/>
        /// Properties can be forced to either <see langword="null"/> or <see langword="new"/> <c>List&lt;Class&gt;()</c> by having the set in <paramref name="objs"/>
        /// as a type <see cref="PropToNull"/> with <see cref="PropToNull.PropertyName"/> set to the name
        /// of the property. Usefull if <typeparamref name="T"/> is in many relationships with the same type. 
        /// See exemple for more information.
        /// <br/>
        /// <example>Exemple : assume T is a class deriving from <see cref="BaseEntity"/> with properties 
        /// <list type="bullet">
        /// <item>S propS</item>
        /// <item>Q propQ1</item>
        /// <item>Q propQ2</item>
        /// <item>R propR</item>
        /// </list>
        /// where Q,R and S are other types in DB. Say you want to setup the <see cref="CustomParam"/> for the following values :
        /// <br/>
        /// propS = <see langword="null"/>, propQ1 = <see langword="null"/>, propQ2 = VARQ, propR = VARR. To do so, call :
        /// <code>
        /// Save(<see langword="new"/> PropToNull("propQ1"), VARQ , VARR)
        /// </code>
        /// Reason and purpose : <see langword="null"/> values are ignored (since they could be assigned to any DB 
        /// type a priori, leading to ambiguity if some properties values are not specified)
        /// and in the case of many properties of the same type, the order set in the definition of the 
        /// class <typeparamref name="T"/> has to be respected. Thus, doing either
        /// <c>Save(<see langword="null"/>, VARQ, VARR)</c> or <c>Save(VARQ, VARR)</c> would result in setting :
        /// <br/>
        /// propS = <see langword="null"/>, propQ1 = VARQ, propQ2 = <see langword="null"/>, propR = VARR
        /// <br/>
        /// which is not what was wanted. <see cref="PropToNull"/> is usefull only for that specific case.
        /// </example>
        /// </summary>
        /// <param name="t">The object to update</param>
        /// <param name="objs">Objects that are properties of the object <paramref name="t"/> and that
        /// are in relationship with the type <typeparamref name="T"/>. 
        /// <br/>
        /// <remark><paramref name="objs"/> are properties of <typeparamref name="T"/> in a relationship
        /// with <typeparamref name="T"/>.</remark>
        /// <br/>
        /// <remark>Objects not mentionned will be set to either <see langword="null"/> or <see langword="new"/> <c>List&lt;Class&gt;()</c>.</remark>
        /// </param>
        /// <exception cref="InvalidArgumentsForClassException"/>
        /// <exception cref="CascadeCreationInDBException" />
        /// <exception cref="InvalidKeyForClassException"/>
        public void Save(T t, params object[] objs)
        {
            if (GenericToolsTypeAnalysis.HasDynamicDBTypeOrListType<T>())
            {
                CustomParam[] props = SetCustom(objs);
                SaveGeneric(t, props);
            }
            else
            {
                Add(t);
                Commit();
            }
        }

        /// <summary>
        /// Updates an element <paramref name="t"/> in DB of type <typeparamref name="T"/>.
        /// <br/>
        /// <remark><paramref name="objs"/> are properties of <typeparamref name="T"/> in a relationship
        /// with <typeparamref name="T"/>.</remark>
        /// <br/>
        /// <remark>Objects not mentionned will be set to either <see langword="null"/> or <see langword="new"/> <c>List&lt;Class&gt;()</c>.</remark>
        /// <br/>
        /// Objects in <paramref name="objs"/> with values <see langword="null"/> will be ignored.
        /// <br/>
        /// Order is not important, unless properties are of the same type. In that case, they will be assigned
        /// in the same order as they are declared in the class <typeparamref name="T"/>.
        /// <br/>
        /// Properties can be forced to either <see langword="null"/> or <see langword="new"/> <c>List&lt;Class&gt;()</c> by having the set in <paramref name="objs"/>
        /// as a type <see cref="PropToNull"/> with <see cref="PropToNull.PropertyName"/> set to the name
        /// of the property. Usefull if <typeparamref name="T"/> is in many relationships with the same type. 
        /// See exemple for more information.
        /// <br/>
        /// <example>Exemple : assume T is a class deriving from <see cref="BaseEntity"/> with properties 
        /// <list type="bullet">
        /// <item>S propS</item>
        /// <item>Q propQ1</item>
        /// <item>Q propQ2</item>
        /// <item>R propR</item>
        /// </list>
        /// where Q,R and S are other types in DB. Say you want to setup the <see cref="CustomParam"/> for the following values :
        /// <br/>
        /// propS = <see langword="null"/>, propQ1 = <see langword="null"/>, propQ2 = VARQ, propR = VARR. To do so, call :
        /// <code>
        /// Update(<see langword="new"/> PropToNull("propQ1"), VARQ , VARR)
        /// </code>
        /// Reason and purpose : <see langword="null"/> values are ignored (since they could be assigned to any DB 
        /// type a priori, leading to ambiguity if some properties values are not specified)
        /// and in the case of many properties of the same type, the order set in the definition of the 
        /// class <typeparamref name="T"/> has to be respected. Thus, doing either
        /// <c>Update(<see langword="null"/>, VARQ, VARR)</c> or <c>Update(VARQ, VARR)</c> would result in setting :
        /// <br/>
        /// propS = <see langword="null"/>, propQ1 = VARQ, propQ2 = <see langword="null"/>, propR = VARR
        /// <br/>
        /// which is not what was wanted. <see cref="PropToNull"/> is usefull only for that specific case.
        /// </example>
        /// </summary>
        /// <param name="t">The object to update</param>
        /// <param name="objs">Objects that are properties of the object <paramref name="t"/> and that
        /// are in relationship with the type <typeparamref name="T"/>. 
        /// <br/>
        /// <remark><paramref name="objs"/> are properties of <typeparamref name="T"/> in a relationship
        /// with <typeparamref name="T"/>.</remark>
        /// <br/>
        /// <remark>Objects not mentionned will be set to either <see langword="null"/> or <see langword="new"/> <c>List&lt;Class&gt;()</c>.</remark>
        /// </param>
        /// <exception cref="InvalidArgumentsForClassException"/>
        /// <exception cref="CascadeCreationInDBException" />
        /// <exception cref="InvalidKeyForClassException"/>
        public void Update(T t, params object[] objs)
        {
            if (GenericToolsTypeAnalysis.HasDynamicDBTypeOrListType<T>())
            {
                CustomParam[] props = SetCustom(objs);
                UpdateGeneric(DataContext, t, props);
            }
            else
            {
                Modify(t);
                Commit();
            }
        }

        /// <summary>
        /// Create a <see cref="CustomParam"/> with value <see langword="new"/> <c>List&lt;Class&gt;()</c> for the 
        /// property with key <paramref name="key"/> in <see cref="_DynamicDBListTypes"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The new <see cref="CustomParam"/></returns>
        private CustomParam CreateDefaultListCustomParamFromKey(string key)
        {
            return new CustomParam((IList)(Activator.CreateInstance(typeof(List<>).MakeGenericType(_DynamicDBListTypes[key]))),
                                    _DynamicDBListTypes[key],
                                    key,
                                    typeof(T).GetProperty(key),
                                    true
                                   );
        }

        /// <summary>
        /// Create a <see cref="CustomParam"/> with value <see langword="null"/> for the 
        /// property with key <paramref name="key"/> in <see cref="_DynamicDBTypes"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The new <see cref="CustomParam"/></returns>
        private CustomParam CreateDefaultCustomParamFromKey(string key)
        {
            return new CustomParam(null,
                                   _DynamicDBTypes[key],
                                   key,
                                   typeof(T).GetProperty(key),
                                   false
                                  );
        }

        /// <summary>
        /// Create a <see cref="CustomParam"/> with value <paramref name="obj"/> for the 
        /// property with key <paramref name="key"/> in <see cref="_DynamicDBListTypes"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The new <see cref="CustomParam"/></returns>
        private CustomParam CreateListCustomParamFromKey(string key, object obj)
        {
            return new CustomParam(obj,
                                    _DynamicDBListTypes[key],
                                    key,
                                    typeof(T).GetProperty(key),
                                    true
                                   );
        }

        /// <summary>
        /// Create a <see cref="CustomParam"/> with value <paramref name="obj"/> for the 
        /// property with key <paramref name="key"/> in <see cref="_DynamicDBTypes"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The new <see cref="CustomParam"/></returns>
        private CustomParam CreateCustomParamFromKey(string key, object obj)
        {
            return new CustomParam(obj,
                                   _DynamicDBTypes[key],
                                   key,
                                   typeof(T).GetProperty(key),
                                   false
                                  );
        }

        /// <summary>
        /// Setup for <see cref="CustomParam"/>.
        /// <br/>
        /// If an object corresponding to a property representing a relationship involving <typeparamref name="T"/> is in <paramref name="objs"/>
        /// construct a new <see cref="CustomParam"/> corresponding to that property.
        /// <br/>
        /// Once every <see cref="CustomParam"/> is constructed, construct default param (with value either <see langword="null"/> or <see langword="new"/> <c>List&lt;Class&gt;()</c>)
        /// for every other property that was not included in <paramref name="objs"/>.
        /// <br/>
        /// Objects in <paramref name="objs"/> with values <see langword="null"/> will be ignored.
        /// <br/>
        /// Order is not important, unless properties are of the same type. In that case, they will be assigned
        /// in the same order as they are declared in the class <typeparamref name="T"/>.
        /// <br/>
        /// Properties can be forced to either <see langword="null"/> or <see langword="new"/> <c>List&lt;Class&gt;()</c> by having the set in <paramref name="objs"/>
        /// as a type <see cref="PropToNull"/> with <see cref="PropToNull.PropertyName"/> set to the name
        /// of the property. Usefull if <typeparamref name="T"/> is in many relationships with the same type. 
        /// See exemple for more information.
        /// <br/>
        /// <example>Exemple : assume T is a class deriving from <see cref="BaseEntity"/> with properties 
        /// <list type="bullet">
        /// <item>S propS</item>
        /// <item>Q propQ1</item>
        /// <item>Q propQ2</item>
        /// <item>R propR</item>
        /// </list>
        /// where Q,R and S are other types in DB. Say you want to setup the <see cref="CustomParam"/> for the following values :
        /// <br/>
        /// propS = <see langword="null"/>, propQ1 = <see langword="null"/>, propQ2 = VARQ, propR = VARR. To do so, call :
        /// <code>
        /// SetCustom(<see langword="new"/> PropToNull("propQ1"), VARQ , VARR)
        /// </code>
        /// Reason and purpose : <see langword="null"/> values are ignored (since they could be assigned to any DB 
        /// type a priori, leading to ambiguity if some properties values are not specified)
        /// and in the case of many properties of the same type, the order set in the definition of the 
        /// class <typeparamref name="T"/> has to be respected. Thus, doing either
        /// <c>SetCustom(<see langword="null"/>, VARQ, VARR)</c> or <c>SetCustom(VARQ, VARR)</c> would result in setting :
        /// <br/>
        /// propS = <see langword="null"/>, propQ1 = VARQ, propQ2 = <see langword="null"/>, propR = VARR
        /// <br/>
        /// which is not what was wanted. <see cref="PropToNull"/> is usefull only for that specific case.
        /// </example>
        /// </summary>
        /// <param name="objs">List of objets for which to set up <see cref="CustomParam"/></param>
        /// <returns>The complete list of <see cref="CustomParam"/> with every property representing a 
        /// relationship involving <typeparamref name="T"/> covered.</returns>
        /// <exception cref="InvalidArgumentsForClassException"/>
        private CustomParam[] SetCustom(params object[] objs)
        {
            CustomParam[] res = new CustomParam[_DynamicDBListTypes.Count() + _DynamicDBTypes.Count()];
            int resindex = 0;

            List<string> lkeysforlisttypes = _DynamicDBListTypes.Keys.ToList();
            List<Type> ltypesforlisttpes = _DynamicDBListTypes.Values.ToList();
            List<Type> lTlisttype = _DynamicDBListTypes.Values.Select(typ => 
                                                                        typeof(List<>).MakeGenericType(typ)
                                                                      ).ToList();

            List<string> lkeysfortypes = _DynamicDBTypes.Keys.ToList();
            List<Type> lTtypes = _DynamicDBTypes.Values.ToList();

            foreach (object obj in objs.Where(o => o != null))
            {
                bool isFound = false;
                int i = 0;
                if (obj is PropToNull proptonull)
                {
                    isFound = true;
                    Type typeofprop = typeof(T).GetProperty(proptonull.PropertyName).PropertyType;
                    if (GenericToolsTypeAnalysis.TryListOfWhat(typeofprop, out Type innertype))
                    {
                        res[resindex] = CreateDefaultListCustomParamFromKey(proptonull.PropertyName);
                        lkeysforlisttypes.Remove(proptonull.PropertyName);
                        ltypesforlisttpes.Remove(innertype);
                        lTlisttype.Remove(typeof(List<>).MakeGenericType(innertype));
                    }
                    else
                    {
                        res[resindex] = CreateDefaultCustomParamFromKey(proptonull.PropertyName);
                        lkeysfortypes.Remove(proptonull.PropertyName);
                        lTtypes.Remove(typeofprop);
                    }
                    resindex++;
                }
                else
                {
                    foreach (Type typ in lTlisttype)
                    {
                        try
                        {
                            var test = Convert.ChangeType(obj, typ);
                            isFound = true;
                            res[resindex] = CreateListCustomParamFromKey(lkeysforlisttypes[i], obj);
                            resindex++;
                            lkeysforlisttypes.RemoveAt(i);
                            ltypesforlisttpes.RemoveAt(i);
                            lTlisttype.RemoveAt(i);
                            break;
                        }
                        catch { }
                        i++;
                    }
                    i = 0;
                    if (!isFound)
                    {
                        foreach (Type typ in lTtypes)
                        {
                            try
                            {
                                var test = Convert.ChangeType(obj, typ);
                                isFound = true;
                                res[resindex] = CreateCustomParamFromKey(lkeysfortypes[i], obj);
                                resindex++;
                                lkeysfortypes.RemoveAt(i);
                                lTtypes.RemoveAt(i);
                                break;
                            }
                            catch { }
                            i++;
                        }
                    }
                }
                if (!isFound)
                    throw new InvalidArgumentsForClassException(typeof(T));
            }

            foreach (string key in lkeysforlisttypes)
            {
                res[resindex] = CreateDefaultListCustomParamFromKey(key);
                resindex++;
            }

            foreach (string key in lkeysfortypes)
            {
                res[resindex] = CreateDefaultCustomParamFromKey(key);
                resindex++;
            }

            return res;
        }

        /// <summary>
        /// Setup a new parameter <see cref="CustomParam"/>. The only difference with <paramref name="customParam"/>
        /// is that the value is loaded from an other context <paramref name="newContext"/>
        /// <remark>The property is of type <see cref="IList"/></remark>
        /// </summary>
        /// <param name="newContext">New context</param>
        /// <param name="customParam">Parameter from old context</param>
        /// <returns>The new parameter <see cref="CustomParam"/> from context <paramref name="newContext"/></returns>
        /// <exception cref="InvalidKeyForClassException"/>
        private CustomParam SetNewParamFromContextList(DbContext newContext, CustomParam customParam)
        {
            var newvalue = Convert.ChangeType(
                                                Activator.CreateInstance(typeof(List<>).MakeGenericType(customParam.TypeofElement)), 
                                                typeof(List<>).MakeGenericType(customParam.TypeofElement)
                                             );
            if (customParam.Value != null)
                foreach (object item in customParam.Value as IList)
                {
                    object newitem;
                    if (item is BaseEntity entity)
                    {
                        newitem = newContext.Set(customParam.TypeofElement).Find(entity.Id);
                    }
                    else
                    {
                        object[] objs = GenericToolsTypeAnalysis.GetKeysValuesForType(item, customParam.TypeofElement);
                        newitem = GenericToolsCRUD.FindByKeysInNewContextForType(customParam.TypeofElement, newContext, objs);
                    }
                    ((IList)newvalue).Add(newitem);
                }
            return new CustomParam(
                                    newvalue,
                                    customParam.TypeofElement,
                                    customParam.PropertyName,
                                    customParam.Prop,
                                    true
                                  );
        }

        /// <summary>
        /// Setup a new parameter <see cref="CustomParam"/>. The only difference with <paramref name="customParam"/>
        /// is that the value is loaded from an other context <paramref name="newContext"/>
        /// <remark>The property is NOT of type <see cref="IList"/></remark>
        /// </summary>
        /// <param name="newContext">New context</param>
        /// <param name="customParam">Parameter from old context</param>
        /// <returns>The new parameter <see cref="CustomParam"/> from context <paramref name="newContext"/></returns>
        /// <exception cref="InvalidKeyForClassException"/>s
        private CustomParam SetNewParamFromContextNotList(DbContext newContext, CustomParam customParam)
        {
            object newvalue = null;
            if (customParam.Value != null)
            {
                if (customParam.Value is BaseEntity entity)
                {
                    newvalue = newContext.Set(customParam.TypeofElement).Find(entity.Id);
                }
                else
                {
                    object[] objs = GenericToolsTypeAnalysis.GetKeysValuesForType(customParam.Value, customParam.TypeofElement);
                    newvalue = GenericToolsCRUD.FindByKeysInNewContextForType(customParam.TypeofElement, newContext, objs);
                }
            }
            return new CustomParam(
                                    newvalue,
                                    customParam.TypeofElement,
                                    customParam.PropertyName,
                                    customParam.Prop,
                                    false
                                   );
        }

        /// <summary>
        /// SetUp new <see cref="CustomParam"/> for properties representing relationships involving <typeparamref name="T"/> using the context <paramref name="newContext"/>.
        /// <br/>
        /// <remark>
        /// Assumes every property representing a relationships involving <typeparamref name="T"/> has a corresponding <see cref="CustomParam"/> in <paramref name="propss"/>
        /// </remark>
        /// </summary>
        /// <param name="newContext">Context</param>
        /// <param name="props">Initial <see cref="CustomParam"/></param>
        /// <returns>New <see cref="CustomParam"/> from the context <paramref name="newContext"/></returns>
        /// <exception cref="InvalidKeyForClassException"/>
        private List<CustomParam> SetNewParamsFromContext(DbContext newContext, params CustomParam[] props)
        {
            List<CustomParam> newparams = new List<CustomParam>();

            foreach (CustomParam customParam in props)
            {
                if (customParam.IsList)
                {
                    newparams.Add(SetNewParamFromContextList(newContext, customParam));
                }
                else
                {
                    newparams.Add(SetNewParamFromContextNotList(newContext, customParam));
                }

            }
            return newparams;
        }

        /// <summary>
        /// Change every property (that does NOT represent a relationship involving <typeparamref name="T"/>) of <paramref name="t"/> with values from <paramref name="newt"/>
        /// <br/>
        /// <remark>
        /// Assumes every property representing a relationships involving <typeparamref name="T"/> has a corresponding <see cref="CustomParam"/> in <paramref name="props"/>
        /// </remark>
        /// </summary>
        /// <param name="t">The object to modify</param>
        /// <param name="newt">The object to get values from</param>
        /// <param name="props">The <see cref="CustomParam"/> representing properties representing relationships involving <typeparamref name="T"/></param>
        /// <returns>The object <paramref name="t"/> with properties not representing relationships involving <typeparamref name="T"/> having values from <paramref name="newt"/></returns>
        private T ModifyOtherProperties(T t, T newt, params CustomParam[] props)
        {
            T res = t;
            foreach (var p in typeof(T).GetProperties())
            {
                if (!props.Select(cp => cp.Prop).Contains(p) && 
                    p.CanWrite
                   )
                    p.SetValue(res, p.GetValue(newt));
            }
            return res;
        }

        /// <summary>
        /// Saves an object <paramref name="t"/> of class <typeparamref name="T"/> in DB.
        /// <br/>
        /// <remark>Assumes every property representing a relationships involving <typeparamref name="T"/> has a corresponding <see cref="CustomParam"/> in <paramref name="propss"/></remark>
        /// </summary>
        /// <param name="t">Object to update</param>
        /// <param name="propss"><see cref="CustomParam"/> for properties representing a relationship involving <typeparamref name="T"/>
        /// containing new values</param>
        /// <exception cref="InvalidKeyForClassException"/>
        private void SaveGeneric(T t, params CustomParam[] propss)
        {
            using (DbContext newContext = (DbContext)Activator.CreateInstance(DataContext.GetType()))
            {
                List<CustomParam> newparams = SetNewParamsFromContext(newContext, propss);

                foreach (CustomParam newparam in newparams)
                {
                    typeof(T).GetProperty(newparam.PropertyName).SetValue(t, newparam.Value);
                }

                foreach (var entry in newContext.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Modified;
                }

                newContext.Entry(t).State = EntityState.Added;

                newContext.SaveChanges();
            }
        }

        /// <summary>
        /// See <see cref="FindByIdIncludesTracked(object[])"/>. Does the same thing in a specific context
        /// <paramref name="dbContext"/>, ie
        /// <br/>
        /// finds an object from DB having
        /// <list type="bullet">
        /// <item>
        /// either a specific Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or have specific key values otherwise.
        /// </item>
        /// </list>
        /// Other types in relationship with <typeparamref name="T"/> included, elements tracked.
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="DbContext">The context from which the element has to be found</param>
        /// <param name="objs">Either the Id of the object to delete, or its keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        private T FindByIdIncludesTrackedInNewContext(DbContext dbContext, params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            return GenericToolsQueriesAndLists.QueryWhereKeysAre(
                                                                    GenericToolsQueriesAndLists.QueryTIncludeTracked<T>(dbContext), 
                                                                    objs
                                                                 ).SingleOrDefault();
        }

        /// <summary>
        /// See <see cref="FindByIdIncludes(object[])"/>. Does the same thing in a specific context
        /// <paramref name="dbContext"/>, ie
        /// <br/>
        /// finds an object from DB having
        /// <list type="bullet">
        /// <item>
        /// either a specific Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or have specific key values otherwise.
        /// </item>
        /// </list>
        /// Other types in relationship with <typeparamref name="T"/> included, elements not tracked.
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="dbContext">The context from which the element has to be found</param>
        /// <param name="objs">Either the Id of the object to delete, or its keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        private T FindByIdIncludesInNewContext(DbContext dbContext, params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            return GenericToolsQueriesAndLists.QueryWhereKeysAre(
                                                                    GenericToolsQueriesAndLists.QueryTInclude<T>(dbContext), 
                                                                    objs
                                                                ).SingleOrDefault();
        }

        /// <summary>
        /// Updates an object <paramref name="t"/> of class <typeparamref name="T"/> in DB.
        /// <br/>
        /// <remark>Assumes every property representing a relationships involving <typeparamref name="T"/> has a corresponding <see cref="CustomParam"/> in <paramref name="propss"/></remark>
        /// </summary>
        /// <param name="t">Object to update</param>
        /// <param name="propss"><see cref="CustomParam"/> for properties representing a relationship involving <typeparamref name="T"/>
        /// containing new values</param>
        /// <exception cref="InvalidKeyForClassException"/>
        private void UpdateGeneric(DbContext context, T t, params CustomParam[] propss)
        {
            using (DbContext newContext = (DbContext)Activator.CreateInstance(context.GetType()))
            {
                T tToChange = FindByIdIncludesTrackedInNewContext(newContext, GenericToolsTypeAnalysis.GetKeysValues(t));

                tToChange = ModifyOtherProperties(tToChange, t, propss);

                List<CustomParam> newparams = SetNewParamsFromContext(newContext, propss);

                foreach (CustomParam newparam in newparams)
                {
                    typeof(T).GetProperty(newparam.PropertyName).SetValue(tToChange, newparam.Value);
                }

                newContext.Entry(tToChange).State = EntityState.Modified;

                newContext.SaveChanges();
            }
        }

        /// <summary>
        /// Updates one specific property with name <paramref name="propertyName"/> with the value 
        /// <paramref name="newValue"/> for an object <paramref name="t"/> of class <typeparamref name="T"/> in DB.
        /// </summary>
        /// <param name="t">Object to update</param>
        /// <param name="propertyName">The name of the property to update</param>
        /// <param name="newValue">The new value</param>
        /// <exception cref="PropertyNameNotFoundException"/>
        /// <exception cref="CannotWriteReadOnlyPropertyException"/>
        /// <exception cref="InvalidArgumentsForClassException"/>
        /// <exception cref="InvalidKeyForClassException"/>
        public void UpdateOne(T t, string propertyName, object newValue)
        {
            using (DbContext newContext = (DbContext)Activator.CreateInstance(DataContext.GetType()))
            {
                T tToChange = FindByIdIncludesInNewContext(newContext, GenericToolsTypeAnalysis.GetKeysValues(t));

                PropertyInfo propToChange = typeof(T).GetProperty(propertyName);

                if (propToChange == null)
                    throw new PropertyNameNotFoundException(typeof(T), propertyName);

                if (!propToChange.CanWrite)
                    throw new CannotWriteReadOnlyPropertyException(typeof(T), propertyName);

                if (newValue != null && !propToChange.PropertyType.IsAssignableFrom(newValue.GetType()))
                    throw new InvalidArgumentsForClassException(typeof(T));

                typeof(T).GetProperty(propertyName).SetValue(tToChange, newValue);

                foreach (var entry in newContext.ChangeTracker.Entries())
                {
                    entry.State = EntityState.Detached;
                }

                newContext.SaveChanges();
            }
        }
    }
}