using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestGenericRepositoryAndService.TestInterfaces;

namespace TestGenericRepositoryAndService.TestSkillService.GenericCRUD
{
    [TestClass]
    public class TestSkillGenericDelete : BaseTest, ITestDelete, ITest
    {
        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void ClassCleanup()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void ClassInitialize()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_objs_DeleteSuccessfull()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_objs_ExceptionArgumentsNotKeys()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_objs_ExceptionElementNotFound()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_T_DeleteSuccessfullForElement()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_T_ExceptionElementNotFound()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_UpdateOne_UpdateSuccessfullForDependent()
        {
            throw new NotImplementedException();
        }
    }
}
