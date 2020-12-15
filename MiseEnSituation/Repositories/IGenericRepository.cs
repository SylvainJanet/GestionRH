using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MiseEnSituation.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T t);
        IQueryable<T> Collection(bool isIncludes, bool isTracked);
        IQueryable<T> CollectionExcludes();
        IQueryable<T> CollectionExcludesTracked();
        IQueryable<T> CollectionIncludes();
        IQueryable<T> CollectionIncludesTracked();
        void Commit();
        long Count(Expression<Func<T, bool>> predicateWhere = null);
        void Delete(params object[] objs);
        void Delete(T t);
        T FindById(bool isIncludes, bool isTracked, params object[] objs);
        T FindByIdExcludes(params object[] objs);
        T FindByIdExcludesTracked(params object[] objs);
        T FindByIdIncludes(params object[] objs);
        T FindByIdIncludesTracked(params object[] objs);
        List<T> GetAll(bool isIncludes, bool isTracked, int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllBy(bool isIncludes, bool isTracked, Expression<Func<T, bool>> predicateWhere);
        List<T> GetAllByExcludes(Expression<Func<T, bool>> predicateWhere);
        List<T> GetAllByExcludesTracked(Expression<Func<T, bool>> predicateWhere);
        List<T> GetAllByIncludes(Expression<Func<T, bool>> predicateWhere);
        List<T> GetAllByIncludesTracked(Expression<Func<T, bool>> predicateWhere);
        List<T> GetAllExcludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllExcludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllIncludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllIncludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> List(bool isIncludes, bool isTracked);
        List<T> ListExcludes();
        List<T> ListExcludesTracked();
        List<T> ListIncludes();
        List<T> ListIncludesTracked();
        void Modify(T t);
        void Remove(params object[] objs);
        void Remove(T t);
        void Save(T t, params object[] objs);
        void Update(T t, params object[] objs);
        void UpdateOne(T t, string propertyName, object newValue);
    }
}