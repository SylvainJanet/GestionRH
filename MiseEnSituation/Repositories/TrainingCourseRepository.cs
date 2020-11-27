using MiseEnSituation.Exceptions;
using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class TrainingCourseRepository : GenericRepository<TrainingCourse>, ITrainingCourseRepository
    {
        public TrainingCourseRepository(MyDbContext db) : base(db)
        {
        }

        public override IQueryable<TrainingCourse> Collection()
        {
            return dbSet.AsNoTracking()
                        .Include(tc => tc.EnrolledEmployees)
                        .Include(tc => tc.TrainedSkills)
                        .Include(tc => tc.ReportsFinished)
                        .Include(tc => tc.ReportsWished)
                        .AsQueryable();
        }

        public override IQueryable<TrainingCourse> CollectionTracked()
        {
            return dbSet.Include(tc => tc.EnrolledEmployees)
                        .Include(tc => tc.TrainedSkills)
                        .Include(tc => tc.ReportsFinished)
                        .Include(tc => tc.ReportsWished)
                        .AsQueryable();
        }

        public override TrainingCourse FindById(int id)
        {
            return Collection().SingleOrDefault(tc => tc.Id == id);
        }

        public override TrainingCourse FindByIdTracked(int id)
        {
            return CollectionTracked().SingleOrDefault(tc => tc.Id == id);
        }

        public override List<TrainingCourse> GetAll(int start = 0, int maxByPage = int.MaxValue, Expression<Func<TrainingCourse, int?>> keyOrderBy = null, Expression<Func<TrainingCourse, bool>> predicateWhere = null)
        {
            IQueryable<TrainingCourse> req;
            if (keyOrderBy != null)
                req = Collection().OrderBy(keyOrderBy);
            else
                req = Collection().OrderBy(t => t.Id);

            req = WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        public override List<TrainingCourse> GetAllTracked(int start = 0, int maxByPage = int.MaxValue, Expression<Func<TrainingCourse, int?>> keyOrderBy = null, Expression<Func<TrainingCourse, bool>> predicateWhere = null)
        {
            IQueryable<TrainingCourse> req;
            if (keyOrderBy != null)
                req = CollectionTracked().OrderBy(keyOrderBy);
            else
                req = CollectionTracked().OrderBy(t => t.Id);

            req = WhereSkipTake(req, start, maxByPage, predicateWhere);

            return req.ToList();
        }

        public List<TrainingCourse> GetBySkillDescription(string searchField)
        {
            return (from tc in Collection()
                    where tc.TrainedSkills.Where(s =>
                                                 s.Description.Contains(searchField)
                                                 ).Count() != 0
                    select tc
                    ).ToList();
        }

        public List<TrainingCourse> GetBySkillDescriptionTracked(string searchField)
        {
            return (from tc in CollectionTracked()
                    where tc.TrainedSkills.Where(s => 
                                                 s.Description.Contains(searchField)
                                                 ).Count() != 0 
                    select tc
                    ).ToList();
        }

        protected override void DetachProperties(TrainingCourse t)
        {
            foreach (Skill skill in t.TrainedSkills)
            {
                DataContext.Entry(skill).State = EntityState.Unchanged;
            }
            foreach (Employee employee in t.EnrolledEmployees)
            {
                DataContext.Entry(employee).State = EntityState.Unchanged;
            }
            foreach (CheckUpReport checkUpReport in t.ReportsFinished)
            {
                DataContext.Entry(checkUpReport).State = EntityState.Unchanged;
            }
            foreach (CheckUpReport checkUpReport in t.ReportsWished)
            {
                DataContext.Entry(checkUpReport).State = EntityState.Unchanged;
            }
        }

        public override void Add(TrainingCourse t)
        {
            throw new CascadeCreationInDBException(typeof(TrainingCourse));
        }

        public void Save(TrainingCourse trainingCourse, List<Skill> skills, List<Employee> employees = null, List<CheckUpReport> reportsfinished = null, List<CheckUpReport> reportswished = null)
        {
            employees = employees ?? new List<Employee>();
            reportsfinished = reportsfinished ?? new List<CheckUpReport>();
            reportswished = reportswished ?? new List<CheckUpReport>();

            using (MyDbContext newContext = new MyDbContext())
            {
                List<Skill> newskills = GetFromNewContext(skills, newContext);
                List<Employee> newemployees = GetFromNewContext(employees, newContext);
                List<CheckUpReport> newreportsfinished = GetFromNewContext(reportsfinished, newContext);
                List<CheckUpReport> newreportswished = GetFromNewContext(reportswished, newContext);

                trainingCourse.TrainedSkills = newskills;
                trainingCourse.EnrolledEmployees = newemployees;
                trainingCourse.ReportsFinished = newreportsfinished;
                trainingCourse.ReportsWished = newreportswished;

                newContext.TrainingCourses.Add(trainingCourse);
                DetachProperties(trainingCourse);
                newContext.SaveChanges();
            }
        }

        public override void Modify(TrainingCourse t)
        {
            throw new CascadeCreationInDBException(typeof(TrainingCourse));
        }

        public void Update(TrainingCourse trainingCourse, List<Skill> skills, List<Employee> employees = null, List<CheckUpReport> reportsfinished = null, List<CheckUpReport> reportswished = null)
        {
            employees = employees ?? new List<Employee>();
            reportsfinished = reportsfinished ?? new List<CheckUpReport>();
            reportswished = reportswished ?? new List<CheckUpReport>();

            using (MyDbContext newContext = new MyDbContext())
            {
                TrainingCourse tcToChange = newContext.TrainingCourses.Include(tc => tc.TrainedSkills)
                                                                      .Include(tc => tc.EnrolledEmployees)
                                                                      .Include(tc => tc.ReportsFinished)
                                                                      .Include(tc => tc.ReportsWished)
                                                                      .SingleOrDefault(tc => tc.Id==trainingCourse.Id);

                tcToChange.Name = trainingCourse.Name;
                tcToChange.Price = trainingCourse.Price;
                tcToChange.StartingDate = trainingCourse.StartingDate;
                tcToChange.EndingDate = trainingCourse.EndingDate;
                tcToChange.DurationInHours = trainingCourse.DurationInHours;

                List<Skill> newskills = GetFromNewContext(skills, newContext);
                List<Employee> newemployees = GetFromNewContext(employees, newContext);
                List<CheckUpReport> newreportsfinished = GetFromNewContext(reportsfinished, newContext);
                List<CheckUpReport> newreportswished = GetFromNewContext(reportswished, newContext);

                tcToChange.TrainedSkills = newskills;
                tcToChange.EnrolledEmployees = newemployees;
                tcToChange.ReportsFinished = newreportsfinished;
                tcToChange.ReportsWished = newreportswished;

                newContext.Entry(tcToChange).State = EntityState.Modified;

                newContext.SaveChanges();
            }
        }
    }
}