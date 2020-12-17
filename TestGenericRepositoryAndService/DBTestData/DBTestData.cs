using GenericRepositoryAndService.Tools;
using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MiseEnSituation.Controllers;

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
    }
}
