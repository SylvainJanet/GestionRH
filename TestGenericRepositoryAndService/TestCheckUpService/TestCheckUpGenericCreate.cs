using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Models;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;
using System;
using System.Diagnostics;
using TestGenericRepositoryAndService.TestInterfaces;
using System.Data.Entity;
using System.Linq;

namespace TestGenericRepositoryAndService.TestCheckUpService.GenericCRUD
{
    [TestClass]
    public class TestCheckUpGenericCreate : BaseTest, ITestCreate
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
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        public void Test_SaveCrypted_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            DBTestData.DBTestData.ResetDB();

            CheckUp a = new CheckUp();
            CheckUp b = new CheckUp();

            using (MyDbContext db = new MyDbContext())
            {
                a.Date = DateTime.Now;
                b.Date = a.Date;
                a.Employee = db.Employees.Take(1).ToList()[0];
                b.Employee = a.Employee;
                a.Manager = db.Employees.Take(1).ToList()[0];
                b.Manager = b.Manager;
                a.RH = db.Employees.Take(1).ToList()[0];
                b.RH = a.RH;
                a.Report = db.CheckUpReports.Take(1).ToList()[0];
                b.Report = a.Report;

                var watch = System.Diagnostics.Stopwatch.StartNew();
                foreach (var item in db.ChangeTracker.Entries())
                {
                    item.State = EntityState.Modified;
                }
                db.CheckUps.Add(a);
                watch.Stop();
                var elapsedTicks = watch.ElapsedTicks;
                Trace("Specific method time : " + elapsedTicks + " ticks");
            }

            DBTestData.DBTestData.ResetDB();
            using (MyDbContext db = new MyDbContext())
            {
                a.Date = DateTime.Now;
                b.Date = a.Date;
                a.Employee = db.Employees.Take(1).ToList()[0];
                b.Employee = a.Employee;
                a.Manager = db.Employees.Take(1).ToList()[0];
                b.Manager = b.Manager;
                a.RH = db.Employees.Take(1).ToList()[0];
                b.RH = a.RH;
                a.Report = db.CheckUpReports.Take(1).ToList()[0];
                b.Report = a.Report;
            }

            long oldcount = DBTestData.DBTestData.TotalCount();
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            //_CheckUpServiceToTest.SaveCrypted(a);
            throw new NotImplementedException();
            watch2.Stop();
            var elapsedTicks2 = watch2.ElapsedTicks;
            Trace("Generic method time : " + elapsedTicks2 + " ticks");

            long newCount = DBTestData.DBTestData.TotalCount();
            if (oldcount + 1 != newCount)
                Assert.Fail();
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        [TestCategory("CheckUp")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        public void Test_Save_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            DBTestData.DBTestData.ResetDB();

            CheckUp a = new CheckUp();

            using (MyDbContext db = new MyDbContext())
            {
                a.Date = DateTime.Now;
                a.Employee = db.Employees.Take(1).ToList()[0];
                a.Manager = db.Employees.Take(1).ToList()[0];
                a.RH = db.Employees.Take(1).ToList()[0];
                a.Report = db.CheckUpReports.Take(1).ToList()[0];

                var watch = System.Diagnostics.Stopwatch.StartNew();
                db.CheckUps.Add(a);
                watch.Stop();
                var elapsedTicks = watch.ElapsedTicks;
                Trace("Specific method time : " + elapsedTicks + " ticks");
            }

            DBTestData.DBTestData.ResetDB();
            using (MyDbContext db = new MyDbContext())
            {
                a.Date = DateTime.Now;
                a.Employee = db.Employees.Take(1).ToList()[0];
                a.Manager = db.Employees.Take(1).ToList()[0];
                a.RH = db.Employees.Take(1).ToList()[0];
                a.Report = db.CheckUpReports.Take(1).ToList()[0];
            }

            long oldcount = DBTestData.DBTestData.TotalCount();
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            _CheckUpServiceToTest.Save(a);
            watch2.Stop();
            var elapsedTicks2 = watch2.ElapsedTicks;
            Trace("Generic method time : " + elapsedTicks2 + " ticks");

            long newCount = DBTestData.DBTestData.TotalCount();
            Assert.AreEqual(oldcount + 1, newCount);
        }
    }
}
