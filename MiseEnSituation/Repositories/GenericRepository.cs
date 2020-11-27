using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected MyDbContext DataContext;
        protected DbSet<T> dbSet;

        public GenericRepository(MyDbContext DataContext)
        {
            this.DataContext = DataContext;
            this.dbSet = DataContext.Set<T>();
        }

        public virtual void Add(T t)
        {
            dbSet.Add(t);
        }

        public virtual IQueryable<T> Collection()
        {
            return dbSet.AsNoTracking();
        }

        public virtual IQueryable<T> CollectionTracked()
        {
            return dbSet;
        }

        public void Commit()
        {
            DataContext.SaveChanges();
        }

        public long Count(Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req = Collection();

            if (predicateWhere != null)
                req = req.Where(predicateWhere);

            return req.Count();
        }

        public void Delete(int id)
        {
            Remove(id);
            Commit();
        }

        public void Delete(T t)
        {
            Remove(t);
            Commit();
        }

        public T FindById(int id)
        {
            return Collection().SingleOrDefault(t => t.Id == id);
        }

        public T FindByIdTracked(int id)
        {
            return CollectionTracked().SingleOrDefault(t => t.Id == id);
        }

        protected IQueryable<T> WhereSkipTake(IQueryable<T> orderedreq, int start, int maxByPage, Expression<Func<T, bool>> predicateWhere)
        {
            if (predicateWhere != null)
                orderedreq = orderedreq.Where(predicateWhere);

            orderedreq = orderedreq.Skip(start).Take(maxByPage);
            return orderedreq;
        }

        public List<T> GetAll(int start = 0, int maxByPage = int.MaxValue, Expression<Func<T, int?>> keyOrderBy = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req;
            if (keyOrderBy != null)
                req = Collection().OrderBy(keyOrderBy);
            else
                req = Collection().OrderBy(t => t.Id);

            req = WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        public List<T> GetAllTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<T, int?>> keyOrderBy = null, Expression<Func<T, bool>> predicateWhere = null)
        {
            IQueryable<T> req;
            if (keyOrderBy != null)
                req = CollectionTracked().OrderBy(keyOrderBy);
            else
                req = CollectionTracked().OrderBy(t => t.Id);

            req = WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        public List<T> List()
        {
            return Collection().ToList();
        }

        public List<T> ListTracked()
        {
            return CollectionTracked().ToList();
        }

        public virtual void Modify(T t)
        {
            if (DataContext.Entry(t).State == EntityState.Detached)
            {
                dbSet.Attach(t);
            }
            DataContext.Entry(t).State = EntityState.Modified;
        }

        public void Remove(int id)
        {
            Remove(FindById(id));
        }

        public void Remove(T t)
        {
            if (DataContext.Entry(t).State == EntityState.Detached)
            {
                dbSet.Attach(t);
            }
            dbSet.Remove(t);
        }

        public void Save(T t)
        {
            Add(t);
            Commit();
            //If in Many-to-Many relationship, create method :
            //public void Save(T t, List<A> requiredParam, List<B> nullableParam = null)
            //{
            //    nullableParam = nullableParam ?? new List<B>();

            //    using (MyDbContext newContext = new MyDbContext())
            //    {
            //        List<A> newRequiredParam = GetFromNewContext(requiredParam, newContext);
            //        List<B> newNullableParam = GetFromNewContext(nullableParam, newContext);

            //        t.requiredParam = newRequiredParam;
            //        t.nullableParam = newNullableParam;

            //        newContext.Set<T>().Add(t);
            //        DetachProperties(t);
            //        newContext.SaveChanges();
            //    }
            //}
        }

        public void Update(T t)
        {
            Modify(t);
            Commit();
            //If in Many-to-Many relationship, create method :
            //public void Update(T t, List<A> requiredParam, List<B> nullableParam = null)
            //{
            //    nullableParam = nullableParam ?? new List<B>();

            //    using (MyDbContext newContext = new MyDbContext())
            //    {
            //        T tToChange = newContext.TrainingCourses.Include(t => t.requiredParam)
            //                                                .Include(t => t.nullableParam)
            //                                                .SingleOrDefault(tt => tt.Id == t.Id);

            //        tToChange.EveryOtherParam = t.EveryOtherParam;

            //        List<A> newRequiredParam = GetFromNewContext(requiredParam, newContext);
            //        List<B> newNullableParam = GetFromNewContext(nullableParam, newContext);

            //        tToChange.requiredParam = newRequiredParam;
            //        tToChange.nullableParam = newNullableParam;

            //        newContext.Entry(tToChange).State = EntityState.Modified;

            //        newContext.SaveChanges();
            //    }
            //}
        }

        protected List<TT> GetFromNewContext<TT>(List<TT> ts, MyDbContext newdbContext) where TT : BaseEntity
        {
            List<TT> res = new List<TT>();
            foreach (TT t in ts)
            {
                res.Add(newdbContext.Set<TT>().Find(t.Id));
            }
            return res;
        }

        protected virtual void DetachProperties(T t)
        {
            //For every properties used for a many-to-many relationship
            //
            //foreach (Property property in t.properties)
            //{
            //    DataContext.Entry(property).State = EntityState.Unchanged;
            //}
        }
    }
}