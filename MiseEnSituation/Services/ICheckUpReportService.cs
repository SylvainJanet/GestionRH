using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Services
{
    public interface ICheckUpReportService
    {
        List<CheckUpReport> FindAll(int page, int maxByPage);
        bool NextExist(int page, int maxByPage);
        CheckUpReport Find(int? id);
        void Save(CheckUpReport checkUpReport);
        void Update(CheckUpReport checkUpReport);
        void Remove(int id);
    }
}