using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestGenericRepositoryAndService.TestInterfaces;

namespace TestGenericRepositoryAndService.TestCompanyService.GenericCRUD
{
    [TestClass]
    public class TestCompanyGenericDelete : BaseTest, ITestDelete, ITest
    {
        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void ClassCleanup()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void ClassInitialize()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_objs_DeleteSuccessfull()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_objs_ExceptionArgumentsNotKeys()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_objs_ExceptionElementNotFound()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_T_DeleteSuccessfullForElement()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_T_ExceptionElementNotFound()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_UpdateOne_UpdateSuccessfullForDependent()
        {
            throw new NotImplementedException();
        }
    }
}
