using GenericRepositoryAndService.Repository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RepositoriesAndServices.Repositories
{
    public class CheckUpReportRepository : GenericRepository<CheckUpReport>, ICheckUpReportRepository
    {
        public CheckUpReportRepository(MyDbContext db) : base(db)
        {
        }
    }
}