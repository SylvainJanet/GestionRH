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
        //no Commit / Commit
        void Save(T t);

        //Read
        //IQueryable List no tracking / tracking  //  page query no tracking / page query tracked  //  id no tracking / id tracked
        IQueryable<T> Collection();
        IQueryable<T> CollectionTracked();
        List<T> List();
        List<T> ListTracked();
        List<T> GetAll(int start = 0, int maxByPage = int.MaxValue, Expression<Func<T, int?>> keyOrderBy = null, Expression<Func<T, bool>> predicateWhere = null);
        List<T> GetAllTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<T, int?>> keyOrderBy = null, Expression<Func<T, bool>> predicateWhere = null);
        T FindById(int? id);
        T FindByIdTracked(int? id);

        //Update
        //no Commit / Commit
        void Update(T t);

        //Delete
        //id / T  //  no Commit / Commit
        void Delete(int? id);
        void Delete(T t);

        //Count
        long Count(Expression<Func<T, bool>> predicateWhere = null);
    }
}