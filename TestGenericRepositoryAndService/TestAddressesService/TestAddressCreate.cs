using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using MiseEnSituation.Services;
using System;
using System.Diagnostics;
using TestGenericRepositoryAndService.TestInterfaces;

namespace TestGenericRepositoryAndService.TestAddressesService.GenericCRUD
{
    [TestClass]
    public class TestAddressCreate : BaseTest, ITestCreate
    {
        AddressService _AddressServiceToTest = new AddressService(new AddressRepository(new MyDbContext()));

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
        [TestCategory("Address")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_SaveCrypted_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            throw new NotImplementedException();
        }

        [TestMethod]
        [TestCategory("Address")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        public void Test_Save_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            DBTestData.DBTestData.ResetDB();
            Address a = new Address(1, "street", "city", 1234, "country");
            using (MyDbContext db = new MyDbContext())
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                watch.Stop();
                var elapsedTicks = watch.ElapsedTicks;
                Trace("Specific method time : " + elapsedTicks);
            }
            DBTestData.DBTestData.ResetDB();
            long oldcount = DBTestData.DBTestData.TotalCount();
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            _AddressServiceToTest.Save(a);
            watch2.Stop();
            var elapsedTicks2 = watch2.ElapsedTicks;
            Trace("Generic method time : " + elapsedTicks2);
            long newCount = DBTestData.DBTestData.TotalCount();
            Assert.AreEqual(oldcount + 1, newCount);
        }
    }
}
