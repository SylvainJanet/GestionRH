using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Services
{
    public interface ITrainingCourseService : IGenericService<TrainingCourse>
    {
        List<TrainingCourse> FindAll(int page, int maxByPage, string searchField);
        List<TrainingCourse> FindAllTracked(int page, int maxByPage, string searchField);

        bool NextExist(int page, int maxByPage, string searchField);

        List<TrainingCourse> GetBySkillDescription(string searchField);
        List<TrainingCourse> GetBySkillDescriptionTracked(string searchField);
        void Save(TrainingCourse trainingCourse, List<Skill> skills, List<Employee> employees = null, List<CheckUpReport> reportsfinished = null, List<CheckUpReport> reportswished = null);
        void Update(TrainingCourse trainingCourse, List<Skill> skills, List<Employee> employees = null, List<CheckUpReport> reportsfinished = null, List<CheckUpReport> reportswished = null);
    }
}