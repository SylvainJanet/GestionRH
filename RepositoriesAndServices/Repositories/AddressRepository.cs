using GenericRepositoryAndService.Repository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RepositoriesAndServices.Repositories
{
    public class AddressRepository : GenericRepository<Address> , IAddressRepository
    {
        public AddressRepository(MyDbContext db) : base(db)
        {

        }
    }
}