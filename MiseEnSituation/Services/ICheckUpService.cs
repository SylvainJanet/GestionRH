using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Controllers
{
    internal interface ICheckUpService
    {
        List<CheckUp> FindAll(int page, int maxByPage);
        bool NextExist(int page, int maxByPage);
        CheckUp Find(int? id);
        void Save(CheckUp checkUp);
        void Update(CheckUp checkUp);
        void Remove(int id);
    }
}