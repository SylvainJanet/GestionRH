using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenericRepositoryAndService.TestInterfaces
{
    public interface ITestDelete
    {
        //void Delete(params object[] objs);
        void Test_Delete_objs_DeleteSuccessfull();
        void Test_Delete_objs_ExceptionArgumentsNotKeys();
        void Test_Delete_objs_ExceptionElementNotFound();

        //void Delete(T t);
        void Test_Delete_T_DeleteSuccessfullForElement();
        void Test_Delete_T_ExceptionElementNotFound();
        void Test_UpdateOne_UpdateSuccessfullForDependent();
    }
}
