using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;
using System;
using System.Diagnostics;
using TestGenericRepositoryAndService.TestInterfaces;

namespace TestGenericRepositoryAndService.TestCompanyService.GenericCRUD
{
    [TestClass]
    public class TestCompanyGenericUpdate : BaseTest, ITestUpdate
    {
        CompanyService _CompanyServiceToTest = new CompanyService(new CompanyRepository(new MyDbContext()));

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
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_UpdateCrypted_UpdateSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_UpdateOneCrypted_UpdateSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Updateone_ExceptionElementNotInDB()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_UpdateOne_ExceptionNewValueIncorrect()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_UpdateOne_ExceptionPropNameInvalid()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_UpdateOne_ExceptionPropReadOnly()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_UpdateOne_UpdateSuccessfullForDependent()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_UpdateOne_UpdateSuccessfullForElement()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Update_ExceptionElementNotInDB()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Update_UpdateSuccessfullForDependent()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Update")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Update_UpdateSuccessfullForElement()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }
    }
}
