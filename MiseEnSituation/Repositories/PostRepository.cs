using GenericRepositoryAndService.Repository;
using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class PostRepository : GenericRepository<Post>
    {
        public PostRepository(MyDbContext DataContext) : base(DataContext)
        {

        }

    }
}