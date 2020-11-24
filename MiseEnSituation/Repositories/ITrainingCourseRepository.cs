using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Repositories
{
    public interface ITrainingCourseRepository
    {
        void Add(TrainingCourse tc);
        long Count(string searchField);
        TrainingCourse Find(int? id);
        List<TrainingCourse> FindAll(int start, int maxByPage, string searchField);
        void Remove(int id);
        void Save(TrainingCourse tc);
        List<TrainingCourse> SearchByDescription(string searchField);
        void Update(TrainingCourse tc);
    }
}