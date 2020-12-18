using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenericRepositoryAndService.TestInterfaces
{
    public interface ITestUpdate
    {
        //void Update(T t);
        void Test_Update_UpdateSuccessfullForElement();
        void Test_Update_ExceptionElementNotInDB();
        void Test_Update_UpdateSuccessfullForDependent();

        //void UpdateCrypted(T t);
        void Test_UpdateCrypted_UpdateSuccessfull();

        //void UpdateOne(T t, string propertyName, object newValue);
        void Test_UpdateOne_UpdateSuccessfullForElement();
        void Test_Updateone_ExceptionElementNotInDB();
        void Test_UpdateOne_UpdateSuccessfullForDependent();
        void Test_UpdateOne_ExceptionPropNameInvalid();
        void Test_UpdateOne_ExceptionPropReadOnly();
        void Test_UpdateOne_ExceptionNewValueIncorrect();

        //void UpdateOneCrypted(T t, string propertyName, object newValue);
        void Test_UpdateOneCrypted_UpdateSuccessfull();
    }
}
