using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Services
{
    public class CheckUpReportService : GenericService<CheckUpReport>, ICheckUpReportService
    {
        private ICheckUpReportRepository _CheckUpReportRepository;

        public CheckUpReportService(ICheckUpReportRepository CheckUpReportRepository) : base(CheckUpReportRepository)
        {
            _CheckUpReportRepository = (ICheckUpReportRepository)_repository;
        }
        public override Expression<Func<IQueryable<CheckUpReport>, IOrderedQueryable<CheckUpReport>>> OrderExpression()
        {
            return req => req.OrderBy(s => s.Id);
        }

        public override Expression<Func<CheckUpReport, bool>> SearchExpression(string searchField = "")
        {
            searchField = searchField.Trim().ToLower();
            return s => s.Content.Trim().ToLower().Contains(searchField);
        }
    }
}