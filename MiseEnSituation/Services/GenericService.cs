using MiseEnSituation.Exceptions;
using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Services
{
    public abstract class GenericService<T> : IGenericService<T> where T : BaseEntity
    {
        protected IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public IQueryable<T> CollectionExcludes()
        {
            return _repository.CollectionExcludes();
        }

        public IQueryable<T> CollectionExcludesTracked()
        {
            return _repository.CollectionExcludesTracked();
        }

        public IQueryable<T> CollectionIncludes()
        {
            return _repository.CollectionIncludes();
        }

        public IQueryable<T> CollectionIncludesTracked()
        {
            return _repository.CollectionIncludesTracked();
        }

        public long Count(Expression<Func<T, bool>> pedicateWhere = null)
        {
            return _repository.Count(pedicateWhere);
        }

        public void Delete(int? id)
        {
            if (!id.HasValue)
                throw new IdNullExceptionForClass(typeof(T));
            _repository.Delete(id.Value);
        }

        public void Delete(T t)
        {
            _repository.Delete(t);
        }

        public T FindByIdExcludes(int? id)
        {
            if (!id.HasValue)
                throw new IdNullExceptionForClass(typeof(T));
            return _repository.FindByIdIncludes(id.Value);
        }

        public T FindByIdExcludesTracked(int? id)
        {
            if (!id.HasValue)
                throw new IdNullExceptionForClass(typeof(T));
            return _repository.FindByIdIncludesTracked(id.Value);
        }

        public T FindByIdIncludes(int? id)
        {
            if (!id.HasValue)
                throw new IdNullExceptionForClass(typeof(T));
            return _repository.FindByIdIncludes(id.Value);
        }

        public T FindByIdIncludesTracked(int? id)
        {
            if (!id.HasValue)
                throw new IdNullExceptionForClass(typeof(T));
            return _repository.FindByIdIncludesTracked(id.Value);
        }

        public List<T> FindManyByIdExcludes(int?[] ids)
        {
            if (ids.Length == 0)
                throw new IdListEmptyForClassException(typeof(T));
            List<T> lst = new List<T>();
            foreach (var id in ids)
            {
                if (!id.HasValue)
                    throw new IdNullExceptionForClass(typeof(T));
                lst.Add(_repository.FindByIdExcludes(id.Value));
            }
            return lst;
        }

        public List<T> FindManyByIdExcludesTracked(int?[] ids)
        {
            if (ids.Length == 0)
                throw new IdListEmptyForClassException(typeof(T));
            List<T> lst = new List<T>();
            foreach (var id in ids)
            {
                if (!id.HasValue)
                    throw new IdNullExceptionForClass(typeof(T));
                lst.Add(_repository.FindByIdExcludesTracked(id.Value));
            }
            return lst;
        }

        public List<T> FindManyByIdIncludes(int?[] ids)
        {
            if (ids.Length == 0)
                throw new IdListEmptyForClassException(typeof(T));
            List<T> lst = new List<T>();
            foreach (var id in ids)
            {
                if (!id.HasValue)
                    throw new IdNullExceptionForClass(typeof(T));
                lst.Add(_repository.FindByIdIncludes(id.Value));
            }
            return lst;
        }

        public List<T> FindManyByIdIncludesTracked(int?[] ids)
        {
            if (ids.Length == 0)
                throw new IdListEmptyForClassException(typeof(T));
            List<T> lst = new List<T>();
            foreach (var id in ids)
            {
                if (!id.HasValue)
                    throw new IdNullExceptionForClass(typeof(T));
                lst.Add(_repository.FindByIdIncludesTracked(id.Value));
            }
            return lst;
        }

        public List<T> GetAllExcludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            int start = (page - 1) * maxByPage;
            return _repository.GetAllExcludes(start, maxByPage, orderreq, predicateWhere);
        }

        public List<T> GetAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            int start = (page - 1) * maxByPage;
            return _repository.GetAllExcludesTracked(start, maxByPage, orderreq, predicateWhere);
        }

        public List<T> GetAllIncludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            int start = (page - 1) * maxByPage;
            return _repository.GetAllIncludes(start, maxByPage, orderreq, predicateWhere);
        }

        public List<T> GetAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            int start = (page - 1) * maxByPage;
            return _repository.GetAllIncludesTracked(start, maxByPage, orderreq, predicateWhere);
        }

        public List<T> ListExcludes()
        {
            return _repository.ListExcludes();
        }

        public List<T> ListExcludesTracked()
        {
            return _repository.ListExcludesTracked();
        }

        public List<T> ListIncludes()
        {
            return _repository.ListIncludes();
        }

        public List<T> ListIncludesTracked()
        {
            return _repository.ListIncludesTracked();
        }

        public void Save(T t, params object[] objs)
        {
            _repository.Save(t, objs);
        }

        public void Update(T t, params object[] objs)
        {
            _repository.Update(t, objs);
        }

        public List<T> FindAllExcludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return GetAllExcludes(page, maxByPage, OrderExpression(), SearchExpression(searchField));
        }

        public List<T> FindAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return GetAllExcludesTracked(page, maxByPage, OrderExpression(), SearchExpression(searchField));
        }

        public List<T> FindAllIncludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return GetAllIncludes(page, maxByPage, OrderExpression(), SearchExpression(searchField));
        }

        public List<T> FindAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return GetAllIncludesTracked(page, maxByPage, OrderExpression(), SearchExpression(searchField));
        }

        public bool NextExist(int page = 1, int maxByPage = int.MaxValue, string searchField = "")
        {
            return (page * maxByPage) < _repository.Count(SearchExpression(searchField));
        }

        public abstract Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> OrderExpression();
        public abstract Expression<Func<T, bool>> SearchExpression(string searchField = "");
    }
}