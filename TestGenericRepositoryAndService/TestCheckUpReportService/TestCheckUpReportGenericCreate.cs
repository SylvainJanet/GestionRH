using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Models;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;
using System;
using System.Diagnostics;
using TestGenericRepositoryAndService.TestInterfaces;
using System.Data.Entity;
using System.Linq;

namespace TestGenericRepositoryAndService.TestCheckUpReportService.GenericCRUD
{
    [TestClass]
    public class TestCheckUpReportGenericCreate : BaseTest, ITestCreate
    {
        CheckUpReportService _CheckUpReportServiceToTest = new CheckUpReportService(new CheckUpReportRepository(new MyDbContext()));

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
        [TestCategory("CheckUpReport")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        public void Test_SaveCrypted_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            DBTestData.DBTestData.ResetDB();

            CheckUpReport a = new CheckUpReport("Test");
            CheckUpReport b = new CheckUpReport("Test");

            using (MyDbContext db = new MyDbContext())
            {
                a.WishedCourses = db.TrainingCourses.Take(1).ToList();
                a.FinishedCourses = db.TrainingCourses.Take(1).ToList();
                b.WishedCourses = a.WishedCourses;
                b.FinishedCourses = a.FinishedCourses;

                var watch = System.Diagnostics.Stopwatch.StartNew();
                foreach (var item in db.ChangeTracker.Entries())
                {
                    item.State = EntityState.Modified;
                }
                db.CheckUpReports.Add(a);
                watch.Stop();
                var elapsedTicks = watch.ElapsedTicks;
                Trace("Specific method time : " + elapsedTicks + " ticks");
            }

            DBTestData.DBTestData.ResetDB();
            using (MyDbContext db = new MyDbContext())
            {
                a.WishedCourses = db.TrainingCourses.Take(1).ToList();
                a.FinishedCourses = db.TrainingCourses.Take(1).ToList();
                b.WishedCourses = a.WishedCourses;
                b.FinishedCourses = a.FinishedCourses;
            }
            
            long oldcount = DBTestData.DBTestData.TotalCount();
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            _CheckUpReportServiceToTest.SaveCrypted(a);
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
        [TestCategory("CheckUpReport")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        public void Test_Save_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            DBTestData.DBTestData.ResetDB();

            CheckUpReport a = new CheckUpReport("Test");

            using (MyDbContext db = new MyDbContext())
            {
                a.WishedCourses = db.TrainingCourses.Take(1).ToList();
                a.FinishedCourses = db.TrainingCourses.Take(1).ToList();

                var watch = System.Diagnostics.Stopwatch.StartNew();
                foreach (var item in db.ChangeTracker.Entries())
                {
                    item.State = EntityState.Modified;
                }
                db.CheckUpReports.Add(a);
                watch.Stop();
                var elapsedTicks = watch.ElapsedTicks;
                Trace("Specific method time : " + elapsedTicks + " ticks");
            }

            DBTestData.DBTestData.ResetDB();
            using (MyDbContext db = new MyDbContext())
            {
                a.WishedCourses = db.TrainingCourses.Take(1).ToList();
                a.FinishedCourses = db.TrainingCourses.Take(1).ToList();
            }
                
            long oldcount = DBTestData.DBTestData.TotalCount();
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            _CheckUpReportServiceToTest.Save(a);
            watch2.Stop();
            var elapsedTicks2 = watch2.ElapsedTicks;
            Trace("Generic method time : " + elapsedTicks2 + " ticks");

            long newCount = DBTestData.DBTestData.TotalCount();
            Assert.AreEqual(oldcount + 1, newCount);
        }
    }
}
