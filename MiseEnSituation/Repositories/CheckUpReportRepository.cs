using MiseEnSituation.Controllers;
using MiseEnSituation.Models;
using MiseEnSituation.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class CheckUpReportRepository :GenericRepository<CheckUpReport>, ICheckUpReportRepository
    {
        public CheckUpReportRepository(MyDbContext db) : base(db)
        {
        }
    }
}