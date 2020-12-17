using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using MiseEnSituation.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace MiseEnSituation.Controllers
{
    public class TestDataController : Controller
    {
        public static void EmptyDB()
        {
            using (MyDbContext db = new MyDbContext())
            {
                foreach (var obj in db.Addresses)
                {
                    db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var obj in db.CheckUps)
                {
                    db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var obj in db.CheckUpReports)
                {
                    db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var obj in db.Companies)
                {
                    db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var obj in db.Employees.Include(e => e.Skills))
                {
                    db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var obj in db.Posts.Include(p => p.RequiredSkills))
                {
                    db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var obj in db.Skills.Include(s => s.Posts).Include(s => s.Employees))
                {
                    db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var obj in db.TrainingCourses)
                {
                    db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                }
                foreach (var obj in db.Users)
                {
                    db.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                }
                db.SaveChanges();
            }
        }

        public static void ResetDB()
        {
            EmptyDB();
            using (MyDbContext db = new MyDbContext())
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


                TrainingCourse tc1 = new TrainingCourse("TC1", DateTime.Now, DateTime.Now.AddDays(7), 35, new List<Skill> { s1, s2, s3, s4, s5 });
                TrainingCourse tc2 = new TrainingCourse("TC2", DateTime.Now, DateTime.Now.AddDays(14), 70, new List<Skill> { s1, s2, s3, s4, s5 });
                TrainingCourse tc3 = new TrainingCourse("TC3", DateTime.Now, DateTime.Now.AddDays(7), 35, new List<Skill> { s1, s2, s3, s4, s5 });
                TrainingCourse tc4 = new TrainingCourse("TC4", DateTime.Now, DateTime.Now.AddDays(14), 70, new List<Skill> { s1, s2, s3, s4, s5 });
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

                //Address

                Address a1 = new Address(1, "rue 1", "city 1", 1111, "Pays1");
                Address a2 = new Address(2, "rue 2", "city 2", 2222, "Pays2");
                Address a3 = new Address(3, "rue 3", "city 3", 3333, "Pays3");
                Address a4 = new Address(4, "rue 4", "city 4", 4444, "Pays4");

                db.Addresses.Add(a1);
                db.Addresses.Add(a2);
                db.Addresses.Add(a3);
                db.Addresses.Add(a4);

                //Company

                Company c1 = new Company("Company 1", a1);
                Company c2 = new Company("Company 2", a2);
                Company c3 = new Company("Company 3", a3);
                Company c4 = new Company("Company 4", a4);


                db.Companies.Add(c1);
                db.Companies.Add(c2);
                db.Companies.Add(c3);
                db.Companies.Add(c4);

                //Post

                Post p1 = new Post("Post1", DateTime.Now, DateTime.Now.AddDays(4), ContractType.ALTERNANCE, 35, "file1.pdf", c1, new List<Skill> { s1, s2, s3 });
                Post p2 = new Post("Post2", DateTime.Now, DateTime.Now.AddDays(6), ContractType.APPRENTISSAGE, 38, "file2.pdf", c2, new List<Skill> { s2, s3, s4 });
                Post p3 = new Post("Post3", DateTime.Now, DateTime.Now.AddDays(8), ContractType.CDD, 34, "file3.pdf", c3, new List<Skill> { s3, s4, s5 });
                Post p4 = new Post("Post4", DateTime.Now, DateTime.Now.AddDays(9), ContractType.CDI, 39, "file4.pdf", c4, new List<Skill> { s4, s5, s6 });
                Post p5 = new Post("Post5", DateTime.Now, DateTime.Now.AddDays(2), ContractType.CONTRAT_PRO, 39, "file5.pdf", c1, new List<Skill> { s5, s6, s7 });
                Post p6 = new Post("Post6", DateTime.Now, DateTime.Now.AddDays(4), ContractType.ALTERNANCE, 35, "file6.pdf", c1, new List<Skill> { s8, s9, s10 });
                Post p7 = new Post("Post7", DateTime.Now, DateTime.Now.AddDays(6), ContractType.APPRENTISSAGE, 38, "file7.pdf", c2, new List<Skill> { s8, s9, s10 });
                Post p8 = new Post("Post8", DateTime.Now, DateTime.Now.AddDays(8), ContractType.CDD, 34, "file8.pdf", c3, new List<Skill> { s9, s10, s11 });
                Post p9 = new Post("Post9", DateTime.Now, DateTime.Now.AddDays(9), ContractType.CDI, 39, "file9.pdf", c4, new List<Skill> { s10, s11, s12 });


                db.Posts.Add(p1);
                db.Posts.Add(p2);
                db.Posts.Add(p3);
                db.Posts.Add(p4);
                db.Posts.Add(p5);
                db.Posts.Add(p6);
                db.Posts.Add(p7);
                db.Posts.Add(p8);
                db.Posts.Add(p9);

                //CheckUpReport

                CheckUpReport cur1 = new CheckUpReport("Report1");
                CheckUpReport cur2 = new CheckUpReport("Report2");
                CheckUpReport cur3 = new CheckUpReport("Report3");
                CheckUpReport cur4 = new CheckUpReport("Report4");
                CheckUpReport cur5 = new CheckUpReport("Report5");


                db.CheckUpReports.Add(cur1);
                db.CheckUpReports.Add(cur2);
                db.CheckUpReports.Add(cur3);
                db.CheckUpReports.Add(cur4);
                db.CheckUpReports.Add(cur5);

                //CheckUp

                Employee emp1 = new Employee("Employe1", "employe1@gmail.com", HashTools.ComputeSha256Hash("employee1"), UserType.EMPLOYEE, DateTime.Now, "123456789", c1, false, new List<Skill> { s1, s2 }, p1);
                Employee emp2 = new Employee("Employe2", "employe2@gmail.com", HashTools.ComputeSha256Hash("employee2"), UserType.EMPLOYEE, DateTime.Now, "123456789", c2, false, new List<Skill> { s3, s4 }, p2);

                Employee man1 = new Employee("Manager1", "manager1@gmail.com", HashTools.ComputeSha256Hash("manager1"), UserType.MANAGER, DateTime.Now, "456789123", c1, true, new List<Skill> { s1, s2, s3, s4 }, p1);
                Employee man2 = new Employee("Manager2", "manager2@gmail.com", HashTools.ComputeSha256Hash("manager2"), UserType.MANAGER, DateTime.Now, "456789123", c2, true, new List<Skill> { s5, s6, s7, s8 }, p2);

                Employee rh1 = new Employee("RH1", "rh1@gmail.com", HashTools.ComputeSha256Hash("rh1"), UserType.RH, DateTime.Now, "789123456", c1, false, new List<Skill> { s1 }, p1);
                Employee rh2 = new Employee("Rh2", "rh2@gmail.com", HashTools.ComputeSha256Hash("rh2"), UserType.RH, DateTime.Now, "789123456", c2, false, new List<Skill> { s2 }, p2);


                db.Employees.Add(emp1);
                db.Employees.Add(emp2);

                db.Employees.Add(man1);
                db.Employees.Add(man2);

                db.Employees.Add(rh1);
                db.Employees.Add(rh2);

                CheckUp cu1 = new CheckUp(DateTime.Now.AddDays(2), cur1, emp1, emp1.Id, man1, man1.Id, rh1, rh1.Id);
                CheckUp cu2 = new CheckUp(DateTime.Now.AddDays(5), cur2, emp2, emp2.Id, man2, man2.Id, rh2, rh2.Id);


                db.CheckUps.Add(cu1);
                db.CheckUps.Add(cu2);

                db.SaveChanges();
            }
        }

        public ActionResult Index()
        {

            ResetDB();

            return RedirectToAction("Index", "Home");
        }
    }
}