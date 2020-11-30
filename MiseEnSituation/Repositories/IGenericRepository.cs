using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MiseEnSituation.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        void Commit();

        //Create
        //no Commit / Commit
        void Add(T t);
        void Save(T t, params object[] objs);

        //Read
        //IQueryable / List  no tracking / tracking  Includes / Excludes  //  page query no tracking / page query tracked  Includes / Excludes  //  id no tracking / tracked  Includes / Excludes
        IQueryable<T> CollectionExcludes();
        IQueryable<T> CollectionExcludesTracked();
        IQueryable<T> CollectionIncludes();
        IQueryable<T> CollectionIncludesTracked();
        List<T> ListExcludes();
        List<T> ListExcludesTracked();
        List<T> ListIncludes();
        List<T> ListIncludesTracked();
        List<T> GetAllExcludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllExcludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllIncludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllIncludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        T FindByIdExcludes(int id);
        T FindByIdExcludesTracked(int id);
        T FindByIdIncludes(int id);
        T FindByIdIncludesTracked(int id);

        //Update
        //no Commit / Commit
        void Modify(T t);
        void Update(T t, params object[] objs);

        //Delete
        //id / T  //  no Commit / Commit
        void Remove(int id);
        void Remove(T t);
        void Delete(int id);
        void Delete(T t);

        //Count
        long Count(Expression<Func<T, bool>> predicateWhere = null);
    }
}