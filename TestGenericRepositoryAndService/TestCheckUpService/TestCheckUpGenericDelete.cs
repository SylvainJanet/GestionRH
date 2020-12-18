using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;
using System;
using System.Diagnostics;
using TestGenericRepositoryAndService.TestInterfaces;

namespace TestGenericRepositoryAndService.TestCheckUpService.GenericCRUD
{
    [TestClass]
    public class TestCheckUpGenericDelete : BaseTest, ITestDelete
    {
        CheckUpService _CheckUpServiceToTest = new CheckUpService(new CheckUpRepository(new MyDbContext()));

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DBTestData.DBTestData.EmptyDb();
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            string message = "---------- " + testContext.FullyQualifiedTestClassName + " ----------";
            Debug.WriteLine(message);
            testContext.WriteLine(message);
            DBTestData.DBTestData.EmptyDb();
        }

        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_objs_DeleteSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_objs_ExceptionArgumentsNotKeys()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_objs_ExceptionElementNotFound()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_T_DeleteSuccessfullForElement()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Delete_T_ExceptionElementNotFound()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Delete")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_UpdateOne_UpdateSuccessfullForDependent()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }
    }
}
