using MiseEnSituation.Exceptions;
using MiseEnSituation.Models;
using MiseEnSituation.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace MiseEnSituation.Repositories
{
    /// <summary>
    /// Generic Repository for class <typeparamref name="T"/> using context type <see cref="MyDbContext"/>.
    /// <remark>
    /// Assumes every class that derives from <see cref="BaseEntity"/> has a <see cref="DbSet"/> in <see cref="MyDbContext"/>
    /// <br/>
    /// And that reciprocally, every class having a <see cref="DbSet"/> in <see cref="MyDbContext"/> derives from <see cref="BaseEntity"/>
    /// </remark>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected MyDbContext DataContext;
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

        public GenericRepository(MyDbContext dataContext)
        {
            DataContext = dataContext;
            dbSet = DataContext.Set<T>();
            _DynamicDBListTypes = SetDynamicDBListTypes();
            _DynamicDBTypes = SetDynamicDBTypes();
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
        /// Code could be refactored and dismiss this class, since <see cref="GenericTools.TryListOfWhat(Type, out Type)"/> does the job <br/>
        /// I didn't know it was possible when I coded the handling of relationships and found out about that possibility when I was about to finish this code
        /// </remark>
        /// </summary>
        private class CustomParam
        {
            ///<summary> 
            ///The value of the property of <typeparamref name="T"/>
            ///</summary>
            public object Value { get; set; }
            /// <summary>
            /// The property of <typeparamref name="T"/> is of type <see cref="IList"/>&lt;<see cref="TypeofElement"/>&gt; if <see cref="IsList"/> is true, and it is of 
            /// type <see cref="TypeofElement"/> if <see cref="IsList"/> is false.
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
        /// Setup for <see cref="_DynamicDBListTypes"/>
        /// </summary>
        /// <returns>The dictionary</returns>
        private Dictionary<string, Type> SetDynamicDBListTypes()
        {
            Dictionary<string, Type> res = new Dictionary<string, Type>();
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (GenericTools.TryListOfWhat(property.PropertyType, out Type innerType))
                {
                    if (innerType.IsSubclassOf(typeof(BaseEntity)))
                    {
                        res.Add(property.Name, innerType);
                    }
                }
            }
            return res;
        }

        /// /// <summary>
        /// Setup for <see cref="_DynamicDBTypes"/>
        /// </summary>
        /// <returns>The dictionary</returns>
        private Dictionary<string, Type> SetDynamicDBTypes()
        {
            Dictionary<string, Type> res = new Dictionary<string, Type>();
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.PropertyType.IsSubclassOf(typeof(BaseEntity)))
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
        /// <returns>The query. Essentially, context.DbSetName.AsNoTracking().Include(...).Include(...)....Include(...)</returns>
        private IQueryable<T> QueryTInclude(MyDbContext myDbContext)
        {
            IQueryable<T> req = myDbContext.Set<T>().AsNoTracking().AsQueryable();
            foreach (string name in _DynamicDBListTypes.Keys.ToList())
            {
                req = req.Include(name);
            }
            foreach (string name in _DynamicDBTypes.Keys.ToList())
            {
                req = req.Include(name);
            }
            return req;
        }

        /// <summary>
        /// Setup all possible and necessary Include(propertyname) for <see cref="DbSet"/> queries.
        /// </summary>
        /// <param name="myDbContext">The context used for the query</param>
        /// <returns>The query. Essentially, context.DbSetName.Include(...).Include(...)....Include(...)</returns>
        private IQueryable<T> QueryTIncludeTracked(MyDbContext myDbContext)
        {
            IQueryable<T> req = myDbContext.Set<T>().AsQueryable();
            foreach (string name in _DynamicDBListTypes.Keys.ToList())
            {
                req = req.Include(name);
            }
            foreach (string name in _DynamicDBTypes.Keys.ToList())
            {
                req = req.Include(name);
            }
            return req;
        }

        public IQueryable<T> CollectionExcludes()
        {
            return dbSet.AsNoTracking();
        }

        public IQueryable<T> CollectionExcludesTracked()
        {
            return dbSet;
        }

        public IQueryable<T> CollectionIncludes()
        {
            return QueryTInclude(DataContext);
        }

        public IQueryable<T> CollectionIncludesTracked()
        {
            return QueryTIncludeTracked(DataContext);
        }

        public void Commit()
        {
            DataContext.SaveChanges();
        }

        public long Count(Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req = CollectionIncludes();

            if (predicateWhere != null)
                req = req.Where(predicateWhere);

            return req.Count();
        }

        public void Delete(int id)
        {
            Remove(id);
            Commit();
        }

        public void Delete(T t)
        {
            Remove(t);
            Commit();
        }

        public T FindByIdExcludes(int id)
        {
            return CollectionExcludes().SingleOrDefault(t => t.Id == id);
        }

        public T FindByIdExcludesTracked(int id)
        {
            return CollectionExcludesTracked().SingleOrDefault(t => t.Id == id);
        }

        public T FindByIdIncludes(int id)
        {
            return CollectionIncludes().SingleOrDefault(t => t.Id == id);
        }

        public T FindByIdIncludesTracked(int id)
        {
            return CollectionIncludesTracked().SingleOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Specify a query <paramref name="orderedreq"/> (ordered) to get element following the condition <paramref name="predicateWhere"/> from element <paramref name="start"/> with at most <paramref name="maxByPage"/> elements
        /// </summary>
        /// <param name="orderedreq">The ordered query to specify</param>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="predicateWhere">Conidition</param>
        /// <returns>The query</returns>
        private IQueryable<T> WhereSkipTake(IQueryable<T> orderedreq, int start, int maxByPage, Expression<Func<T, bool>> predicateWhere)
        {
            if (predicateWhere != null)
                orderedreq = orderedreq.Where(predicateWhere);

            orderedreq = orderedreq.Skip(start).Take(maxByPage);
            return orderedreq;
        }

        /// <summary>
        /// Get a list of elements oreder by <paramref name="keyOrderBy"/> following condition <paramref name="predicateWhere"/>
        /// starting at index <paramref name="start"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Elements will be tracked
        /// <br/>
        /// Every other property will be excluded
        /// </summary>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="keyOrderBy">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllExcludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req;
            if (orderreq != null)
            {
                req = orderreq.Compile().Invoke(CollectionExcludes());
            }
            else
            {
                req = CollectionExcludes().OrderBy(t => t.Id);
            }

            req = WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        /// <summary>
        /// Get a list of elements oreder by <paramref name="keyOrderBy"/> following condition <paramref name="predicateWhere"/>
        /// starting at index <paramref name="start"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Elements will not be tracked
        /// <br/>
        /// Every other property will be excluded
        /// </summary>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="keyOrderBy">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllExcludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req;
            if (orderreq != null)
            {
                req = orderreq.Compile().Invoke(CollectionExcludesTracked());
            }
            else
            {
                req = CollectionExcludesTracked().OrderBy(t => t.Id);
            }

            req = WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        /// <summary>
        /// Get a list of elements oreder by <paramref name="keyOrderBy"/> following condition <paramref name="predicateWhere"/>
        /// starting at index <paramref name="start"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Elements will be tracked
        /// <br/>
        /// Every other property will be included
        /// </summary>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="keyOrderBy">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllIncludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req;
            if (orderreq != null)
            {
                req = orderreq.Compile().Invoke(CollectionIncludes());
            }
            else
            {
                req = CollectionIncludes().OrderBy(t => t.Id);
            }

            req = WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        /// <summary>
        /// Get a list of elements ordered by <paramref name="keyOrderBy"/> following condition <paramref name="predicateWhere"/>
        /// starting at index <paramref name="start"/> with at most <paramref name="maxByPage"/> elements.
        /// <br/>
        /// Elements will not be tracked
        /// <br/>
        /// Every other property will be included
        /// </summary>
        /// <param name="start">Starting index</param>
        /// <param name="maxByPage">Maximum number of elements</param>
        /// <param name="keyOrderBy">Order function</param>
        /// <param name="predicateWhere">Condition</param>
        /// <returns>The list of objects</returns>
        public List<T> GetAllIncludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req;
            if (orderreq != null)
            {
                req = orderreq.Compile().Invoke(CollectionIncludesTracked());
            }
            else
            {
                req = CollectionIncludesTracked().OrderBy(t => t.Id);
            }

            req = WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        public List<T> ListExcludes()
        {
            return CollectionExcludes().ToList();
        }

        public List<T> ListExcludesTracked()
        {
            return CollectionExcludesTracked().ToList();
        }

        public List<T> ListIncludes()
        {
            return CollectionIncludes().ToList();
        }

        public List<T> ListIncludesTracked()
        {
            return CollectionIncludesTracked().ToList();
        }

        /// <summary>
        /// Modifies an element <paramref name="t"/> in DB of type <typeparamref name="T"/>
        /// <br/>
        /// Throws exception <see cref="CascadeCreationInDBException"/> if <typeparamref name="T"/> is in a relationship with a class in a <see cref="DbSet"/> of <see cref="DataContext"/>. 
        /// These elements could be dublicated in DB otherwise, they could be loaded in the context.
        /// </summary>
        /// <param name="t">Element to modify</param>
        /// <exception cref="CascadeCreationInDBException"/>
        public void Modify(T t)
        {
            if (_DynamicDBListTypes.Count != 0 || _DynamicDBTypes.Count != 0)
                throw new CascadeCreationInDBException(typeof(T));
            if (DataContext.Entry(t).State == EntityState.Detached)
            {
                dbSet.Attach(t);
            }
            DataContext.Entry(t).State = EntityState.Modified;
        }

        public void Remove(int id)
        {
            Remove(FindByIdIncludes(id));
        }

        public void Remove(T t)
        {
            if (DataContext.Entry(t).State == EntityState.Detached)
            {
                dbSet.Attach(t);
            }
            dbSet.Remove(t);
        }

        /// <summary>
        /// Updates an element <paramref name="t"/> in DB of type <typeparamref name="T"/>.
        /// <br/>
        /// <remark><paramref name="objs"/> are properties of <typeparamref name="T"/> in a relationship
        /// with <typeparamref name="T"/>.</remark>
        /// <br/>
        /// <remark>Objects not mentionned will be set to either null or <c>new List&lt;Class&gt;()</c>.</remark>
        /// </summary>
        /// <param name="t">The object to update</param>
        /// <param name="objs">List of objects that are in the object <paramref name="t"/> and that
        /// are in relationship with the type <typeparamref name="T"/>. 
        /// <br/>
        /// <remark><paramref name="objs"/> are properties of <typeparamref name="T"/> in a relationship
        /// with <typeparamref name="T"/>.</remark>
        /// <br/>
        /// <remark>Objects not mentionned will be set to either null or <c>new List&lt;Class&gt;()</c>.</remark>
        /// </param>
        public void Update(T t, params object[] objs)
        {
            if (_DynamicDBListTypes.Count != 0 || _DynamicDBTypes.Count != 0)
            {
                CustomParam[] props = SetCustom(objs);
                UpdateGeneric(t, props);
            }
            else
            {
                Modify(t);
                Commit();
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
            if (_DynamicDBListTypes.Count != 0 || _DynamicDBTypes.Count != 0)
                throw new CascadeCreationInDBException(typeof(T));
            dbSet.Add(t);
        }

        /// <summary>
        /// Saves an element <paramref name="t"/> in DB of type <typeparamref name="T"/>.
        /// <br/>
        /// <remark><paramref name="objs"/> are properties of <typeparamref name="T"/> in a relationship
        /// with <typeparamref name="T"/>.</remark>
        /// <br/>
        /// <remark>Objects not mentionned will be set to either null or <c>new List&lt;Class&gt;()</c></remark>
        /// </summary>
        /// <param name="t">The object to update</param>
        /// <param name="objs">List of objects that are in the object <paramref name="t"/> and that
        /// are in relationship with the type <typeparamref name="T"/>. 
        /// <br/>
        /// <remark><paramref name="objs"/> are properties of <typeparamref name="T"/> in a relationship
        /// with <typeparamref name="T"/>.</remark>
        /// <br/>
        /// <remark>Objects not mentionned will be set to either null or <c>new List&lt;Class&gt;()</c>.</remark>
        /// </param>
        public void Save(T t, params object[] objs)
        {
            if (_DynamicDBListTypes.Count != 0 || _DynamicDBTypes.Count != 0)
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
        /// Create a <see cref="CustomParam"/> with value <c>new List&lt;Class&gt;()</c> for the 
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
                                    true);
        }

        /// <summary>
        /// Create a <see cref="CustomParam"/> with value null for the 
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
                                   false);
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
                                    true);
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
                                   false);
        }

        /// <summary>
        /// Setup for <see cref="CustomParam"/>.
        /// <br/>
        /// If an object corresponding to a property representing a relationship involving <typeparamref name="T"/> is in <paramref name="objs"/>
        /// construct a new <see cref="CustomParam"/> corresponding to that property.
        /// <br/>
        /// Once every <see cref="CustomParam"/> is constructed, construct default param (with value either null or <c>new List&lt;Class&gt;()</c>)
        /// for every other property that was not included in <paramref name="objs"/>.
        /// </summary>
        /// <param name="objs">List of objets for which to set up <see cref="CustomParam"/></param>
        /// <returns>The complete list of <see cref="CustomParam"/> with every property representing a 
        /// relationship involving <typeparamref name="T"/> covered.</returns>
        private CustomParam[] SetCustom(params object[] objs)
        {
            CustomParam[] res = new CustomParam[_DynamicDBListTypes.Count() + _DynamicDBTypes.Count()];
            int resindex = 0;

            List<string> lkeysforlisttypes = _DynamicDBListTypes.Keys.ToList();
            List<Type> ltypesforlisttpes = _DynamicDBListTypes.Values.ToList();
            List<Type> lTlisttype = _DynamicDBListTypes.Values.Select(typ => typeof(List<>).MakeGenericType(typ)).ToList();

            List<string> lkeysfortypes = _DynamicDBTypes.Keys.ToList();
            List<Type> lTtypes = _DynamicDBTypes.Values.ToList();

            foreach (object obj in objs.Where(o => o != null))
            {
                bool isFound = false;
                int i = 0;
                foreach (Type typ in lTlisttype)
                {
                    try
                    {
                        var test = Convert.ChangeType(obj, typ);
                        if (((IList)test).Count == 0)
                            throw new Exception();
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

        private T FindByIdIncludesTrackedInNewContext(int id, MyDbContext myDbContext)
        {
            return QueryTIncludeTracked(myDbContext).SingleOrDefault(tt => tt.Id == id);
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
                if (!props.Select(cp => cp.Prop).Contains(p) && p.CanWrite)
                    p.SetValue(res, p.GetValue(newt));
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
        private CustomParam SetNewParamFromContextList(MyDbContext newContext, CustomParam customParam)
        {
            var newvalue = Convert.ChangeType(Activator.CreateInstance(typeof(List<>).MakeGenericType(customParam.TypeofElement)), typeof(List<>).MakeGenericType(customParam.TypeofElement));
            if (customParam.Value != null)
                foreach (BaseEntity item in customParam.Value as IList)
                {
                    var newitem = newContext.Set(customParam.TypeofElement).Find(item.Id);
                    ((IList)newvalue).Add(newitem);
                }
            return new CustomParam(
                        newvalue,
                        customParam.TypeofElement,
                        customParam.PropertyName,
                        customParam.Prop,
                        true);
        }

        /// <summary>
        /// Setup a new parameter <see cref="CustomParam"/>. The only difference with <paramref name="customParam"/>
        /// is that the value is loaded from an other context <paramref name="newContext"/>
        /// <remark>The property is NOT of type <see cref="IList"/></remark>
        /// </summary>
        /// <param name="newContext">New context</param>
        /// <param name="customParam">Parameter from old context</param>
        /// <returns>The new parameter <see cref="CustomParam"/> from context <paramref name="newContext"/></returns>
        private CustomParam SetNewParamFromContextNotList(MyDbContext newContext, CustomParam customParam)
        {
            object newvalue = null;
            if (customParam.Value != null)
                newvalue = newContext.Set(customParam.TypeofElement).Find(((BaseEntity)customParam.Value).Id);
            return new CustomParam(
                                    newvalue,
                                    customParam.TypeofElement,
                                    customParam.PropertyName,
                                    customParam.Prop,
                                    false);
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
        private List<CustomParam> SetNewParamsFromContext(MyDbContext newContext, params CustomParam[] props)
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
        /// Updates an object <paramref name="t"/> of class <typeparamref name="T"/> in DB.
        /// <br/>
        /// <remark>Assumes every property representing a relationships involving <typeparamref name="T"/> has a corresponding <see cref="CustomParam"/> in <paramref name="propss"/></remark>
        /// </summary>
        /// <param name="t">Object to update</param>
        /// <param name="propss"><see cref="CustomParam"/> for properties representing a relationship involving <typeparamref name="T"/>
        /// containing new values</param>
        private void UpdateGeneric(T t, params CustomParam[] propss)
        {
            using (MyDbContext newContext = new MyDbContext())
            {
                T tToChange = FindByIdIncludesTrackedInNewContext(t.Id.Value, newContext);

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
        /// Saves an object <paramref name="t"/> of class <typeparamref name="T"/> in DB.
        /// <br/>
        /// <remark>Assumes every property representing a relationships involving <typeparamref name="T"/> has a corresponding <see cref="CustomParam"/> in <paramref name="propss"/></remark>
        /// </summary>
        /// <param name="t">Object to update</param>
        /// <param name="propss"><see cref="CustomParam"/> for properties representing a relationship involving <typeparamref name="T"/>
        /// containing new values</param>
        private void SaveGeneric(T t, params CustomParam[] propss)
        {
            using (MyDbContext newContext = new MyDbContext())
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
    }
}