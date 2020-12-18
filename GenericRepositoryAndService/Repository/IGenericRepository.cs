using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GenericRepositoryAndService.Tools.Generic;

namespace GenericRepositoryAndService.Repository
{
    ///<include file='docs.xml' path='doc/members/member[@name="T:GenericRepositoryAndService.Repository.IGenericRepository`1"]/*'/>
    public interface IGenericRepository<T> where T : class
    {
        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Add(`0)"]/*'/>
        void Add(T t);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Collection(System.Boolean,System.Boolean)"]/*'/>
        IQueryable<T> Collection(bool isIncludes, bool isTracked);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.CollectionExcludes"]/*'/>
        IQueryable<T> CollectionExcludes();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.CollectionExcludesTracked"]/*'/>
        IQueryable<T> CollectionExcludesTracked();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.CollectionIncludes"]/*'/>
        IQueryable<T> CollectionIncludes();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.CollectionIncludesTracked"]/*'/>
        IQueryable<T> CollectionIncludesTracked();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Commit"]/*'/>
        void Commit();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Count(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        long Count(Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Delete(System.Object[])"]/*'/>
        void Delete(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Delete(`0)"]/*'/>
        void Delete(T t);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.FindById(System.Boolean,System.Boolean,System.Object[])"]/*'/>
        T FindById(bool isIncludes, bool isTracked, params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.FindByIdExcludes(System.Object[])"]/*'/>
        T FindByIdExcludes(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.FindByIdExcludesTracked(System.Object[])"]/*'/>
        T FindByIdExcludesTracked(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.FindByIdIncludes(System.Object[])"]/*'/>
        T FindByIdIncludes(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.FindByIdIncludesTracked(System.Object[])"]/*'/>
        T FindByIdIncludesTracked(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.GetAll(System.Boolean,System.Boolean,System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAll(bool isIncludes, bool isTracked, int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.GetAllBy(System.Boolean,System.Boolean,System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllBy(bool isIncludes, bool isTracked, Expression<Func<T, bool>> predicateWhere);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.GetAllByExcludes(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllByExcludes(Expression<Func<T, bool>> predicateWhere);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.GetAllByExcludesTracked(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllByExcludesTracked(Expression<Func<T, bool>> predicateWhere);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.GetAllByIncludes(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllByIncludes(Expression<Func<T, bool>> predicateWhere);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.GetAllByIncludesTracked(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllByIncludesTracked(Expression<Func<T, bool>> predicateWhere);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.GetAllExcludes(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllExcludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.GetAllExcludesTracked(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllExcludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.GetAllIncludes(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllIncludes(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.GetAllIncludesTracked(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllIncludesTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.List(System.Boolean,System.Boolean)"]/*'/>
        List<T> List(bool isIncludes, bool isTracked);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.ListExcludes"]/*'/>
        List<T> ListExcludes();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.ListExcludesTracked"]/*'/>
        List<T> ListExcludesTracked();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.ListIncludes"]/*'/>
        List<T> ListIncludes();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.ListIncludesTracked"]/*'/>
        List<T> ListIncludesTracked();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Modify(`0)"]/*'/>
        void Modify(T t);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Remove(System.Object[])"]/*'/>
        void Remove(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Remove(`0)"]/*'/>
        void Remove(T t);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Save(`0,System.Object[])"]/*'/>
        void Save(T t, params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.Update(`0,System.Object[])"]/*'/>
        void Update(T t, params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Repository.IGenericRepository`1.UpdateOne(`0,System.String,System.Object)"]/*'/>
        void UpdateOne(T t, string propertyName, object newValue);
    }
}