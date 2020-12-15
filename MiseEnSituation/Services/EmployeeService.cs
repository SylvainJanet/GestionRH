using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Services
{
    public class EmployeeService : GenericService<Employee>, IEmployeeService
    {
        protected IGenericRepository<Employee> _genericRepository;

        public EmployeeService(IGenericRepository<Employee> genericRepository) : base(genericRepository)
        {
            this._genericRepository = genericRepository;

        }

        public EmployeeService(IEmployeeRepository employeeRepository, IGenericRepository<Employee> genericRepository) : base(genericRepository)
        {
            this._genericRepository = employeeRepository;
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
        //on pourra utiliser le employeeRepository comme le genericRepository pour faire le job

    }
}