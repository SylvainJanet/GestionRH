using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Services
{
    public class TrainingCourseService : GenericService<TrainingCourse>, ITrainingCourseService
    {
        private ITrainingCourseRepository _TrainingCourseRepository;

        public TrainingCourseService(ITrainingCourseRepository trainingCourseRepository) : base(trainingCourseRepository)
        {
            _TrainingCourseRepository = (ITrainingCourseRepository)_repository;
        }

        public List<TrainingCourse> FindAll(int page, int maxByPage, string searchField)
        {
            int start = (page - 1) * maxByPage;
            searchField = searchField.Trim().ToLower();
            return _TrainingCourseRepository.GetAll(start, maxByPage, null,
                                                       tc => tc.TrainedSkills.Where(s =>
                                                                                    s.Description.Contains(searchField)
                                                                                    )
                                                                            .Count() != 0);
        }

        public List<TrainingCourse> FindAllTracked(int page, int maxByPage, string searchField)
        {
            int start = (page - 1) * maxByPage;
            searchField = searchField.Trim().ToLower();
            return _TrainingCourseRepository.GetAllTracked(start, maxByPage, null,
                                                       tc => tc.TrainedSkills.Where(s =>
                                                                                    s.Description.Contains(searchField)
                                                                                    )
                                                                            .Count() != 0);
        }

        public bool NextExist(int page, int maxByPage, string searchField)
        {
            searchField = searchField.Trim().ToLower();
            return (page * maxByPage) < _TrainingCourseRepository.Count(tc => tc.TrainedSkills.Where(s =>
                                                                                                    s.Description.Contains(searchField)
                                                                                                    )
                                                                                            .Count() != 0);
        }

        public List<TrainingCourse> GetBySkillDescription(string searchField)
        {
            return _TrainingCourseRepository.GetBySkillDescription(searchField.Trim().ToLower());
        }

        public List<TrainingCourse> GetBySkillDescriptionTracked(string searchField)
        {
            return _TrainingCourseRepository.GetBySkillDescriptionTracked(searchField.Trim().ToLower());
        }

        public void Save(TrainingCourse trainingCourse, List<Skill> skills, List<Employee> employees = null, List<CheckUpReport> reportsfinished = null, List<CheckUpReport> reportswished = null)
        {
            _TrainingCourseRepository.Save(trainingCourse, skills, employees, reportsfinished, reportswished);
        }

        public void Update(TrainingCourse trainingCourse, List<Skill> skills, List<Employee> employees = null, List<CheckUpReport> reportsfinished = null, List<CheckUpReport> reportswished = null)
        {
            _TrainingCourseRepository.Update(trainingCourse, skills, employees, reportsfinished, reportswished);
        }
    }
}