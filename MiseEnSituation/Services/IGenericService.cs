using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MiseEnSituation.Services
{
    public interface IGenericService<T> where T : BaseEntity
    {
        //Create
        void Save(T t, params object[] objs);

        //Read
        //IQueryable List no tracking / tracking  //  page query no tracking / page query tracked  //  id no tracking / id tracked
        IQueryable<T> CollectionExcludes();
        IQueryable<T> CollectionExcludesTracked();
        IQueryable<T> CollectionIncludes();
        IQueryable<T> CollectionIncludesTracked();
        List<T> ListExcludes();
        List<T> ListExcludesTracked();
        List<T> ListIncludes();
        List<T> ListIncludesTracked();
        List<T> GetAllExcludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllIncludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> FindAllExcludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        List<T> FindAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        List<T> FindAllIncludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        List<T> FindAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        T FindByIdExcludes(int? id);
        T FindByIdExcludesTracked(int? id);
        T FindByIdIncludes(int? id);
        T FindByIdIncludesTracked(int? id);
        List<T> FindManyByIdExcludes(int?[] ids);
        List<T> FindManyByIdExcludesTracked(int?[] ids);
        List<T> FindManyByIdIncludes(int?[] ids);
        List<T> FindManyByIdIncludesTracked(int?[] ids);

        //Update
        void Update(T t, params object[] objs);
        void UpdateOne(T t, string properyname, object newValue);

        //Delete
        //id / T
        void Delete(int? id);
        void Delete(T t);

        //Count
        long Count(Expression<Func<T, bool>> predicateWhere = null);

        bool NextExist(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

        //Override the following to specify what predicate is used for ordering and search
        //Find all will call get all with those predicates

        Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> OrderExpression();
        Expression<Func<T, bool>> SearchExpression(string searchField = "");
    }
}