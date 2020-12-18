using GenericRepositoryAndService.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MiseEnSituation.Controllers;
using RepositoriesAndServices.Repositories;

namespace TestGenericRepositoryAndService.DBTestData
{
    public class DBTestData
    {
        public static void EmptyDb()
        {
            TestDataController.EmptyDB();
        }

        public static void ResetDB()
        {
            TestDataController.ResetDB();
        }

        public static long TotalCount()
        {
            long res = 0;
            using (MyDbContext db = new MyDbContext())
            {
                res += db.Addresses.Count(); 
                res += db.CheckUpReports.Count();
                res += db.CheckUps.Count();
                res += db.Companies.Count();
                res += db.Employees.Count();
                res += db.Posts.Count();
                res += db.Skills.Count();
                res += db.TrainingCourses.Count();
                res += db.Users.Count();
            }
            return res;
        }
    }
}
