using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MiseEnSituation.Exceptions;
using MiseEnSituation.Tools;
using System.ComponentModel.DataAnnotations;

namespace MiseEnSituation.Services
{
    public interface IGenericService<T> where T : class
    {
        /// <summary>
        /// Get the IQueryable collection. Specify if all other types in relationship with <typeparamref name="T"/>
        /// have to be included in the query, and if the elements have to be tracked.
        /// </summary>
        /// <param name="isIncludes">Whether or not other types in relationship with <typeparamref name="T"/>
        /// have to be included in the query</param>
        /// <param name="isTracked">Whether or not elements have to be tracked</param>
        /// <returns>The IQueryable collection</returns>
        IQueryable<T> Collection(bool isIncludes, bool isTracked);

        /// <summary>
        /// Get the IQueryable collection, other types in relationship with <typeparamref name="T"/> excluded, elements not tracked.
        /// </summary>
        /// <returns>The query</returns>
        IQueryable<T> CollectionExcludes();

        /// <summary>
        /// Get the IQueryable collection, other types in relationship with <typeparamref name="T"/> excluded, elements tracked.
        /// </summary>
        /// <returns>The query</returns>
        IQueryable<T> CollectionExcludesTracked();

        /// <summary>
        /// Get the IQueryable collection, other types in relationship with <typeparamref name="T"/> included, elements not tracked.
        /// </summary>
        /// <returns>The query</returns>
        IQueryable<T> CollectionIncludes();

        /// <summary>
        /// Get the IQueryable collection, other types in relationship with <typeparamref name="T"/> included, elements tracked.
        /// </summary>
        /// <returns>The query</returns>
        IQueryable<T> CollectionIncludesTracked();

        /// <summary>
        /// Counts the elements in DB for which the predicate <paramref name="predicateWhere"/> is <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </remarks>
        /// <param name="predicateWhere"></param>
        /// <returns>The number of elements in DB satisfying <paramref name="predicateWhere"/></returns>
        long Count(Expression<Func<T, bool>> pedicateWhere = null);

        /// <summary>
        /// Deletes the element with either Id or keys <paramref name="objs"/>.
        /// </summary>
        /// <param name="objs">Either the Id or keys</param>
        void Delete(params object[] objs);

        /// <summary>
        /// Deletes the element <paramref name="t"/>
        /// </summary>
        /// <param name="t">The element to delete</param>
        void Delete(T t);

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
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="isIncludes">Will all other properties be included</param>
        /// <param name="isTracked">Will the element be tracked</param>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="searchField">The string with the search</param>
        /// <returns>The list of objects</returns>
        List<T> FindAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, string searchField = "");

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
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="searchField">The string with the search</param>
        /// <returns>The list of objects</returns>
        List<T> FindAllExcludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

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
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="searchField">The string with the search</param>
        /// <returns>The list of objects</returns>
        List<T> FindAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

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
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="searchField">The string with the search</param>
        /// <returns>The list of objects</returns>
        List<T> FindAllIncludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

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
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="searchField">The string with the search</param>
        /// <returns>The list of objects</returns>
        List<T> FindAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

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
        T FindById(bool isIncludes, bool isTracked, params object[] objs);

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
        T FindByIdExcludes(params object[] objs);

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
        T FindByIdExcludesTracked(params object[] objs);

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
        T FindByIdIncludes(params object[] objs);

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
        T FindByIdIncludesTracked(params object[] objs);

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
        /// <returns>The element, if found, <see langword="null"/> otherwise.</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        List<T> FindByManyId(bool isIncludes, bool isTracked, params object[] objs);

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
        List<T> FindManyByIdExcludes(params object[] objs);

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
        List<T> FindManyByIdExcludesTracked(params object[] objs);

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
        List<T> FindManyByIdIncludes(params object[] objs);

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
        List<T> FindManyByIdIncludesTracked(params object[] objs);

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
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="isIncludes">Will all other properties be included</param>
        /// <param name="isTracked">Will the element be tracked</param>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        List<T> GetAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

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
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="isIncludes">Will all other properties be included</param>
        /// <param name="isTracked">Will the element be tracked</param>
        /// <param name="predicateWhere">Condition/param>
        /// <returns>The list of objects</returns>
        List<T> GetAllBy(bool isIncludes, bool isTracked, Expression<Func<T, bool>> predicateWhere);

        /// <summary>
        /// Get a list of elements following condition <paramref name="predicateWhere"/>.
        /// <br/>
        /// Every other property will be excluded, elements will not be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        List<T> GetAllByExcludes(Expression<Func<T, bool>> predicateWhere);

        /// <summary>
        /// Get a list of elements following condition <paramref name="predicateWhere"/>.
        /// <br/>
        /// Every other property will be excluded, elements will be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        List<T> GetAllByExcludesTracked(Expression<Func<T, bool>> predicateWhere);

        /// <summary>
        /// Get a list of elements following condition <paramref name="predicateWhere"/>.
        /// <br/>
        /// Every other property will be included, elements will not be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        List<T> GetAllByIncludes(Expression<Func<T, bool>> predicateWhere);

        /// <summary>
        /// Get a list of elements following condition <paramref name="predicateWhere"/>.
        /// <br/>
        /// Every other property will be included, elements will be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        List<T> GetAllByIncludesTracked(Expression<Func<T, bool>> predicateWhere);

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be excluded, elements will not be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        List<T> GetAllExcludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be excluded, elements will be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        List<T> GetAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be included, elements will not be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        List<T> GetAllIncludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        /// <summary>
        /// Get a list of elements ordered by <paramref name="orderreq"/> following condition <paramref name="predicateWhere"/>
        /// for page <paramref name="page"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Every other property will be included, elements will be tracked.
        /// <br/>
        /// If <paramref name="predicateWhere"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="page">The page</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="orderreq">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        List<T> GetAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        /// <summary>
        /// Get the collection as a <see cref="List{T}"/>. Specify if all other types in relationship with <typeparamref name="T"/>
        /// have to be included in the query, and if the elements have to be tracked.
        /// </summary>
        /// <param name="isIncludes">Whether or not other types in relationship with <typeparamref name="T"/>
        /// have to be included in the query</param>
        /// <param name="isTracked">Whether or not elements have to be tracked</param>
        /// <returns>The list</returns>
        List<T> List(bool isIncludes, bool isTracked);

        /// <summary>
        /// Get the collection as a <see cref="List{T}"/>, other types in relationship with <typeparamref name="T"/> excluded, elements not tracked.
        /// </summary>
        /// <returns>The list</returns>
        List<T> ListExcludes();

        /// <summary>
        /// Get the collection as a <see cref="List{T}"/>, other types in relationship with <typeparamref name="T"/> excluded, elements tracked.
        /// </summary>
        /// <returns>The list</returns>
        List<T> ListExcludesTracked();

        /// <summary>
        /// Get the collection as a <see cref="List{T}"/>, other types in relationship with <typeparamref name="T"/> included, elements not tracked.
        /// </summary>
        /// <returns>The list</returns>
        List<T> ListIncludes();

        /// <summary>
        /// Get the collection as a <see cref="List{T}"/>, other types in relationship with <typeparamref name="T"/> included, elements tracked.
        /// </summary>
        /// <returns>The list</returns>
        List<T> ListIncludesTracked();

        /// <summary>
        /// Checks whether or not there is another page after <paramref name="page"/>
        /// for the search using <see cref="SearchExpression(string)"/> having <paramref name="maxByPage"/> 
        /// elements per page.
        /// </summary>
        /// <param name="page">The current page number.</param>
        /// <param name="maxByPage">The maximum number of elements per page.</param>
        /// <param name="searchField">The string searched.</param>
        /// <returns>Whether or not there is another page after page number <paramref name="page"/></returns>
        bool NextExist(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

        /// <summary>
        /// The expression with which the elements will be ordered when using <see cref="FindAll"/>.
        /// </summary>
        /// <returns>The expression.</returns>
        Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> OrderExpression();

        /// <summary>
        /// Saves the element <paramref name="t"/>.
        /// </summary>
        /// <param name="t">The element to save</param>
        void Save(T t);

        /// <summary>
        /// Saves the elements <paramref name="t"/> after having crypted using 
        /// <see cref="HashTools.ComputeSha256Hash(string)"/> every string property with annotation
        /// <see cref="DataTypeAttribute"/> with type <see cref="DataType.Password"/>.
        /// </summary>
        /// <param name="t">The element to crypt then save.</param>
        void SaveCrypted(T t);

        /// <summary>
        /// The expression used to search the elements in <see cref="FindAll"/> and
        /// <see cref="NextExist(int, int, string)"/>.
        /// <br/>
        /// If <see cref="OrderExpression"/> fails to be translated from EntityFramework C# LINQ query to
        /// a SQL command, the predicate will be ignored. 
        /// <br/>
        /// See <see cref="GenericTools.QueryTryPredicateWhere{T}(IQueryable{T}, Expression{Func{T, bool}})"/>
        /// for more information.
        /// </summary>
        /// <param name="searchField"></param>
        /// <returns></returns>
        Expression<Func<T, bool>> SearchExpression(string searchField = "");

        /// <summary>
        /// Updates an element <paramref name="t"/>
        /// </summary>
        /// <param name="t">The element to update</param>
        void Update(T t);

        /// <summary>
        /// Updates the element <paramref name="t"/> after having crypted using 
        /// <see cref="HashTools.ComputeSha256Hash(string)"/> every string property with annotation
        /// <see cref="DataTypeAttribute"/> with type <see cref="DataType.Password"/> that have changed.
        /// </summary>
        /// <param name="t">The object to update.</param>
        void UpdateCrypted(T t);

        /// <summary>
        /// Update only the property with name <paramref name="propertyName"/> of the object with
        /// either same Id or keys as <paramref name="t"/> with value <paramref name="newValue"/>.
        /// </summary>
        /// <param name="t">The object to update</param>
        /// <param name="propertyName">The name of the property to update</param>
        /// <param name="newValue">The new value of the property with name <paramref name="propertyName"/></param>
        void UpdateOne(T t, string propertyName, object newValue);

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
        void UpdateOneCrypted(T t, string propertyName, object newValue);
    }
}