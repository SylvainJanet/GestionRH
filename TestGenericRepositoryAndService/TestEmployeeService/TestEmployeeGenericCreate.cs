using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepositoriesAndServices.Repositories;
using RepositoriesAndServices.Services;
using System;
using System.Diagnostics;
using TestGenericRepositoryAndService.TestInterfaces;
using System.Data.Entity;
using System.Linq;
using Model.Models;
using GenericRepositoryAndService.Tools;

namespace TestGenericRepositoryAndService.TestEmployeeService.GenericCRUD
{
    [TestClass]
    public class TestEmployeeGenericCreate : BaseTest, ITestCreate
    {
        EmployeeService _EmployeeServiceToTest = new EmployeeService(new EmployeeRepository(new MyDbContext()));

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
        [TestCategory("Employee")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        public void Test_SaveCrypted_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            DBTestData.DBTestData.ResetDB();

            Employee a = new Employee
            {
                BirthDate = DateTime.Now,
                PersonalPhone = "123456789",
                Name = "test",
                Email = "testtest",
                Password = "testtesttest",
                CreationDate = DateTime.Now,
                ProPhone = "123456789",
                Type = UserType.EMPLOYEE,
                IsManager = false
            };
            Employee b = new Employee
            {
                BirthDate = DateTime.Now,
                PersonalPhone = "123456789",
                Name = "test",
                Email = "testtest",
                Password = HashTools.ComputeSha256Hash("testtesttest"),
                CreationDate = DateTime.Now,
                ProPhone = "123456789",
                Type = UserType.EMPLOYEE,
                IsManager = false
        };

            using (MyDbContext db = new MyDbContext())
            {
                a.PersonalAdress = db.Addresses.Take(1).ToList()[0];
                b.PersonalAdress = a.PersonalAdress;
                a.Company = db.Companies.Take(1).ToList()[0];
                b.Company = a.Company;
                a.Skills = db.Skills.ToList();
                b.Skills = a.Skills;
                a.Courses =db.TrainingCourses.ToList();
                b.Courses = a.Courses;
                a.Post = db.Posts.Take(1).ToList()[0];
                b.Post = a.Post;
                var watch = System.Diagnostics.Stopwatch.StartNew();
                foreach (var item in db.ChangeTracker.Entries())
                {
                    item.State = EntityState.Modified;
                }
                a.Password = HashTools.ComputeSha256Hash(a.Password);
                db.Employees.Add(a);
                watch.Stop();
                var elapsedTicks = watch.ElapsedTicks;
                Trace("Specific method time : " + elapsedTicks + " ticks");
            }

            DBTestData.DBTestData.ResetDB();
            using (MyDbContext db = new MyDbContext())
            {
                a.PersonalAdress = db.Addresses.Take(1).ToList()[0];
                b.PersonalAdress = a.PersonalAdress;
                a.Company = db.Companies.Take(1).ToList()[0];
                b.Company = a.Company;
                a.Skills = db.Skills.ToList();
                b.Skills = a.Skills;
                a.Courses = db.TrainingCourses.ToList();
                b.Courses = a.Courses;
                a.Post = db.Posts.Take(1).ToList()[0];
                b.Post = a.Post;
            }

            long oldcount = DBTestData.DBTestData.TotalCount();
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            _EmployeeServiceToTest.SaveCrypted(a);
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
        [TestCategory("Employee")]
        [TestProperty("CRUD", "Create")]
        [Owner("Sylvain")]
        [ExpectedException(typeof(NotImplementedException))]
        public void Test_Save_SaveSuccessfull()
        {
            Trace("...Testing... " + Class + " : " + Method);
            DBTestData.DBTestData.ResetDB();

            Employee a = new Employee();

            using (MyDbContext db = new MyDbContext())
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                db.Employees.Add(a);
                watch.Stop();
                var elapsedTicks = watch.ElapsedTicks;
                Trace("Specific method time : " + elapsedTicks + " ticks");
            }

            DBTestData.DBTestData.ResetDB();
            using (MyDbContext db = new MyDbContext())
            {
                a.PersonalAdress = db.Addresses.Take(1).ToList()[0];
                a.Company = db.Companies.Take(1).ToList()[0];
                a.Skills = db.Skills.ToList();
                a.Courses = db.TrainingCourses.ToList();
                a.Post = db.Posts.Take(1).ToList()[0];
            }

            long oldcount = DBTestData.DBTestData.TotalCount();
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            _EmployeeServiceToTest.Save(a);
            watch2.Stop();
            var elapsedTicks2 = watch2.ElapsedTicks;
            Trace("Generic method time : " + elapsedTicks2 + " ticks");

            long newCount = DBTestData.DBTestData.TotalCount();
            Assert.AreEqual(oldcount + 1, newCount);
        }
    }
}
