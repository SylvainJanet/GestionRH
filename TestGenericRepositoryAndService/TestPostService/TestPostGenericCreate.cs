using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestGenericRepositoryAndService.TestInterfaces;

namespace TestGenericRepositoryAndService.TestPostService.GenericCRUD
{
    [TestClass]
    public class TestPostGenericCreate : BaseTest, ITestCreate, ITest
    {
        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void ClassCleanup()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void ClassInitialize()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_SaveCrypted_SaveSuccessfull()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Post")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Save_SaveSuccessfull()
        {
            throw new NotImplementedException();
        }
    }
}
