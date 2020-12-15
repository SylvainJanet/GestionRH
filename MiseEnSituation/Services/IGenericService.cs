using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MiseEnSituation.Services
{
    public interface IGenericService<T> where T : class
    {
        IQueryable<T> Collection(bool isIncludes, bool isTracked);
        IQueryable<T> CollectionExcludes();
        IQueryable<T> CollectionExcludesTracked();
        IQueryable<T> CollectionIncludes();
        IQueryable<T> CollectionIncludesTracked();
        long Count(Expression<Func<T, bool>> pedicateWhere = null);
        void Delete(params object[] objs);
        void Delete(T t);
        List<T> FindAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        List<T> FindAllExcludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        List<T> FindAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        List<T> FindAllIncludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        List<T> FindAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        T FindById(bool isIncludes, bool isTracked, params object[] objs);
        T FindByIdExcludes(params object[] objs);
        T FindByIdExcludesTracked(params object[] objs);
        T FindByIdIncludes(params object[] objs);
        T FindByIdIncludesTracked(params object[] objs);
        List<T> FindByManyId(bool isInclude, bool isTracked, params object[] objs);
        List<T> FindManyByIdExcludes(params object[] objs);
        List<T> FindManyByIdExcludesTracked(params object[] objs);
        List<T> FindManyByIdIncludes(params object[] objs);
        List<T> FindManyByIdIncludesTracked(params object[] objs);
        List<T> GetAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllBy(bool isIncludes, bool isTracked, Expression<Func<T, bool>> predicateWhere);
        List<T> GetAllByExcludes(Expression<Func<T, bool>> predicateWhere);
        List<T> GetAllByExcludesTracked(Expression<Func<T, bool>> predicateWhere);
        List<T> GetAllByIncludes(Expression<Func<T, bool>> predicateWhere);
        List<T> GetAllByIncludesTracked(Expression<Func<T, bool>> predicateWhere);
        List<T> GetAllExcludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllIncludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> List(bool isIncludes, bool isTracked);
        List<T> ListExcludes();
        List<T> ListExcludesTracked();
        List<T> ListIncludes();
        List<T> ListIncludesTracked();
        bool NextExist(int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> OrderExpression();
        void Save(T t);
        Expression<Func<T, bool>> SearchExpression(string searchField = "");
        void Update(T t);
        void UpdateOne(T t, string propertyName, object newValue);
    }
}