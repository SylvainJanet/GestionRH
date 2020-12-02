using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Services
{
    public class EmployeeService : GenericService<Employee>
    {
        private IEmployeeRepository _companyRepository;

        public EmployeeService(IGenericRepository<Employee> genericRepository) : base(genericRepository)
        {
            _companyRepository = (IEmployeeRepository)_repository;
        }

        public EmployeeService(IEmployeeRepository companyRepository, IGenericRepository<Employee> genericRepository) : base(genericRepository)
        {
            this._companyRepository = companyRepository;
        }

        public override Expression<Func<IQueryable<Employee>, IOrderedQueryable<Employee>>> OrderExpression()
        {
            return req => req.OrderBy(s => s.Name);
        }

        public override Expression<Func<Employee, bool>> SearchExpression(string searchField = "")
        {
            searchField = searchField.Trim().ToLower();
            return s => s.Name.Trim().ToLower().Contains(searchField);
        }

        //ajouter les implémentations spécifiques des méthodes ajoutées dans IEmployeeService
        //on pourra utiliser le companyRepository comme le genericRepository pour faire le job

    }
}