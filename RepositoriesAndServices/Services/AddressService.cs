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
    public class AddressService : GenericService<Address> , IAddressService
    {
        public AddressService(IAddressRepository addressRepository) : base(addressRepository)
        {

        }

        public override Expression<Func<IQueryable<Address>, IOrderedQueryable<Address>>> OrderExpression()
        {
            return req => req.OrderBy(a => a.City);
        }

        public override Expression<Func<Address, bool>> SearchExpression(string searchField = "")
        {
            searchField = searchField.Trim().ToLower();
            return a => a.City.Contains(searchField);
        }
    }
}