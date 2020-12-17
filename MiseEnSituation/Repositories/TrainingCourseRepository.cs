using GenericRepositoryAndService.Repository;
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
    }
}