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
        void Save(T t);

        //Read
        //IQueryable List no tracking / tracking  //  page query no tracking / page query tracked  //  id no tracking / id tracked
        IQueryable<T> Collection();
        IQueryable<T> CollectionTracked();
        List<T> List();
        List<T> ListTracked();
        List<T> GetAll(int start = 0, int maxByPage = int.MaxValue, Expression<Func<T, int?>> keyOrderBy = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<T, int?>> keyOrderBy = null, Expression<Func<T, bool>> predicateWhere = null);
        T FindById(int id);
        T FindByIdTracked(int id);

        //Update
        //no Commit / Commit
        void Modify(T t);
        void Update(T t);

        //Delete
        //id / T  //  no Commit / Commit
        void Remove(int id);
        void Remove(T t);
        void Delete(int id);
        void Delete(T t);

        //Count
        long Count(Expression<Func<T, bool>> predicateWhere = null);

        //Tools to Add/Update when T is in Many-to-Many relationship (private and virtual but necessary to override
        //along with add, save, modify and update)

        //List<TT> GetFromNewContext<TT>(List<TT> ts, MyDbContext newdbContext) where TT : BaseEntity;
        //void DetachProperties(T t);
    }
}