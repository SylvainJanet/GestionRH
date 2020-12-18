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
    public class AddressService: GenericService<Address>
    {
        public AddressService(AddressRepository addressRepository):base(addressRepository)
        {

        }

        public override Expression<Func<IQueryable<Address>, IOrderedQueryable<Address>>> OrderExpression()
        {
            return req => req.OrderBy(s => s.City);
        }

        public override Expression<Func<Address, bool>> SearchExpression(string searchField = "")
        {
            searchField = searchField.Trim().ToLower();
            return s => s.City.Trim().ToLower().Contains(searchField);
        }
    }
}