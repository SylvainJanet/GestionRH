using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using MiseEnSituation.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiseEnSituation.Controllers
{
    public class TestDataController : Controller
    {
        private readonly MyDbContext db = new MyDbContext();

        public ActionResult Index()
        {
            User u1 = new User() { Name = "Toto", Email = "toto@dawan.fr", Password = HashTools.ComputeSha256Hash("toto"), Type = UserType.ADMIN, ProPhone = "0669563654", };
            User u2 = new User() { Name = "Tata", Email = "tata@dawan.fr", Password = HashTools.ComputeSha256Hash("tata"), Type = UserType.MANAGER, ProPhone = "0756562354" };
            User u3 = new User() { Name = "Titi", Email = "titi@dawan.fr", Password = HashTools.ComputeSha256Hash("titi"), Type = UserType.EMPLOYEE, ProPhone = "0669262354" };
            User u4 = new User() { Name = "Tutu", Email = "tutu@dawan.fr", Password = HashTools.ComputeSha256Hash("tutu"), Type = UserType.MANAGER, ProPhone = "0653562354" };
            User u5 = new User() { Name = "Tooto", Email = "tooto@dawan.fr", Password = HashTools.ComputeSha256Hash("tooto"), Type = UserType.ADMIN, ProPhone = "0723562354" };
            User u6 = new User() { Name = "Taata", Email = "taata@dawan.fr", Password = HashTools.ComputeSha256Hash("taata"), Type = UserType.RH, ProPhone = "0669422354" };
            User u7 = new User() { Name = "Tiiti", Email = "tiiti@dawan.fr", Password = HashTools.ComputeSha256Hash("tiiti"), Type = UserType.EMPLOYEE, ProPhone = "0674532354" };
            User u8 = new User() { Name = "Tuutu", Email = "tuutu@dawan.fr", Password = HashTools.ComputeSha256Hash("tuutu"), Type = UserType.RH, ProPhone = "0712982354" };
            User u9 = new User() { Name = "Totoo", Email = "totoo@dawan.fr", Password = HashTools.ComputeSha256Hash("totoo"), Type = UserType.ADMIN, ProPhone = "0669492354" };
            User u10 = new User() { Name = "Tataa", Email = "tataa@dawan.fr", Password = HashTools.ComputeSha256Hash("tataa"), Type = UserType.EMPLOYEE, ProPhone = "0669568954" };
            User u11 = new User() { Name = "Titii", Email = "titii@dawan.fr", Password = HashTools.ComputeSha256Hash("titii"), Type = UserType.MANAGER, ProPhone = "0769272354" };
            User u12 = new User() { Name = "Tutuu", Email = "tutuu@dawan.fr", Password = HashTools.ComputeSha256Hash("tutuu"), Type = UserType.EMPLOYEE, ProPhone = "0678566954" };
            User u13 = new User() { Name = "Tootoo", Email = "tootoo@dawan.fr", Password = HashTools.ComputeSha256Hash("tootoo"), Type = UserType.ADMIN, ProPhone = "0672562454" };
            User u14 = new User() { Name = "Taataa", Email = "taataa@dawan.fr", Password = HashTools.ComputeSha256Hash("taataa"), Type = UserType.EMPLOYEE, ProPhone = "0669562328" };
            User u15 = new User() { Name = "Tiitii", Email = "tiitii@dawan.fr", Password = HashTools.ComputeSha256Hash("tiitii"), Type = UserType.RH, ProPhone = "0669562306" };
            User u16 = new User() { Name = "Tuutuu", Email = "tuutuu@dawan.fr", Password = HashTools.ComputeSha256Hash("tuutuu"), Type = UserType.EMPLOYEE, ProPhone = "0769562379" };

            foreach (User user in db.Users)
            {
                db.Users.Remove(user);
            }
            db.Users.Add(u1);
            db.Users.Add(u2);
            db.Users.Add(u3);
            db.Users.Add(u4);
            db.Users.Add(u5);
            db.Users.Add(u6);
            db.Users.Add(u7);
            db.Users.Add(u8);
            db.Users.Add(u9);
            db.Users.Add(u10);
            db.Users.Add(u11);
            db.Users.Add(u12);
            db.Users.Add(u13);
            db.Users.Add(u14);
            db.Users.Add(u15);
            db.Users.Add(u16);


            Skill s1 = new Skill("C");
            Skill s2 = new Skill("C++");
            Skill s3 = new Skill("C#");
            Skill s4 = new Skill("Java");
            Skill s5 = new Skill("Python");
            Skill s6 = new Skill(".NET");
            Skill s7 = new Skill("Javascript");
            Skill s8 = new Skill("HTML");
            Skill s9 = new Skill("CSS");
            Skill s10 = new Skill("Vue.js");
            Skill s11 = new Skill("Pascal");
            Skill s12 = new Skill("PHP");
            Skill s13 = new Skill("Ruby");
            Skill s14 = new Skill("SQL");
            Skill s15 = new Skill("TypeScript");
            Skill s16 = new Skill("Assembly");

            foreach (Skill skill in db.Skills)
            {
                db.Skills.Remove(skill);
            }
            db.Skills.Add(s1);
            db.Skills.Add(s2);
            db.Skills.Add(s3);
            db.Skills.Add(s4);
            db.Skills.Add(s5);
            db.Skills.Add(s6);
            db.Skills.Add(s7);
            db.Skills.Add(s8);
            db.Skills.Add(s9);
            db.Skills.Add(s10);
            db.Skills.Add(s11);
            db.Skills.Add(s12);
            db.Skills.Add(s13);
            db.Skills.Add(s14);
            db.Skills.Add(s15);
            db.Skills.Add(s16);


            TrainingCourse tc1 = new TrainingCourse("TC1", DateTime.Now, DateTime.Now.AddDays(7), 35, new List<Skill> { s1,s2,s3,s4,s5 });
            TrainingCourse tc2 = new TrainingCourse("TC2", DateTime.Now, DateTime.Now.AddDays(14), 70, new List<Skill> { s1,s2,s3,s4,s5 });
            TrainingCourse tc3 = new TrainingCourse("TC3", DateTime.Now, DateTime.Now.AddDays(7), 35, new List<Skill> { s1,s2,s3,s4,s5 });
            TrainingCourse tc4 = new TrainingCourse("TC4", DateTime.Now, DateTime.Now.AddDays(14), 70, new List<Skill> { s1,s2,s3,s4,s5 });
            TrainingCourse tc5 = new TrainingCourse("TC5", DateTime.Now, DateTime.Now.AddDays(7), 35, new List<Skill> { s6, s7, s8, s9, s10, s11 });
            TrainingCourse tc6 = new TrainingCourse("TC6", DateTime.Now, DateTime.Now.AddDays(14), 70, new List<Skill> { s6, s7, s8, s9, s10, s11 });
            TrainingCourse tc7 = new TrainingCourse("TC7", DateTime.Now, DateTime.Now.AddDays(7), 35, new List<Skill> { s6, s7, s8, s9, s10, s11 });
            TrainingCourse tc8 = new TrainingCourse("TC8", DateTime.Now, DateTime.Now.AddDays(28), 140, new List<Skill> { s6, s7, s8, s9, s10, s11 });
            TrainingCourse tc9 = new TrainingCourse("TC9", DateTime.Now, DateTime.Now.AddDays(28), 140, new List<Skill> { s12, s13, s14, s15, s16 });
            TrainingCourse tc10 = new TrainingCourse("TC10", DateTime.Now, DateTime.Now.AddDays(2), 12, new List<Skill> { s12, s13, s14, s15, s16 });
            TrainingCourse tc11 = new TrainingCourse("TC11", DateTime.Now, DateTime.Now.AddDays(2), 12, new List<Skill> { s12, s13, s14, s15, s16 });
            TrainingCourse tc12 = new TrainingCourse("TC12", DateTime.Now, DateTime.Now.AddDays(1), 10, new List<Skill> { s12, s13, s14, s15, s16 });
            TrainingCourse tc13 = new TrainingCourse("TC13", DateTime.Now.AddDays(7), DateTime.Now.AddDays(14), 35, new List<Skill> { s2, s4, s6 });
            TrainingCourse tc14 = new TrainingCourse("TC14", DateTime.Now.AddDays(7), DateTime.Now.AddDays(14), 35, new List<Skill> { s3, s6, s9, s12 });
            TrainingCourse tc15 = new TrainingCourse("TC15", DateTime.Now.AddDays(7), DateTime.Now.AddDays(14), 35, new List<Skill> { s4, s8, s12 });
            TrainingCourse tc16 = new TrainingCourse("TC16", DateTime.Now.AddDays(7), DateTime.Now.AddDays(21), 70, new List<Skill> { s5, s10, s15 });

            tc1.Price = 100;
            tc2.Price = 200;
            tc3.Price = 300;
            tc4.Price = 400;
            tc5.Price = 500;
            tc6.Price = 450;
            tc7.Price = 350;
            tc8.Price = 250;
            tc9.Price = 150;
            tc10.Price = 50;
            tc11.Price = 1500;
            tc12.Price = 2000;
            tc13.Price = 3500;
            tc14.Price = 3000;
            tc15.Price = 1250;
            tc16.Price = 750;

            tc1.EnrolledEmployees = new List<Employee>();
            tc2.EnrolledEmployees = new List<Employee>();
            tc3.EnrolledEmployees = new List<Employee>();
            tc4.EnrolledEmployees = new List<Employee>();
            tc5.EnrolledEmployees = new List<Employee>();
            tc6.EnrolledEmployees = new List<Employee>();
            tc7.EnrolledEmployees = new List<Employee>();
            tc8.EnrolledEmployees = new List<Employee>();
            tc9.EnrolledEmployees = new List<Employee>();
            tc10.EnrolledEmployees = new List<Employee>();
            tc11.EnrolledEmployees = new List<Employee>();
            tc12.EnrolledEmployees = new List<Employee>();
            tc13.EnrolledEmployees = new List<Employee>();
            tc14.EnrolledEmployees = new List<Employee>();
            tc15.EnrolledEmployees = new List<Employee>();
            tc16.EnrolledEmployees = new List<Employee>();

            foreach (TrainingCourse tc in db.TrainingCourses)
            {
                db.TrainingCourses.Remove(tc);
            }
            db.TrainingCourses.Add(tc1);
            db.TrainingCourses.Add(tc2);
            db.TrainingCourses.Add(tc3);
            db.TrainingCourses.Add(tc4);
            db.TrainingCourses.Add(tc5);
            db.TrainingCourses.Add(tc6);
            db.TrainingCourses.Add(tc7);
            db.TrainingCourses.Add(tc8);
            db.TrainingCourses.Add(tc9);
            db.TrainingCourses.Add(tc10);
            db.TrainingCourses.Add(tc11);
            db.TrainingCourses.Add(tc12);
            db.TrainingCourses.Add(tc13);
            db.TrainingCourses.Add(tc14);
            db.TrainingCourses.Add(tc15);
            db.TrainingCourses.Add(tc16);


            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}