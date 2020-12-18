using GenericRepositoryAndService.Service;
using Model.Models;
using RepositoriesAndServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace RepositoriesAndServices.Services
{
    public class CheckUpReportService : GenericService<CheckUpReport>, ICheckUpReportService
    {

        public CheckUpReportService(ICheckUpReportRepository CheckUpReportRepository) : base(CheckUpReportRepository)
        {
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