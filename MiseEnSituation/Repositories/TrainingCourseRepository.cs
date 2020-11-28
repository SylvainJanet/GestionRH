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
    }
}