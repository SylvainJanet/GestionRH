﻿//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using RepositoriesAndServices.Repositories;
//using RepositoriesAndServices.Services;
//using System;
//using System.Diagnostics;
//using TestGenericRepositoryAndService.TestInterfaces;

//namespace TestGenericRepositoryAndService.TestUserService.GenericCRUD
//{
//    [TestClass]
//    public class TestUserGenericCreate : BaseTest, ITestCreate
//    {
//        UserService _UserServiceToTest = new UserService(new UserRepository(new MyDbContext()));

//        [ClassCleanup]
//        public static void ClassCleanup()
//        {
//            DBTestData.DBTestData.EmptyDb();
//        }

//        [ClassInitialize]
//        public static void ClassInitialize(TestContext testContext)
//        {
//            string message = "---------- " + testContext.FullyQualifiedTestClassName + " ----------";
//            Debug.WriteLine(message);
//            testContext.WriteLine(message);
//            DBTestData.DBTestData.EmptyDb();
//        }

//        [TestMethod]
//        [TestCategory("User")]
//        [TestProperty("CRUD", "Create")]
//        [Owner("Sylvain")]
//        [ExpectedException(typeof(NotImplementedException))]
//        public void Test_SaveCrypted_SaveSuccessfull()
//        {
//            Trace("...Testing... " + Class + " : " + Method);
//            DBTestData.DBTestData.ResetDB();

//            Address a = new Address(1, "street", "city", 1234, "country");
//            Address b = new Address(1, "street", "city", 1234, "country");

//            using (MyDbContext db = new MyDbContext())
//            {
//                var watch = System.Diagnostics.Stopwatch.StartNew();
//                db.Addresses.Add(a);
//                watch.Stop();
//                var elapsedTicks = watch.ElapsedTicks;
//                Trace("Specific method time : " + elapsedTicks + " ticks");
//            }

//            DBTestData.DBTestData.ResetDB();

//            long oldcount = DBTestData.DBTestData.TotalCount();
//            var watch2 = System.Diagnostics.Stopwatch.StartNew();
//            _AddressServiceToTest.SaveCrypted(a);
//            watch2.Stop();
//            var elapsedTicks2 = watch2.ElapsedTicks;
//            Trace("Generic method time : " + elapsedTicks2 + " ticks");

//            long newCount = DBTestData.DBTestData.TotalCount();
//            if (oldcount + 1 != newCount)
//                Assert.Fail();
//            Assert.AreEqual(a, b);
//        }

//        [TestMethod]
//        [TestCategory("User")]
//        [TestProperty("CRUD", "Create")]
//        [Owner("Sylvain")]
//        [ExpectedException(typeof(NotImplementedException))]
//        public void Test_Save_SaveSuccessfull()
//        {
//            Trace("...Testing... " + Class + " : " + Method);
//            DBTestData.DBTestData.ResetDB();

//            Address a = new Address(1, "street", "city", 1234, "country");

//            using (MyDbContext db = new MyDbContext())
//            {
//                var watch = System.Diagnostics.Stopwatch.StartNew();
//                db.Addresses.Add(a);
//                watch.Stop();
//                var elapsedTicks = watch.ElapsedTicks;
//                Trace("Specific method time : " + elapsedTicks + " ticks");
//            }

//            DBTestData.DBTestData.ResetDB();

//            long oldcount = DBTestData.DBTestData.TotalCount();
//            var watch2 = System.Diagnostics.Stopwatch.StartNew();
//            _AddressServiceToTest.Save(a);
//            watch2.Stop();
//            var elapsedTicks2 = watch2.ElapsedTicks;
//            Trace("Generic method time : " + elapsedTicks2 + " ticks");

//            long newCount = DBTestData.DBTestData.TotalCount();
//            Assert.AreEqual(oldcount + 1, newCount);
//        }
//    }
//}
