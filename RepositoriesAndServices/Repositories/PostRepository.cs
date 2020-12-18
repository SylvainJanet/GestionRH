using GenericRepositoryAndService.Repository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RepositoriesAndServices.Repositories
{
    public class PostRepository : GenericRepository<Post>
    {
        public PostRepository(MyDbContext DataContext) : base(DataContext)
        {

        }

    }
}