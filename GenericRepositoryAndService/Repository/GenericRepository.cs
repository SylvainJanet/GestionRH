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
    /// <include file='docs.xml' path='doc/members/member[@name="T:GenericRepositoryAndService.Repository.GenericRepository`1"]/*'/>
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        internal DbContext DataContext;
        protected DbSet<T> dbSet;

        /// <include file='docs.xml' path='doc/members/member[@name="F:GenericRepositoryAndService.Repository.GenericRepository`1._DynamicDBListTypes"]/*'/>
        private readonly Dictionary<string, Type> _DynamicDBListTypes;

        /// <include file='docs.xml' path='doc/members/member[@name="F:GenericRepositoryAndService.Repository.GenericRepository`1._DynamicDBTypes"]/*'/>
        private readonly Dictionary<string, Type> _DynamicDBTypes;

        public GenericRepository(DbContext dataContext)
        {
            DataContext = dataContext;
            dbSet = DataContext.Set<T>();
            _DynamicDBListTypes = GenericToolsTypeAnalysis.DynamicDBListTypes<T>();
            _DynamicDBTypes = GenericToolsTypeAnalysis.DynamicDBTypes<T>();
        }

        /// <include file='docs.xml' path='doc/members/member[@name="T:GenericRepositoryAndService.Repository.GenericRepository`1.CustomParam"]/*'/>
        private class CustomParam
        {
            ///<include file='docs.xml' path='doc/members/member[@name="P:GenericRepositoryAndService.Repository.GenericRepository`1.CustomParam.Value"]/*'/>
            public object Value { get; set; }
            ///<include file='docs.xml' path='doc/members/member[@name="P:GenericRepositoryAndService.Repository.GenericRepository`1.CustomParam.TypeofElement"]/*'/>
            public Type TypeofElement { get; set; }
            ///<include file='docs.xml' path='doc/members/member[@name="P:GenericRepositoryAndService.Repository.GenericRepository`1.CustomParam.PropertyName"]/*'/>
            public string PropertyName { get; set; }
            ///<include file='docs.xml' path='doc/members/member[@name="P:GenericRepositoryAndService.Repository.GenericRepository`1.CustomParam.Prop"]/*'/>
            public PropertyInfo Prop { get; set; }
            ///<include file='docs.xml' path='doc/members/member[@name="P:GenericRepositoryAndService.Repository.GenericRepository`1.CustomParam.IsList"]/*'/>
            public bool IsList { get; set; }

            ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.CustomParam.#ctor(System.Object,System.Type,System.String,System.Reflection.PropertyInfo,System.Boolean)"]/*'/>
            public CustomParam(object value, Type typeofElement, string propertyName, PropertyInfo prop, bool isList)
            {
                Value = value;
                TypeofElement = typeofElement;
                PropertyName = propertyName;
                Prop = prop;
                IsList = isList;
            }
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Add(`0)"]/*'/>
        public void Add(T t)
        {
            if (GenericToolsTypeAnalysis.HasDynamicDBTypeOrListType<T>())
                throw new CascadeCreationInDBException(typeof(T));
            dbSet.Add(t);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Collection(System.Boolean,System.Boolean)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.CollectionExcludes"]/*'/>
        public IQueryable<T> CollectionExcludes()
        {
            return Collection(false, false);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.CollectionExcludesTracked"]/*'/>
        public IQueryable<T> CollectionExcludesTracked()
        {
            return Collection(false, true);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.CollectionIncludes"]/*'/>
        public IQueryable<T> CollectionIncludes()
        {
            return Collection(true, false);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.CollectionIncludesTracked"]/*'/>
        public IQueryable<T> CollectionIncludesTracked()
        {
            return Collection(true, true);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Commit"]/*'/>
        public void Commit()
        {
            DataContext.SaveChanges();
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Count(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public long Count(Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req = CollectionIncludes();

            if (predicateWhere != null)
                req = GenericToolsQueriesAndLists.QueryTryPredicateWhere(req, predicateWhere);

            return req.Count();
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Delete(System.Object[])"]/*'/>
        public void Delete(params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            Remove(objs);
            Commit();
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Delete(`0)"]/*'/>
        public void Delete(T t)
        {
            Remove(t);
            Commit();
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.FindById(System.Boolean,System.Boolean,System.Object[])"]/*'/>
        public T FindById(bool isIncludes, bool isTracked, params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            return GenericToolsQueriesAndLists.QueryWhereKeysAre(
                                                                    Collection(isIncludes, isTracked), 
                                                                    objs
                                                                 ).SingleOrDefault();
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.FindByIdExcludes(System.Object[])"]/*'/>
        public T FindByIdExcludes(params object[] objs)
        {
            return FindById(false, false, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.FindByIdExcludesTracked(System.Object[])"]/*'/>
        public T FindByIdExcludesTracked(params object[] objs)
        {
            return FindById(false, true, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.FindByIdIncludes(System.Object[])"]/*'/>
        public T FindByIdIncludes(params object[] objs)
        {
            return FindById(true, false, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.FindByIdIncludesTracked(System.Object[])"]/*'/>
        public T FindByIdIncludesTracked(params object[] objs)
        {
            return FindById(true, true, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.GetAllBy(System.Boolean,System.Boolean,System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllBy(bool isIncludes, bool isTracked, Expression<Func<T, bool>> predicateWhere)
        {
            return GetAll(isIncludes, isTracked, 0, int.MaxValue, null, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.GetAllByIncludes(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllByIncludes(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(true, false, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.GetAllByIncludesTracked(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllByIncludesTracked(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(true, true, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.GetAllByExcludes(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllByExcludes(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(false, false, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.GetAllByExcludesTracked(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllByExcludesTracked(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(false, true, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.GetAll(System.Boolean,System.Boolean,System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.GetAllExcludes(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllExcludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(false, false, start, maxByPage, orderreq, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.GetAllExcludesTracked(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllExcludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(false, true, start, maxByPage, orderreq, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.GetAllIncludes(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllIncludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(true, false, start, maxByPage, orderreq, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.GetAllIncludesTracked(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllIncludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(true, true, start, maxByPage, orderreq, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.List(System.Boolean,System.Boolean)"]/*'/>
        public List<T> List(bool isIncludes, bool isTracked)
        {
            return Collection(isIncludes, isTracked).ToList();
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.ListExcludes"]/*'/>
        public List<T> ListExcludes()
        {
            return List(false, false);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.ListExcludesTracked"]/*'/>
        public List<T> ListExcludesTracked()
        {
            return List(false, true);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.ListIncludes"]/*'/>
        public List<T> ListIncludes()
        {
            return List(true, false);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.ListIncludesTracked"]/*'/>
        public List<T> ListIncludesTracked()
        {
            return List(true, true);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Modify(`0)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Remove(System.Object[])"]/*'/>
        public void Remove(params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            Remove(FindByIdIncludes(objs));
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Remove(`0)"]/*'/>
        public void Remove(T t)
        {
            if (DataContext.Entry(t).State == EntityState.Detached)
            {
                dbSet.Attach(t);
            }
            dbSet.Remove(t);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Save(`0,System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.Update(`0,System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.CreateDefaultListCustomParamFromKey(System.String)"]/*'/>
        private CustomParam CreateDefaultListCustomParamFromKey(string key)
        {
            return new CustomParam((IList)(Activator.CreateInstance(typeof(List<>).MakeGenericType(_DynamicDBListTypes[key]))),
                                    _DynamicDBListTypes[key],
                                    key,
                                    typeof(T).GetProperty(key),
                                    true
                                   );
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.CreateDefaultCustomParamFromKey(System.String)"]/*'/>
        private CustomParam CreateDefaultCustomParamFromKey(string key)
        {
            return new CustomParam(null,
                                   _DynamicDBTypes[key],
                                   key,
                                   typeof(T).GetProperty(key),
                                   false
                                  );
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.CreateListCustomParamFromKey(System.String,System.Object)"]/*'/>
        private CustomParam CreateListCustomParamFromKey(string key, object obj)
        {
            return new CustomParam(obj,
                                    _DynamicDBListTypes[key],
                                    key,
                                    typeof(T).GetProperty(key),
                                    true
                                   );
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.CreateCustomParamFromKey(System.String,System.Object)"]/*'/>
        private CustomParam CreateCustomParamFromKey(string key, object obj)
        {
            return new CustomParam(obj,
                                   _DynamicDBTypes[key],
                                   key,
                                   typeof(T).GetProperty(key),
                                   false
                                  );
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.SetCustom(System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.SetNewParamFromContextList(System.Data.Entity.DbContext,GenericRepositoryAndService.Repository.GenericRepository{`0}.CustomParam)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.SetNewParamFromContextNotList(System.Data.Entity.DbContext,GenericRepositoryAndService.Repository.GenericRepository{`0}.CustomParam)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.SetNewParamsFromContext(System.Data.Entity.DbContext,GenericRepositoryAndService.Repository.GenericRepository{`0}.CustomParam[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.ModifyOtherProperties(`0,`0,GenericRepositoryAndService.Repository.GenericRepository{`0}.CustomParam[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.SaveGeneric(`0,GenericRepositoryAndService.Repository.GenericRepository{`0}.CustomParam[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.FindByIdIncludesTrackedInNewContext(System.Data.Entity.DbContext,System.Object[])"]/*'/>
        private T FindByIdIncludesTrackedInNewContext(DbContext dbContext, params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            return GenericToolsQueriesAndLists.QueryWhereKeysAre(
                                                                    GenericToolsQueriesAndLists.QueryTIncludeTracked<T>(dbContext), 
                                                                    objs
                                                                 ).SingleOrDefault();
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.FindByIdIncludesInNewContext(System.Data.Entity.DbContext,System.Object[])"]/*'/>
        private T FindByIdIncludesInNewContext(DbContext dbContext, params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            return GenericToolsQueriesAndLists.QueryWhereKeysAre(
                                                                    GenericToolsQueriesAndLists.QueryTInclude<T>(dbContext), 
                                                                    objs
                                                                ).SingleOrDefault();
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.UpdateGeneric(System.Data.Entity.DbContext,`0,GenericRepositoryAndService.Repository.GenericRepository{`0}.CustomParam[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.GenericRepository`1.UpdateOne(`0,System.String,System.Object)"]/*'/>
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