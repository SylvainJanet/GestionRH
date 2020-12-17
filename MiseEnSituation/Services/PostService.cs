using GenericRepositoryAndService.Repository;
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
    public class PostService : GenericService<Post> , IPostService
    {
        private readonly IGenericRepository<Post> _genericRepository;

        public PostService(IGenericRepository<Post> genericRepository) : base(genericRepository)
        {
            this._genericRepository = genericRepository;
        }

        public PostService(IPostRepository postRepository, IGenericRepository<Post> genericRepository) : base(genericRepository)
        {
            this._genericRepository = postRepository;
        }

        public override Expression<Func<IQueryable<Post>, IOrderedQueryable<Post>>> OrderExpression()
        {
            return req => req.OrderBy(s => s.ContractType);
        }

        public override Expression<Func<Post, bool>> SearchExpression(string searchField = "")
        {
            searchField = searchField.Trim().ToLower();
            return s => s.FileForContract.Trim().ToLower().Contains(searchField); ;
        }


        //ajouter les implémentations spécifiques des méthodes ajoutées dans ICompanyService
        //on pourra utiliser le postRepository comme le genericRepository pour faire le job

    }
}