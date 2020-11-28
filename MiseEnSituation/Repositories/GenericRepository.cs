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
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected MyDbContext DataContext;
        protected DbSet<T> dbSet;

        private Dictionary<string, Type> _DynamicDBListTypes;

        private Dictionary<string, Type> _DynamicDBTypes;

        public GenericRepository(MyDbContext dataContext)
        {
            DataContext = dataContext;
            dbSet = DataContext.Set<T>();
            _DynamicDBListTypes = SetDynamicDBListTypes();
            _DynamicDBTypes = SetDynamicDBTypes();
        }

        private class CustomParam
        {
            public object Value { get; set; }
            public Type TypeofElement { get; set; }
            public string PropertyName { get; set; }
            public PropertyInfo Prop { get; set; }
            public bool IsList { get; set; }

            public CustomParam(object value, Type typeofElement, string propertyName, PropertyInfo prop, bool isList)
            {
                Value = value;
                TypeofElement = typeofElement;
                PropertyName = propertyName;
                Prop = prop;
                IsList = isList;
            }
        }

        private Dictionary<string,Type> SetDynamicDBListTypes()
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

        public IQueryable<T> Collection()
        {
            return QueryTInclude(DataContext);
        }

        public IQueryable<T> CollectionTracked()
        {
            return QueryTIncludeTracked(DataContext);
        }

        public void Commit()
        {
            DataContext.SaveChanges();
        }

        public long Count(Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req = Collection();

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

        public T FindById(int id)
        {
            return Collection().SingleOrDefault(t => t.Id == id);
        }

        public T FindByIdTracked(int id)
        {
            return CollectionTracked().SingleOrDefault(t => t.Id == id);
        }

        private IQueryable<T> WhereSkipTake(IQueryable<T> orderedreq, int start, int maxByPage, Expression<Func<T, bool>> predicateWhere)
        {
            if (predicateWhere != null)
                orderedreq = orderedreq.Where(predicateWhere);

            orderedreq = orderedreq.Skip(start).Take(maxByPage);
            return orderedreq;
        }

        public List<T> GetAll(int start = 0, int maxByPage = int.MaxValue, Expression<Func<T, int?>> keyOrderBy = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req;
            if (keyOrderBy != null)
            {
                req = Collection().OrderBy(keyOrderBy);
            }  
            else
            {
                req = Collection().OrderBy(t => t.Id);
            }
                
            req = WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        public List<T> GetAllTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<T, int?>> keyOrderBy = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req;
            if (keyOrderBy != null)
            {
                req = CollectionTracked().OrderBy(keyOrderBy);
            }
            else
            {
                req = CollectionTracked().OrderBy(t => t.Id);
            }  

            req = WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        public List<T> List()
        {
            return Collection().ToList();
        }

        public List<T> ListTracked()
        {
            return CollectionTracked().ToList();
        }

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
            Remove(FindById(id));
        }

        public void Remove(T t)
        {
            if (DataContext.Entry(t).State == EntityState.Detached)
            {
                dbSet.Attach(t);
            }
            dbSet.Remove(t);
        }

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

        public void Add(T t)
        {
            if (_DynamicDBListTypes.Count != 0 || _DynamicDBTypes.Count != 0)
                throw new CascadeCreationInDBException(typeof(T));
            dbSet.Add(t);
        }

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

        private CustomParam[] SetCustom(params object[] objs)
        {
            CustomParam[] res = new CustomParam[_DynamicDBListTypes.Count() + _DynamicDBTypes.Count()];
            int resindex = 0;

            List<string> lstskeyslst = _DynamicDBListTypes.Keys.ToList();
            List<Type> lsttypelst = _DynamicDBListTypes.Values.ToList();
            List<Type> lsttypelstlistform = _DynamicDBListTypes.Values.Select(typ => typeof(List<>).MakeGenericType(typ)).ToList();

            List<string> lstkeys = _DynamicDBTypes.Keys.ToList();
            List<Type> lsttypes = _DynamicDBTypes.Values.ToList();

            foreach (object obj in objs)
            {
                bool isFound = false;
                int i = 0;
                foreach (Type typ in lsttypelstlistform)
                {
                    try
                    {
                        var test = Convert.ChangeType(obj, typ);
                        isFound = true;
                        CustomParam cp = new CustomParam(obj,
                                                    lsttypelst[i],
                                                    lstskeyslst[i],
                                                    typeof(T).GetProperty(lstskeyslst[i]),
                                                    true
                                                    );
                        res[resindex] = cp;
                        resindex++;
                        lstskeyslst.RemoveAt(i);
                        lsttypelst.RemoveAt(i);
                        lsttypelstlistform.RemoveAt(i);
                        break;
                    }
                    catch { }
                    i++;
                }
                i = 0;
                foreach (Type typ in lsttypes)
                {
                    try
                    {
                        var test = Convert.ChangeType(obj, typ);
                        isFound = true;
                        CustomParam cp = new CustomParam(obj,
                                                    lsttypes[i],
                                                    lstkeys[i],
                                                    typeof(T).GetProperty(lstkeys[i]),
                                                    false
                                                    );
                        res[resindex] = cp;
                        resindex++;
                        lstkeys.RemoveAt(i);
                        lsttypes.RemoveAt(i);
                        break;
                    }
                    catch { }
                    i++;
                }
                if (!isFound)
                    throw new ArgumentException("Invalid arguments for class " + typeof(T).Name);
            }

            foreach (string key in lstskeyslst)
            {
                CustomParam cp = new CustomParam((IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(_DynamicDBListTypes[key])),
                                            _DynamicDBListTypes[key],
                                            key,
                                            typeof(T).GetProperty(key),
                                            true);
                res[resindex] = cp;
                resindex++;
            }

            foreach (string key in lstkeys)
            {
                CustomParam cp = new CustomParam(null,
                                            _DynamicDBTypes[key],
                                            key,
                                            typeof(T).GetProperty(key),
                                            false);
                res[resindex] = cp;
                resindex++;
            }

            return res;
        }

        private T QueryTToChange(int id, MyDbContext myDbContext)
        {
            IQueryable<T> req = QueryTIncludeTracked(myDbContext);
            return req.SingleOrDefault(tt => tt.Id == id);
        }

        private T ModifyOtherProperties(T t, T newt, params CustomParam[] props)
        {
            T res = t;
            foreach (var p in typeof(T).GetProperties())
            {
                if (!props.Select(cp => cp.Prop).Contains(p))
                    p.SetValue(res, p.GetValue(newt));
            }
            return res;
        }

        private List<CustomParam> SetNewParamsFromContext(MyDbContext newContext, params CustomParam[] props)
        {
            List<CustomParam> newparams = new List<CustomParam>();

            foreach (CustomParam customParam in props)
            {
                if (customParam.IsList)
                {
                    IList newvalue = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(customParam.TypeofElement));
                    if (customParam.Value != null)
                        foreach (BaseEntity item in customParam.Value as IList)
                        {
                            var newitem = newContext.Set(customParam.TypeofElement).Find(item.Id);
                            newvalue.Add(newitem);
                        }
                    newparams.Add(new CustomParam(
                        newvalue,
                        customParam.TypeofElement,
                        customParam.PropertyName,
                        customParam.Prop,
                        true));
                }
                else
                {
                    object newvalue = null;
                    if (customParam.Value != null)
                        newvalue = newContext.Set(customParam.TypeofElement).Find(((BaseEntity)customParam.Value).Id);
                    newparams.Add(new CustomParam(
                        newvalue,
                        customParam.TypeofElement,
                        customParam.PropertyName,
                        customParam.Prop,
                        false));
                }

            }
            return newparams;
        }

        private void UpdateGeneric(T t, params CustomParam[] propss)
        {

            using (MyDbContext newContext = new MyDbContext())
            {
                T tToChange = QueryTToChange(t.Id.Value, newContext);

                tToChange = ModifyOtherProperties(tToChange, t, propss);

                List<CustomParam> newparams = SetNewParamsFromContext(newContext, propss);

                foreach (CustomParam newparam in newparams)
                {
                    if ((newparam.Value != null && !newparam.IsList ) || ((newparam.Value as IList).Count != 0 && newparam.IsList))
                    {
                        typeof(T).GetProperty(newparam.PropertyName).SetValue(tToChange, Convert.ChangeType(newparam.Value, typeof(T).GetProperty(newparam.PropertyName).PropertyType));
                    }
                }

                newContext.Entry(tToChange).State = EntityState.Modified;

                newContext.SaveChanges();
            }
        }

        private void SaveGeneric(T t, params CustomParam[] propss)
        {
            using (MyDbContext newContext = new MyDbContext())
            {
                List<CustomParam> newparams = SetNewParamsFromContext(newContext, propss);

                foreach (CustomParam newparam in newparams)
                {
                    if ((newparam.Value != null && !newparam.IsList) || ((newparam.Value as IList).Count != 0 && newparam.IsList))
                    {
                        typeof(T).GetProperty(newparam.PropertyName).SetValue(t, Convert.ChangeType(newparam.Value, typeof(T).GetProperty(newparam.PropertyName).PropertyType));
                    }
                }

                newContext.Set<T>().Add(t);

                foreach (var entry in newContext.ChangeTracker.Entries())
                {
                    if (entry.Entity.GetType() != typeof(T))
                        entry.State = EntityState.Modified;
                }

                newContext.SaveChanges();
            }
        }
    }
}