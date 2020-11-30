using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Controllers
{
    internal interface ICheckUpRepository
    {
        CheckUpReport Find(int? id);
        List<CheckUpReport> FindAll(int start, int maxByPage);
        int Count();
        void Remove(int id);
        void Save(CheckUpReport checkUp);
        void Update(CheckUpReport checkUp);
    }
}