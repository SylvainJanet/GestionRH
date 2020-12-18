using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;
using System;
using System.Diagnostics;
using TestGenericRepositoryAndService.TestInterfaces;

namespace TestGenericRepositoryAndService.TestUserService.GenericCRUD
{
    [TestClass]
    public class TestUserGenericCreate : BaseTest, ITestCreate
    {
        UserService _UserServiceToTest = new UserService(new UserRepository(new MyDbContext()));

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
        [TestCategory("User")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_SaveCrypted_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("User")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Save_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }
    }
}
