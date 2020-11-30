using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace MiseEnSituation.Controllers
{
    internal class CheckUpRepository : ICheckUpRepository
    {
        private MyDbContext db;

        public CheckUpRepository(MyDbContext db)
        {
            this.db = db;
        }
        public void Add(CheckUp c)
        {
            db.CheckUps.Add(c);
        }

        public int Count()
        {
            IQueryable<CheckUp> req = db.CheckUps.AsNoTracking();
            return req.Count();
        }

        public CheckUp Find(int? id)
        {
            return db.CheckUps.AsNoTracking().SingleOrDefault(c => c.Id == id);
        }

        public List<CheckUp> FindAll(int start, int maxByPage)
        {
            IQueryable<CheckUp> req = db.CheckUps.AsNoTracking().OrderBy(c => c.Date);
            return req.ToList();
        }

        public void Remove(int id)
        {
            db.CheckUps.Remove(db.CheckUps.Find(id));
            db.SaveChanges();
        }

        public void Save(CheckUp checkUp)
        {
            db.CheckUps.Add(checkUp);
            db.SaveChanges();
        }

        public void Update(CheckUp checkUp)
        {
            db.Entry(checkUp).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}