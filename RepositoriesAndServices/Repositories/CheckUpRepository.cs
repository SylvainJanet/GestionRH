using GenericRepositoryAndService.Repository;
using Model.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace RepositoriesAndServices.Repositories
{
    public class CheckUpRepository : GenericRepository<CheckUp>, ICheckUpRepository
    {

        public CheckUpRepository(MyDbContext db) : base(db)
        {
        }
        
    }
}