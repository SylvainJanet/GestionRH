using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Controllers
{
    internal interface ICheckUpRepository
    {
        CheckUp Find(int? id);
        List<CheckUp> FindAll(int start, int maxByPage);
        int Count();
        void Remove(int id);
        void Save(CheckUp checkUp);
        void Update(CheckUp checkUp);
    }
}