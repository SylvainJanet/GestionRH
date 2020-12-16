using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MiseEnSituation.Exceptions;
using MiseEnSituation.Tools.Generic;
using MiseEnSituation.Models;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace MiseEnSituation.Repositories
{
    /// <summary>
    /// Generic Repository interface for class <typeparamref name="T"/> using context 
    /// type <see cref="MyDbContext"/>.
    /// <remark>
    /// Assumes every class that either derives from <see cref="BaseEntity"/> 
    /// or has at least one property with annotation <see cref="KeyAttribute"/> 
    /// has a <see cref="DbSet"/> in <see cref="MyDbContext"/>.
    /// <br/>
    /// And that reciprocally, every class having a <see cref="DbSet"/> in 
    /// <see cref="MyDbContext"/> either derives from <see cref="BaseEntity"/>
    /// or has at least one property with annotation <see cref="KeyAttribute"/>.
    /// </remark>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Adds an element <paramref name="t"/> in DB of type <typeparamref name="T"/>
        /// <br/>
        /// Throws exception <see cref="CascadeCreationInDBException"/> if <typeparamref name="T"/> is in a relationship with a class in a <see cref="DbSet"/> of <see cref="DataContext"/>. 
        /// These elements could be dublicated in DB otherwise, they could be loaded in the context.
        /// </summary>
        /// <param name="t">Element to add</param>
        /// <exception cref="CascadeCreationInDBException"/>
        void Add(T t);

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
        /// Commit the changes in DB
        /// </summary>
        void Commit();

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
        long Count(Expression<Func<T, bool>> predicateWhere = null);

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
        void Delete(params object[] objs);

        /// <summary>
        /// Deletes a specific object <paramref name="t"/> of type <typeparamref name="T"/> from DB
        /// </summary>
        /// <param name="t">The object to delete</param>
        void Delete(T t);

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
        List<T> GetAll(bool isIncludes, bool isTracked, int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

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
        List<T> GetAllBy(bool isIncludes, bool isTracked, Expression<Func<T, bool>> predicateWhere);

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
        List<T> GetAllByExcludes(Expression<Func<T, bool>> predicateWhere);

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
        List<T> GetAllByExcludesTracked(Expression<Func<T, bool>> predicateWhere);

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
        List<T> GetAllByIncludes(Expression<Func<T, bool>> predicateWhere);

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
        List<T> GetAllByIncludesTracked(Expression<Func<T, bool>> predicateWhere);

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
        List<T> GetAllExcludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

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
        List<T> GetAllExcludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

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
        List<T> GetAllIncludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

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
        List<T> GetAllIncludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

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
        /// Modifies an element <paramref name="t"/> in DB of type <typeparamref name="T"/>
        /// <br/>
        /// Throws exception <see cref="CascadeCreationInDBException"/> if <typeparamref name="T"/> is in a relationship with a class in a <see cref="DbSet"/> of <see cref="DataContext"/>. 
        /// These elements could be dublicated in DB otherwise, since they could be loaded in the context.
        /// </summary>
        /// <param name="t">Element to modify</param>
        /// <exception cref="CascadeCreationInDBException"/>
        void Modify(T t);

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
        void Remove(params object[] objs);

        /// <summary>
        /// Removes a specific object <paramref name="t"/> of type <typeparamref name="T"/> from DB (without comitting)
        /// </summary>
        /// <param name="t">The object to delete</param>
        void Remove(T t);

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
        void Save(T t, params object[] objs);

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
        void Update(T t, params object[] objs);

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
        void UpdateOne(T t, string propertyName, object newValue);
    }
}