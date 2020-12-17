using GenericRepositoryAndService.Exceptions;
using GenericRepositoryAndService.Models;
using GenericRepositoryAndService.Tools.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GenericRepositoryAndService.Tools;
using System.ComponentModel.DataAnnotations;
using GenericRepositoryAndService.Repository;

namespace GenericRepositoryAndService.Service
{
    /// <summary>
    /// Generic Repository for class <typeparamref name="T"/> using context 
    /// type <see cref="DbContext"/>.
    /// <remark>
    /// Assumes that :
    /// <list type="bullet">
    /// <item>
    /// every class that either derives from <see cref="BaseEntity"/> 
    /// or has at least one property with annotation <see cref="KeyAttribute"/> 
    /// has a <see cref="DbSet"/> in <see cref="DbContext"/>
    /// </item>
    /// <item>
    /// and that reciprocally, every class having a <see cref="DbSet"/> in 
    /// <see cref="DbContext"/> either derives from <see cref="BaseEntity"/>
    /// or has at least one property with annotation <see cref="KeyAttribute"/>.
    /// </item>
    /// </list>
    /// Furthermore, assumes that 
    /// <list type="bullet">
    /// <item>
    /// For a class t with name "TName", the corresponding repository is named "TNameRepository"
    /// </item>
    /// <item>
    /// For a class t with name "TName", the corresponding service is named "TNameService"
    /// </item>
    /// </list>
    /// </remark>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
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
            return _repository.Collection(isIncludes, isTracked);
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
            return _repository.Count(predicateWhere);
        }

        /// <summary>
        /// Get a list of elements ordered by <see cref="OrderExpression"/> following condition 
        /// <see cref="SearchExpression(string)"/> applied to <paramref name="searchField"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be excluded if and only if <paramref name="isIncludes"/> is <see langword="true"/>,
        /// otherwise every other property will be included.
        /// <br/>
        /// Elements will be tracked if and only if <paramref name="isTracked"/> is <see langword="true"/>.
        /// <br/>
        /// If <see cref="OrderExpression"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="isIncludes">Will all other properties be included</param>
        /// <param name="isTracked">Will the element be tracked</param>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="searchField">The string with the search</param>
        /// <returns>The list of objects</returns>
        public List<T> FindAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return GetAll(isIncludes, isTracked, page, maxByPage, OrderExpression(), SearchExpression(searchField));
        }

        /// <summary>
        /// Get a list of elements ordered by <see cref="OrderExpression"/> following condition 
        /// <see cref="SearchExpression(string)"/> applied to <paramref name="searchField"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be excluded, elements will not be tracked.
        /// <br/>
        /// If <see cref="OrderExpression"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="searchField">The string with the search</param>
        /// <returns>The list of objects</returns>
        public List<T> FindAllExcludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return FindAll(false, false, page, maxByPage, searchField);
        }

        /// <summary>
        /// Get a list of elements ordered by <see cref="OrderExpression"/> following condition 
        /// <see cref="SearchExpression(string)"/> applied to <paramref name="searchField"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be excluded, elements will be tracked.
        /// <br/>
        /// If <see cref="OrderExpression"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="searchField">The string with the search</param>
        /// <returns>The list of objects</returns>
        public List<T> FindAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return FindAll(false, true, page, maxByPage, searchField);
        }

        /// <summary>
        /// Get a list of elements ordered by <see cref="OrderExpression"/> following condition 
        /// <see cref="SearchExpression(string)"/> applied to <paramref name="searchField"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be included, elements will not be tracked.
        /// <br/>
        /// If <see cref="OrderExpression"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="searchField">The string with the search</param>
        /// <returns>The list of objects</returns>
        public List<T> FindAllIncludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return FindAll(true, false, page, maxByPage, searchField);
        }

        /// <summary>
        /// Get a list of elements ordered by <see cref="OrderExpression"/> following condition 
        /// <see cref="SearchExpression(string)"/> applied to <paramref name="searchField"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be included, elements will be tracked.
        /// <br/>
        /// If <see cref="OrderExpression"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="searchField">The string with the search</param>
        /// <returns>The list of objects</returns>
        public List<T> FindAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return FindAll(true, true, page, maxByPage, searchField);
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
            return _repository.FindById(isIncludes, isTracked, objs);
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
        /// Finds a list of objects from DB having
        /// <list type="bullet">
        /// <item>
        /// either specific Ids, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or specific key values otherwise, if <typeparamref name="T"/> derives from <see cref="EntityWithKeys"/>.
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
        /// <param name="objs">Either the Id of the object to delete, or its keys values.</param>
        /// <returns>The list of elements.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public List<T> FindByManyId(bool isIncludes, bool isTracked, params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjsIsManyKeysOrIds<T>(objs);
            List<T> lst = new List<T>();
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                int?[] ids = GenericToolsTypeAnalysis.GetManyIds(objs);
                foreach (int? id in ids)
                {
                    if (!id.HasValue)
                        throw new IdNullForClassException(typeof(T));
                    lst.Add(_repository.FindById(isIncludes, isTracked, id.Value));
                }
                return lst;
            }
            else
            {
                object[][] objectskeys = GenericToolsTypeAnalysis.GetManyKeys<T>(objs);
                foreach (object[] keys in objectskeys)
                {
                    lst.Add(_repository.FindById(isIncludes, isTracked, keys));
                }
                return lst;
            }
        }

        /// <summary>
        /// Finds a list of objects from DB having
        /// <list type="bullet">
        /// <item>
        /// either specific Ids, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or specific key values otherwise, if <typeparamref name="T"/> derives from <see cref="EntityWithKeys"/>.
        /// </item>
        /// </list>
        /// Other types in relationship with <typeparamref name="T"/> excluded, elements not tracked.
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="objs">Either the Ids of the object to find, or their keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public List<T> FindManyByIdExcludes(params object[] objs)
        {
            return FindByManyId(false, false, objs);
        }

        /// <summary>
        /// Finds a list of objects from DB having
        /// <list type="bullet">
        /// <item>
        /// either specific Ids, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or specific key values otherwise, if <typeparamref name="T"/> derives from <see cref="EntityWithKeys"/>.
        /// </item>
        /// </list>
        /// Other types in relationship with <typeparamref name="T"/> excluded, elements tracked.
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="objs">Either the Ids of the object to find, or their keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public List<T> FindManyByIdExcludesTracked(params object[] objs)
        {
            return FindByManyId(false, true, objs);
        }

        /// <summary>
        /// Finds a list of objects from DB having
        /// <list type="bullet">
        /// <item>
        /// either specific Ids, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or specific key values otherwise, if <typeparamref name="T"/> derives from <see cref="EntityWithKeys"/>.
        /// </item>
        /// </list>
        /// Other types in relationship with <typeparamref name="T"/> included, elements not tracked.
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="objs">Either the Ids of the object to find, or their keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public List<T> FindManyByIdIncludes(params object[] objs)
        {
            return FindByManyId(true, false, objs);
        }

        /// <summary>
        /// Finds a list of objects from DB having
        /// <list type="bullet">
        /// <item>
        /// either specific Ids, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or specific key values otherwise, if <typeparamref name="T"/> derives from <see cref="EntityWithKeys"/>.
        /// </item>
        /// </list>
        /// Other types in relationship with <typeparamref name="T"/> included, elements tracked.
        /// </summary>
        /// <remarks>
        /// Keys have to be specified in the same order as they are declared in the class <typeparamref name="T"/>
        /// </remarks>
        /// <param name="objs">Either the Ids of the object to find, or their keys values.</param>
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public List<T> FindManyByIdIncludesTracked(params object[] objs)
        {
            return FindByManyId(true, true, objs);
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
            return _repository.GetAllBy(isIncludes, isTracked, predicateWhere);
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
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
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
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            int start = (page - 1) * maxByPage;
            return _repository.GetAll(isIncludes, isTracked, start, maxByPage, orderreq, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be excluded, elements will not be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllExcludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(false, false, page, maxByPage, orderreq, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be excluded, elements will be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(false, true, page, maxByPage, orderreq, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be included, elements will not be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllIncludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(true, false, page, maxByPage, orderreq, predicateWhere);
        }

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be included, elements will be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(true, true, page, maxByPage, orderreq, predicateWhere);
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
            return _repository.List(isIncludes, isTracked);
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
        /// Checks whether or not there is another page after <paramref name="page"/>
        /// for the search using <see cref="SearchExpression(string)"/> having <paramref name="maxByPage"/> 
        /// elements per page.
        /// </summary>
        /// <param name="page">The current page number.</param>
        /// <param name="maxByPage">The maximum number of elements per page.</param>
        /// <param name="searchField">The string searched.</param>
        /// <returns>Whether or not there is another page after page number <paramref name="page"/></returns>
        public bool NextExist(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return (page * maxByPage) < _repository.Count(SearchExpression(searchField));
        }

        /// <summary>
        /// The expression with which the elements will be ordered when using <see cref="FindAll"/>.
        /// </summary>
        /// <returns>The expression.</returns>
        public abstract Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> OrderExpression();

        /// <summary>
        /// The expression used to search the elements in <see cref="FindAll"/> and
        /// <see cref="NextExist(int, int, string)"/>.
        /// <br/>
        /// If <see cref="OrderExpression"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericToolsQueriesAndLists.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="searchField"></param>
        /// <returns></returns>
        public abstract Expression<Func<T, bool>> SearchExpression(string searchField = "");

        /// <summary>
        /// Deletes the element with either Id or keys <paramref name="objs"/>.
        /// </summary>
        /// <param name="objs">Either the Id or keys</param>
        public void Delete(params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            dynamic temprep = _repository;
            GenericToolsCRUD.PrepareDelete<T>(temprep.DataContext, objs);
            _repository.Delete(objs);
        }

        /// <summary>
        /// Deletes the element <paramref name="t"/>
        /// </summary>
        /// <param name="t">The element to delete</param>
        public void Delete(T t)
        {
            dynamic temprep = _repository;
            GenericToolsCRUD.PrepareDelete<T>(temprep.DataContext, GenericToolsTypeAnalysis.GetKeysValues(t));
            _repository.Delete(t);
        }

        /// <summary>
        /// Saves the element <paramref name="t"/>.
        /// </summary>
        /// <param name="t">The element to save</param>
        public void Save(T t)
        {
            object[] objs = GenericToolsCRUD.PrepareSave(t);
            _repository.Save(t, objs);
        }

        /// <summary>
        /// Saves the elements <paramref name="t"/> after having crypted using 
        /// <see cref="HashTools.ComputeSha256Hash(string)"/> every string property with annotation
        /// <see cref="DataTypeAttribute"/> with type <see cref="DataType.Password"/>.
        /// </summary>
        /// <param name="t">The element to crypt then save.</param>
        public void SaveCrypted(T t)
        {
            t = GenericToolsCRUDCrypt.Crypt(t);
            Save(t);
        }

        /// <summary>
        /// Updates an element <paramref name="t"/>
        /// </summary>
        /// <param name="t">The element to update</param>
        public void Update(T t)
        {
            dynamic temprep = _repository;
            object[] objs = GenericToolsCRUD.PrepareUpdate(temprep.DataContext, t);
            _repository.Update(t, objs);
        }

        /// <summary>
        /// Updates the element <paramref name="t"/> after having crypted using 
        /// <see cref="HashTools.ComputeSha256Hash(string)"/> every string property with annotation
        /// <see cref="DataTypeAttribute"/> with type <see cref="DataType.Password"/> that have changed.
        /// </summary>
        /// <param name="t">The object to update.</param>
        public void UpdateCrypted(T t)
        {
            T told = FindByIdExcludes(GenericToolsTypeAnalysis.GetKeysValues(t));
            t = GenericToolsCRUDCrypt.CryptIfUpdated(told, t);
            Update(t);
        }

        /// <summary>
        /// Update only the property with name <paramref name="propertyName"/> of the object with
        /// either same Id or keys as <paramref name="t"/> with value <paramref name="newValue"/>.
        /// </summary>
        /// <param name="t">The object to update</param>
        /// <param name="propertyName">The name of the property to update</param>
        /// <param name="newValue">The new value of the property with name <paramref name="propertyName"/></param>
        public void UpdateOne(T t, string propertyName, object newValue)
        {
            dynamic temprep = _repository;
            GenericToolsCRUD.PrepareUpdateOne(temprep.DataContext, t, propertyName);
            _repository.UpdateOne(t, propertyName, newValue);
        }

        /// <summary>
        /// Update only the property with name <paramref name="propertyName"/> of the object with
        /// either same Id or keys as <paramref name="t"/> with value <paramref name="newValue"/> after
        /// having crypted using <see cref="HashTools.ComputeSha256Hash(string)"/> if the property
        /// with name <paramref name="propertyName"/> has the annotation
        /// <see cref="DataTypeAttribute"/> with type <see cref="DataType.Password"/> and have changed
        /// </summary>
        /// <param name="t">The object to update</param>
        /// <param name="propertyName">The name of the property to update</param>
        /// <param name="newValue">The new value of the property with name <paramref name="propertyName"/></param>
        public void UpdateOneCrypted(T t, string propertyName, object newValue)
        {
            T told = FindByIdExcludes(GenericToolsTypeAnalysis.GetKeysValues(t));
            t = GenericToolsCRUDCrypt.CryptIfUpdatedOne(told, t, propertyName, newValue);
            UpdateOne(t, propertyName, newValue);
        }
    }
}