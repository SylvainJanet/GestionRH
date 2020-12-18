using Model.Models;
using System.Collections.Generic;

namespace RepositoriesAndServices.Services
{
    public interface ICheckUpService
    {
        List<CheckUp> FindAll(int page, int maxByPage);
        bool NextExist(int page, int maxByPage);
        CheckUp Find(int? id);
        void Save(CheckUp checkUp);
        void Update(CheckUp checkUp);
        void Remove(int id);
    }
}