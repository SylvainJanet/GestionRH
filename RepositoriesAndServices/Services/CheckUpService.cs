using GenericRepositoryAndService.Repository;
using GenericRepositoryAndService.Service;
using Model.Models;
using RepositoriesAndServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RepositoriesAndServices.Services
{
    public class CheckUpService : GenericService<CheckUp> ,ICheckUpService
    {
         protected IGenericRepository<CheckUp> _checkUpRepository;

        public CheckUpService(IGenericRepository<CheckUp> checkUpRepository) : base(checkUpRepository)
        {
            this._checkUpRepository = checkUpRepository;
        }

        public override Expression<Func<IQueryable<CheckUp>, IOrderedQueryable<CheckUp>>> OrderExpression()
        {
            return req => req.OrderBy(s => s.Date);
        }

        public override Expression<Func<CheckUp, bool>> SearchExpression(string searchField = "")
        {
            throw new NotImplementedException();
        }
    }
}