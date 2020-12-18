using GenericRepositoryAndService.Repository;
using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class AddressRepository: GenericRepository<Address>
    {
        public AddressRepository(MyDbContext db) : base(db)
        {

        }
    }
}