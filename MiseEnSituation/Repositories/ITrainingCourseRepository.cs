using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Repositories
{
    public interface ITrainingCourseRepository : IGenericRepository<TrainingCourse>
    {
        List<TrainingCourse> GetBySkillDescription(string searchField);
        List<TrainingCourse> GetBySkillDescriptionTracked(string searchField);
        void Save(TrainingCourse trainingCourse, List<Skill> skills, List<Employee> employees = null, List<CheckUpReport> reportsfinished = null, List<CheckUpReport> reportswished = null);
        void Update(TrainingCourse trainingCourse, List<Skill> skills, List<Employee> employees = null, List<CheckUpReport> reportsfinished = null, List<CheckUpReport> reportswished = null);
    }
}