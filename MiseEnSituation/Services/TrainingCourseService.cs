using GenericRepositoryAndService.Service;
using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Services
{
    public class TrainingCourseService : GenericService<TrainingCourse>, ITrainingCourseService
    {
        private readonly ITrainingCourseRepository _TrainingCourseRepository;

        public TrainingCourseService(ITrainingCourseRepository trainingCourseRepository) : base(trainingCourseRepository)
        {
            _TrainingCourseRepository = (ITrainingCourseRepository)trainingCourseRepository;
        }

        public override Expression<Func<IQueryable<TrainingCourse>, IOrderedQueryable<TrainingCourse>>> OrderExpression()
        {
            return req => req.OrderBy(tc => tc.Name);
        }

        public override Expression<Func<TrainingCourse, bool>> SearchExpression(string searchField = "")
        {
            searchField = searchField.Trim().ToLower();
            return tc => tc.TrainedSkills.Where(s => s.Description.Contains(searchField))
                                         .Count() > 0;
        }
    }
}