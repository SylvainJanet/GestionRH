using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Repositories
{
    public interface ITrainingCourseRepository : IGenericRepository<TrainingCourse>
    {
        List<TrainingCourse> GetBySkillDescription(string searchField);
        List<TrainingCourse> GetBySkillDescriptionTracked(string searchField);
    }
}