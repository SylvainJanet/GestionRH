using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class TrainingCourseRepository : ITrainingCourseRepository
    {
        private MyDbContext db;

        public TrainingCourseRepository(MyDbContext db)
        {
            this.db = db;
        }

        public void Add(TrainingCourse tc)
        {
            db.TrainingCourses.Add(tc);
        }

        public List<TrainingCourse> FindAll(int start, int maxByPage, string searchField)
        {
            IQueryable<TrainingCourse> req = db.TrainingCourses.AsNoTracking().OrderBy(u => u.StartingDate);
            if (searchField != null && !searchField.Trim().Equals(""))
                req = req.Where(tc => tc.TrainedSkills.Where(s => s.Description.Contains(searchField)).Count() != 0);
            req = req.Skip(start).Take(maxByPage);
            return req.ToList();
        }

        public long Count(string searchField)
        {
            IQueryable<TrainingCourse> req = db.TrainingCourses.AsNoTracking();
            if (searchField != null && !searchField.Trim().Equals(""))
                req = req.Where(tc => tc.TrainedSkills.Where(s => s.Description.Contains(searchField)).Count() != 0);
            return req.Count();
        }

        public void Save(TrainingCourse tc)
        {
            db.TrainingCourses.Add(tc);
            db.SaveChanges();
        }

        public TrainingCourse Find(int? id)
        {
            return db.TrainingCourses.Include(tc => tc.EnrolledEmployees).Include(tc => tc.TrainedSkills).AsNoTracking().SingleOrDefault(s => s.Id == id);
        }

        public void Update(TrainingCourse tc)
        {
            db.Entry(tc).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Remove(int id)
        {
            db.TrainingCourses.Remove(db.TrainingCourses.Find(id));
            db.SaveChanges();
        }

        public List<TrainingCourse> SearchByDescription(string searchField)
        {
            return (from tc in db.TrainingCourses.AsNoTracking() where tc.TrainedSkills.Where(s => s.Description.Contains(searchField)).Count() != 0 select tc).ToList();

        }
    }
}