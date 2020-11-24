using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private MyDbContext db;

        public SkillRepository(MyDbContext db)
        {
            this.db = db;
        }

        public void Add(Skill s)
        {
            db.Skills.Add(s);
        }

        public List<Skill> FindAll(int start, int maxByPage, string searchField)
        {
            IQueryable<Skill> req = db.Skills.AsNoTracking().OrderBy(u => u.Description);
            if (searchField != null && !searchField.Trim().Equals(""))
                req = req.Where(s => s.Description.ToLower().Contains(searchField));
            req = req.Skip(start).Take(maxByPage);
            return req.ToList();
        }

        public long Count(string searchField)
        {
            IQueryable<Skill> req = db.Skills.AsNoTracking();
            if (searchField != null && !searchField.Trim().Equals(""))
                req = req.Where(s => s.Description.ToLower().Contains(searchField));
            return req.Count();
        }

        public void Save(Skill skill)
        {
            db.Skills.Add(skill);
            db.SaveChanges();
        }

        public Skill Find(int? id)
        {
            return db.Skills.Include(s=>s.Employees).AsNoTracking().SingleOrDefault(s => s.Id == id);
        }

        public void Update(Skill skill)
        {
            db.Entry(skill).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Remove(int id)
        {
            db.Skills.Remove(db.Skills.Find(id));
            db.SaveChanges();
        }

        public List<Skill> SearchByDescription(string searchField)
        {
            return (from s in db.Skills.AsNoTracking() where s.Description.ToLower().Contains(searchField) select s).ToList();

        }

        public List<Skill> FindMany(int[] ids)
        {
            List<Skill> lst = new List<Skill>();
            foreach (var skillId in ids)
            {
                lst.Add(Find(skillId));
            }
            return lst;
        }
    }
}