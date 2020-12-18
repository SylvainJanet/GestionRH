using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestGenericRepositoryAndService.TestInterfaces;

namespace TestGenericRepositoryAndService.TestCheckUpService.GenericCRUD
{
    [TestClass]
    public class TestCheckUpGenericCreate : BaseTest, ITestCreate, ITest
    {
        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void ClassCleanup()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void ClassInitialize()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_SaveCrypted_SaveSuccessfull()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Save_SaveSuccessfull()
        {
            throw new NotImplementedException();
        }
    }
}
