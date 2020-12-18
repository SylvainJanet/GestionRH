using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGenericRepositoryAndService.TestInterfaces
{
    public interface ITestCreate
    {
        //void Save(T t);
        void Test_Save_SaveSuccessfull();

        //void SaveCrypted(T t);
        void Test_SaveCrypted_SaveSuccessfull();
    }
}
