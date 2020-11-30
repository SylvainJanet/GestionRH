using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Services
{
    public class CheckUpReportService : ICheckUpReportService
    {
        private ICheckUpReportRepository _checkUpReportRepository;

        public CheckUpReportService(ICheckUpReportRepository _checkUpReportRepository)
        {
            this._checkUpReportRepository = _checkUpReportRepository;
        }

        public CheckUpReport Find(int? id)
        {
            return _checkUpReportRepository.Find(id);
        }

        public List<CheckUpReport> FindAll(int page, int maxByPage)
        {
            int start = (page - 1) * maxByPage;
            return _checkUpReportRepository.FindAll(start, maxByPage);
        }

        public bool NextExist(int page, int maxByPage)
        {
            return (page * maxByPage) < _checkUpReportRepository.Count();
        }

        public void Remove(int id)
        {
            _checkUpReportRepository.Remove(id);
        }

        public void Save(CheckUpReport checkUpReport)
        {
            _checkUpReportRepository.Save(checkUpReport);
        }

        public void Update(CheckUpReport checkUpReport)
        {
            _checkUpReportRepository.Update(checkUpReport);

        }
    }
}