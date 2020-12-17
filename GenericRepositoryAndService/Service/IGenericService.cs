using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GenericRepositoryAndService.Tools.Generic;

namespace GenericRepositoryAndService.Service
{
    ///<include file='docs.xml' path='doc/members/member[@name="T:GenericRepositoryAndService.Service.IGenericService`1"]/*'/>
    public interface IGenericService<T> where T : class
    {
        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.Collection(System.Boolean,System.Boolean)"]/*'/>
        IQueryable<T> Collection(bool isIncludes, bool isTracked);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.CollectionExcludes"]/*'/>
        IQueryable<T> CollectionExcludes();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.CollectionExcludesTracked"]/*'/>
        IQueryable<T> CollectionExcludesTracked();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.CollectionIncludes"]/*'/>
        IQueryable<T> CollectionIncludes();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.CollectionIncludesTracked"]/*'/>
        IQueryable<T> CollectionIncludesTracked();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.Count(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        long Count(Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.Delete(System.Object[])"]/*'/>
        void Delete(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.Delete(`0)"]/*'/>
        void Delete(T t);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindAll(System.Boolean,System.Boolean,System.Int32,System.Int32,System.String)"]/*'/>
        List<T> FindAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, string searchField = "");

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindAllExcludes(System.Int32,System.Int32,System.String)"]/*'/>
        List<T> FindAllExcludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindAllExcludesTracked(System.Int32,System.Int32,System.String)"]/*'/>
        List<T> FindAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindAllIncludes(System.Int32,System.Int32,System.String)"]/*'/>
        List<T> FindAllIncludes(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindAllIncludesTracked(System.Int32,System.Int32,System.String)"]/*'/>
        List<T> FindAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindById(System.Boolean,System.Boolean,System.Object[])"]/*'/>
        T FindById(bool isIncludes, bool isTracked, params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindByIdExcludes(System.Object[])"]/*'/>
        T FindByIdExcludes(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindByIdExcludesTracked(System.Object[])"]/*'/>
        T FindByIdExcludesTracked(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindByIdIncludes(System.Object[])"]/*'/>
        T FindByIdIncludes(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindByIdIncludesTracked(System.Object[])"]/*'/>
        T FindByIdIncludesTracked(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindByManyId(System.Boolean,System.Boolean,System.Object[])"]/*'/>
        List<T> FindByManyId(bool isIncludes, bool isTracked, params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindManyByIdExcludes(System.Object[])"]/*'/>
        List<T> FindManyByIdExcludes(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindManyByIdExcludesTracked(System.Object[])"]/*'/>
        List<T> FindManyByIdExcludesTracked(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindManyByIdIncludes(System.Object[])"]/*'/>
        List<T> FindManyByIdIncludes(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.FindManyByIdIncludesTracked(System.Object[])"]/*'/>
        List<T> FindManyByIdIncludesTracked(params object[] objs);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.GetAll(System.Boolean,System.Boolean,System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.GetAllBy(System.Boolean,System.Boolean,System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllBy(bool isIncludes, bool isTracked, Expression<Func<T, bool>> predicateWhere);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.GetAllByExcludes(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllByExcludes(Expression<Func<T, bool>> predicateWhere);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.GetAllByExcludesTracked(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllByExcludesTracked(Expression<Func<T, bool>> predicateWhere);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.GetAllByIncludes(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllByIncludes(Expression<Func<T, bool>> predicateWhere);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.GetAllByIncludesTracked(System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllByIncludesTracked(Expression<Func<T, bool>> predicateWhere);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.GetAllExcludes(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllExcludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.GetAllExcludesTracked(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllExcludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.GetAllIncludes(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllIncludes(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.GetAllIncludesTracked(System.Int32,System.Int32,System.Linq.Expressions.Expression{System.Func{System.Linq.IQueryable{`0},System.Linq.IOrderedQueryable{`0}}},System.Linq.Expressions.Expression{System.Func{`0,System.Boolean}})"]/*'/>
        List<T> GetAllIncludesTracked(int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.List(System.Boolean,System.Boolean)"]/*'/>
        List<T> List(bool isIncludes, bool isTracked);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.ListExcludes"]/*'/>
        List<T> ListExcludes();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.ListExcludesTracked"]/*'/>
        List<T> ListExcludesTracked();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.ListIncludes"]/*'/>
        List<T> ListIncludes();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.ListIncludesTracked"]/*'/>
        List<T> ListIncludesTracked();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.NextExist(System.Int32,System.Int32,System.String)"]/*'/>
        bool NextExist(int page = 1, int maxByPage = int.MaxValue, string searchField = "");

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.OrderExpression"]/*'/>
        Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> OrderExpression();

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.Save(`0)"]/*'/>
        void Save(T t);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.SaveCrypted(`0)"]/*'/>
        void SaveCrypted(T t);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.SearchExpression(System.String)"]/*'/>
        Expression<Func<T, bool>> SearchExpression(string searchField = "");

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.Update(`0)"]/*'/>
        void Update(T t);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.UpdateCrypted(`0)"]/*'/>
        void UpdateCrypted(T t);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.UpdateOne(`0,System.String,System.Object)"]/*'/>
        void UpdateOne(T t, string propertyName, object newValue);

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Service.IGenericService`1.UpdateOneCrypted(`0,System.String,System.Object)"]/*'/>
        void UpdateOneCrypted(T t, string propertyName, object newValue);
    }
}