using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Services
{
    public class TrainingCourseService : ITrainingCourseService
    {
        private ITrainingCourseRepository _trainingCourseRepository;

        public TrainingCourseService(ITrainingCourseRepository trainingCourseRepository)
        {
            _trainingCourseRepository = trainingCourseRepository;
        }

        public TrainingCourse Find(int? id)
        {
            return _trainingCourseRepository.Find(id);
        }

        public List<TrainingCourse> FindAll(int page, int maxByPage, string searchField)
        {
            int start = (page - 1) * maxByPage;
            return _trainingCourseRepository.FindAll(start, maxByPage, searchField);
        }

        public bool NextExist(int page, int maxByPage, string searchField)
        {
            return (page * maxByPage) < _trainingCourseRepository.Count(searchField);
        }

        public void Remove(int id)
        {
            _trainingCourseRepository.Remove(id);
        }

        public void Save(TrainingCourse tc)
        {
            _trainingCourseRepository.Save(tc);
        }

        public List<TrainingCourse> Search(string searchField)
        {
            searchField = searchField.Trim().ToLower();
            return _trainingCourseRepository.SearchByDescription(searchField);
        }

        public void Update(TrainingCourse tc)
        {
            _trainingCourseRepository.Update(tc);
        }
    }
}