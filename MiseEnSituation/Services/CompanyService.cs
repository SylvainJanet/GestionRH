using GenericRepositoryAndService.Repository;
using GenericRepositoryAndService.Service;
using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Services
{
    public class CompanyService : GenericService<Company>, ICompanyService
    {
        protected IGenericRepository<Company> _genericRepository;

        public CompanyService(IGenericRepository<Company> genericRepository) : base(genericRepository)
        {
            this._genericRepository = genericRepository;

        }

        public CompanyService(ICompanyRepository companyRepository, IGenericRepository<Company> genericRepository) : base(genericRepository)
        {
            this._genericRepository = companyRepository;
        }

        public override Expression<Func<IQueryable<Company>, IOrderedQueryable<Company>>> OrderExpression()
        {
            return req => req.OrderBy(s => s.Name);
        }

        public override Expression<Func<Company, bool>> SearchExpression(string searchField = "")
        {
            searchField = searchField.Trim().ToLower();
            return s => s.Name.Trim().ToLower().Contains(searchField);
        }
        //ajouter les implémentations spécifiques des méthodes ajoutées dans ICompanyService
        //on pourra utiliser le companyRepository comme le genericRepository pour faire le job

    }
}