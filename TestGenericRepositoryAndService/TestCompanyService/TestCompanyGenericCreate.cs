using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Models;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;
using System;
using System.Diagnostics;
using TestGenericRepositoryAndService.TestInterfaces;
using System.Data.Entity;
using System.Linq;

namespace TestGenericRepositoryAndService.TestCompanyService.GenericCRUD
{
    [TestClass]
    public class TestCompanyGenericCreate : BaseTest, ITestCreate
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
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        public void Test_SaveCrypted_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            DBTestData.DBTestData.ResetDB();

            Company a = new Company();
            Company b = new Company();
            a.Name = b.Name = "Test";

            using (MyDbContext db = new MyDbContext())
            {
                a.Adress = db.Addresses.Take(1).ToList()[0];
                b.Adress = a.Adress;

                var watch = System.Diagnostics.Stopwatch.StartNew();
                foreach (var item in db.ChangeTracker.Entries())
                {
                    item.State = EntityState.Modified;
                }
                db.Companies.Add(a);
                watch.Stop();
                var elapsedTicks = watch.ElapsedTicks;
                Trace("Specific method time : " + elapsedTicks + " ticks");
            }

            DBTestData.DBTestData.ResetDB();
            using (MyDbContext db = new MyDbContext())
            {
                a.Adress = db.Addresses.Take(1).ToList()[0];
            }

            long oldcount = DBTestData.DBTestData.TotalCount();
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            _CompanyServiceToTest.SaveCrypted(a);
            watch2.Stop();
            var elapsedTicks2 = watch2.ElapsedTicks;
            Trace("Generic method time : " + elapsedTicks2 + " ticks");

            long newCount = DBTestData.DBTestData.TotalCount();
            if (oldcount + 1 != newCount)
                Assert.Fail();

            b.Id = a.Id;
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        [TestCategory("Company")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        public void Test_Save_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            DBTestData.DBTestData.ResetDB();

            Company a = new Company();
            a.Name = "test";

            using (MyDbContext db = new MyDbContext())
            {
                a.Adress = db.Addresses.Take(1).ToList()[0];

                var watch = System.Diagnostics.Stopwatch.StartNew();
                foreach (var item in db.ChangeTracker.Entries())
                {
                    item.State = EntityState.Modified;
                }
                db.Companies.Add(a);
                watch.Stop();
                var elapsedTicks = watch.ElapsedTicks;
                Trace("Specific method time : " + elapsedTicks + " ticks");
            }

            DBTestData.DBTestData.ResetDB();

            long oldcount = DBTestData.DBTestData.TotalCount();
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            _CompanyServiceToTest.Save(a);
            watch2.Stop();
            var elapsedTicks2 = watch2.ElapsedTicks;
            Trace("Generic method time : " + elapsedTicks2 + " ticks");

            long newCount = DBTestData.DBTestData.TotalCount();
            Assert.AreEqual(oldcount + 1, newCount);
        }
    }
}
