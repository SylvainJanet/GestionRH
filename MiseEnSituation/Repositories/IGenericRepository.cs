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
        void Update(T t, params object[] objs);

        //Delete
        //id / T  //  no Commit / Commit
        void Remove(int id);
        void Remove(T t);
        void Delete(int id);
        void Delete(T t);

        //Count
        long Count(Expression<Func<T, bool>> predicateWhere = null);

        //override these methods to give PropertyName, Type of Many-to-Many or one-to-Many relationship
        //protected abstract Dictionary<string, Type> SetDynamicDBListTypes();
        //protected abstract Dictionary<string, Type> SetDynamicDBTypes();
    }
}