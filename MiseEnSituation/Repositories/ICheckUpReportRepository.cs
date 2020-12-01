using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Repositories
{
    public interface ICheckUpReportRepository
    {
        CheckUpReport Find(int? id);
        List<CheckUpReport> FindAll(int start, int maxByPage);
        int Count();
        void Remove(int id);
        void Save(CheckUpReport checkUpReport);
        void Update(CheckUpReport checkUpReport);
    }
}