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
using System.Data.Entity;

namespace GenericRepositoryAndService.Service
{
    ///<include file='docs.xml' path='doc/members/member[@name="T:GenericRepositoryAndService.Service.GenericService`1"]/*'/>
    public abstract class GenericService<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.Collection(System.Boolean,System.Boolean)"]/*'/>
        public IQueryable<T> Collection(bool isIncludes, bool isTracked)
        {
            return _repository.Collection(isIncludes, isTracked);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.CollectionExcludes"]/*'/>
        public IQueryable<T> CollectionExcludes()
        {
            return Collection(false, false);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.CollectionExcludesTracked"]/*'/>
        public IQueryable<T> CollectionExcludesTracked()
        {
            return Collection(false, true);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.CollectionIncludes"]/*'/>
        public IQueryable<T> CollectionIncludes()
        {
            return Collection(true, false);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.CollectionIncludesTracked"]/*'/>
        public IQueryable<T> CollectionIncludesTracked()
        {
            return Collection(true, true);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.Count(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public long Count(Expression<Func<T, bool>> predicateWhere = null)
        {
            return _repository.Count(predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindAll(System.Boolean,System.Boolean,System.Int32,System.Int32,System.String)"]/*'/>
        public List<T> FindAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return GetAll(isIncludes, isTracked, page, maxByPage, OrderExpression(), SearchExpression(searchField));
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindAllExcludes(System.Int32,System.Int32,System.String)"]/*'/>
        public List<T> FindAllExcludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return FindAll(false, false, page, maxByPage, searchField);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindAllExcludesTracked(System.Int32,System.Int32,System.String)"]/*'/>
        public List<T> FindAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return FindAll(false, true, page, maxByPage, searchField);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindAllIncludes(System.Int32,System.Int32,System.String)"]/*'/>
        public List<T> FindAllIncludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return FindAll(true, false, page, maxByPage, searchField);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindAllIncludesTracked(System.Int32,System.Int32,System.String)"]/*'/>
        public List<T> FindAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return FindAll(true, true, page, maxByPage, searchField);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindById(System.Boolean,System.Boolean,System.Object[])"]/*'/>
        public T FindById(bool isIncludes, bool isTracked, params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            return _repository.FindById(isIncludes, isTracked, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindByIdExcludes(System.Object[])"]/*'/>
        public T FindByIdExcludes(params object[] objs)
        {
            return FindById(false, false, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindByIdExcludesTracked(System.Object[])"]/*'/>
        public T FindByIdExcludesTracked(params object[] objs)
        {
            return FindById(false, true, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindByIdIncludes(System.Object[])"]/*'/>
        public T FindByIdIncludes(params object[] objs)
        {
            return FindById(true, false, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindByIdIncludesTracked(System.Object[])"]/*'/>
        public T FindByIdIncludesTracked(params object[] objs)
        {
            return FindById(true, true, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindByManyId(System.Boolean,System.Boolean,System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindManyByIdExcludes(System.Object[])"]/*'/>
        public List<T> FindManyByIdExcludes(params object[] objs)
        {
            return FindByManyId(false, false, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindManyByIdExcludesTracked(System.Object[])"]/*'/>
        public List<T> FindManyByIdExcludesTracked(params object[] objs)
        {
            return FindByManyId(false, true, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindManyByIdIncludes(System.Object[])"]/*'/>
        public List<T> FindManyByIdIncludes(params object[] objs)
        {
            return FindByManyId(true, false, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.FindManyByIdIncludesTracked(System.Object[])"]/*'/>
        public List<T> FindManyByIdIncludesTracked(params object[] objs)
        {
            return FindByManyId(true, true, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.GetAllBy(System.Boolean,System.Boolean,System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllBy(bool isIncludes, bool isTracked, Expression<Func<T, bool>> predicateWhere)
        {
            return _repository.GetAllBy(isIncludes, isTracked, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.GetAllByExcludes(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllByExcludes(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(false, false, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.GetAllByExcludesTracked(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllByExcludesTracked(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(false, true, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.GetAllByIncludes(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllByIncludes(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(true, false, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.GetAllByIncludesTracked(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllByIncludesTracked(Expression<Func<T, bool>> predicateWhere)
        {
            return GetAllBy(true, true, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.GetAll(System.Boolean,System.Boolean,System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            int start = (page - 1) * maxByPage;
            return _repository.GetAll(isIncludes, isTracked, start, maxByPage, orderreq, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.GetAllExcludes(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllExcludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(false, false, page, maxByPage, orderreq, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.GetAllExcludesTracked(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(false, true, page, maxByPage, orderreq, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.GetAllIncludes(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllIncludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(true, false, page, maxByPage, orderreq, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.GetAllIncludesTracked(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        public List<T> GetAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return GetAll(true, true, page, maxByPage, orderreq, predicateWhere);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.List(System.Boolean,System.Boolean)"]/*'/>
        public List<T> List(bool isIncludes, bool isTracked)
        {
            return _repository.List(isIncludes, isTracked);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.ListExcludes"]/*'/>
        public List<T> ListExcludes()
        {
            return List(false, false);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.ListExcludesTracked"]/*'/>
        public List<T> ListExcludesTracked()
        {
            return List(false, true);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.ListIncludes"]/*'/>
        public List<T> ListIncludes()
        {
            return List(true, false);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.ListIncludesTracked"]/*'/>
        public List<T> ListIncludesTracked()
        {
            return List(true, true);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.NextExist(System.Int32,System.Int32,System.String)"]/*'/>
        public bool NextExist(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return (page * maxByPage) < _repository.Count(SearchExpression(searchField));
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.OrderExpression"]/*'/>
        public abstract Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> OrderExpression();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.SearchExpression(System.String)"]/*'/>
        public abstract Expression<Func<T, bool>> SearchExpression(string searchField = "");

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.Delete(System.Object[])"]/*'/>
        public void Delete(params object[] objs)
        {
            GenericToolsTypeAnalysis.CheckIfObjectIsKey<T>(objs);
            dynamic temprep = _repository;
            GenericToolsCRUD.PrepareDelete<T>(temprep.DataContext, objs);
            _repository.Delete(objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.Delete(`0)"]/*'/>
        public void Delete(T t)
        {
            dynamic temprep = _repository;
            GenericToolsCRUD.PrepareDelete<T>(temprep.DataContext, GenericToolsTypeAnalysis.GetKeysValues(t));
            _repository.Delete(t);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.Save(`0)"]/*'/>
        public void Save(T t)
        {
            object[] objs = GenericToolsCRUD.PrepareSave(t);
            _repository.Save(t, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.SaveCrypted(`0)"]/*'/>
        public void SaveCrypted(T t)
        {
            t = GenericToolsCRUDCrypt.Crypt(t);
            Save(t);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.Update(`0)"]/*'/>
        public void Update(T t)
        {
            dynamic temprep = _repository;
            object[] objs = GenericToolsCRUD.PrepareUpdate(temprep.DataContext, t);
            _repository.Update(t, objs);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.UpdateCrypted(`0)"]/*'/>
        public void UpdateCrypted(T t)
        {
            T told = FindByIdExcludes(GenericToolsTypeAnalysis.GetKeysValues(t));
            t = GenericToolsCRUDCrypt.CryptIfUpdated(told, t);
            Update(t);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.UpdateOne(`0,System.String,System.Object)"]/*'/>
        public void UpdateOne(T t, string propertyName, object newValue)
        {
            dynamic temprep = _repository;
            GenericToolsCRUD.PrepareUpdateOne(temprep.DataContext, t, propertyName);
            _repository.UpdateOne(t, propertyName, newValue);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.GenericService`1.UpdateOneCrypted(`0,System.String,System.Object)"]/*'/>
        public void UpdateOneCrypted(T t, string propertyName, object newValue)
        {
            T told = FindByIdExcludes(GenericToolsTypeAnalysis.GetKeysValues(t));
            t = GenericToolsCRUDCrypt.CryptIfUpdatedOne(told, t, propertyName, newValue);
            UpdateOne(t, propertyName, newValue);
        }
    }
}