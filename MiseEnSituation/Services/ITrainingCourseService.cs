using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Services
{
    public interface ITrainingCourseService
    {
        TrainingCourse Find(int? id);
        List<TrainingCourse> FindAll(int page, int maxByPage, string searchField);
        bool NextExist(int page, int maxByPage, string searchField);
        void Remove(int id);
        void Save(TrainingCourse tc);
        List<TrainingCourse> Search(string searchField);
        void Update(TrainingCourse tc);
    }
}