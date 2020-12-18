using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;
using System;
using System.Diagnostics;
using TestGenericRepositoryAndService.TestInterfaces;

namespace TestGenericRepositoryAndService.TestCompanyService.GenericCRUD
{
    [TestClass]
    public class TestCompanyGenericRead : BaseTest, ITestRead
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
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Collection_CollectionSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Count_CountSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Count_ExceptionIncorrectPredicate()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_FindAll_Successfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_FindById_ExceptionIncorrectArgument()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_FindById_NullResultNotFound()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_FindById_Successfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_FindManyById_ExceptionIncorrectArgument()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_FindManyById_NullResultNotFound()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_FindManyById_Successfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_GetAllBy_ExceptionIncorrectPredicate()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_GetAllBy_Successfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_GetAll_ExceptionIncorrectPredicate()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_GetAll_Successfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_List_Successfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Read")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_NextExist_Successfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }
    }
}
