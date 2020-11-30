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
    public class CheckUpReportRepository : ICheckUpReportRepository
    {
        MyDbContext db;
        public CheckUpReportRepository(MyDbContext db)
        {
            this.db = db;

        }
        public int Count()
        {
            IQueryable<CheckUpReport> req = db.CheckUpReports.AsNoTracking();
            return req.Count();
        }

        public CheckUpReport Find(int? id)
        {
            return db.CheckUpReports.AsNoTracking().SingleOrDefault(c => c.Id == id);
        }

        public List<CheckUpReport> FindAll(int start, int maxByPage)
        {
            IQueryable<CheckUpReport> req = db.CheckUpReports.AsNoTracking();
            return req.ToList();
        }

        public void Remove(int id)
        {
            db.CheckUpReports.Remove(Find(id));
            db.SaveChanges();
        }

        public void Save(CheckUpReport checkUpReport)
        {
            db.CheckUpReports.Add(checkUpReport);
            db.SaveChanges();
        }

        public void Update(CheckUpReport checkUpReport)
        {
            db.Entry(checkUpReport).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}