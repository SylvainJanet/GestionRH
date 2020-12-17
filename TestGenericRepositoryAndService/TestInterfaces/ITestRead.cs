using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenericRepositoryAndService.TestInterfaces
{
    public interface ITestRead
    {
        //IQueryable<T> Collection(bool isIncludes, bool isTracked);
        void Test_Collection_CollectionSuccessfull();

        //long Count(Expression<Func<T, bool>> predicateWhere = null);
        void Test_Count_CountSuccessfull();
        void Test_Count_ExceptionIncorrectPredicate();

        //List<T> FindAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        void Test_FindAll_Successfull();

        //T FindById(bool isIncludes, bool isTracked, params object[] objs);
        void Test_FindById_Successfull();
        void Test_FindById_ExceptionIncorrectArgument();
        void Test_FindById_NullResultNotFound();

        //List<T> FindByManyId(bool isIncludes, bool isTracked, params object[] objs);
        void Test_FindManyById_Successfull();
        void Test_FindManyById_ExceptionIncorrectArgument();
        void Test_FindManyById_NullResultNotFound();

        //List<T> GetAll(bool isIncludes, bool isTracked, int page = 1, int maxByPage = int.MaxValue, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderreq = null, Expression<Func<T, bool>> predicateWhere = null);
        void Test_GetAll_Successfull();
        void Test_GetAll_ExceptionIncorrectPredicate();

        //List<T> GetAllBy(bool isIncludes, bool isTracked, Expression<Func<T, bool>> predicateWhere);
        void Test_GetAllBy_Successfull();
        void Test_GetAllBy_ExceptionIncorrectPredicate();

        //List<T> List(bool isIncludes, bool isTracked);
        void Test_List_Successfull();

        //bool NextExist(int page = 1, int maxByPage = int.MaxValue, string searchField = "");
        void Test_NextExist_Successfull();
    }
}
