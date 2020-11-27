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
    public class GenericService<T> : IGenericService<T> where T : BaseEntity
    {
        protected IGenericRepository<T> _repository;

        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public IQueryable<T> Collection()
        {
            return _repository.Collection();
        }

        public IQueryable<T> CollectionTracked()
        {
            return _repository.CollectionTracked();
        }

        public long Count(Expression<Func<T, bool>> pedicateWhere = null)
        {
            return _repository.Count(pedicateWhere);
        }

        public void Delete(int? id)
        {
            if (!id.HasValue)
                throw new IdNullException(typeof(T));
            _repository.Delete(id.Value);
        }

        public void Delete(T t)
        {
            _repository.Delete(t);
        }

        public T FindById(int? id)
        {
            if (!id.HasValue)
                throw new IdNullException(typeof(T));
            return _repository.FindById(id.Value);
        }

        public T FindByIdTracked(int? id)
        {
            if (!id.HasValue)
                throw new IdNullException(typeof(T));
            return _repository.FindByIdTracked(id.Value);
        }

        public List<T> GetAll(int start = 0, int maxByPage = int.MaxValue, Expression<Func<T, int?>> keyOrderBy = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return _repository.GetAll(start, maxByPage, keyOrderBy, predicateWhere);
        }

        public List<T> GetAllTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<T, int?>> keyOrderBy = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            return _repository.GetAllTracked(start, maxByPage, keyOrderBy, predicateWhere);
        }

        public List<T> List()
        {
            return _repository.List();
        }

        public List<T> ListTracked()
        {
            return _repository.ListTracked();
        }

        public void Save(T t)
        {
            _repository.Save(t);
        }

        public void Update(T t)
        {
            _repository.Update(t);
        }
    }
}