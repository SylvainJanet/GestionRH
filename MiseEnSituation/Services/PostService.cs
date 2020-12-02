using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace MiseEnSituation.Services
{
    public class PostService : GenericService<Post>
    {
        private IPostRepository _postRepository;

        public PostService(IGenericRepository<Post> genericRepository) : base(genericRepository)
        {
            _postRepository = (IPostRepository)_repository;

        }

        public PostService(IPostRepository postRepository, IGenericRepository<Post> genericRepository) : base(genericRepository)
        {
            this._postRepository = postRepository;
        }

        public override Expression<Func<IQueryable<Post>, IOrderedQueryable<Post>>> OrderExpression()
        {
            return req => req.OrderBy(s => s.ContractType);
        }

        public override Expression<Func<Post, bool>> SearchExpression(string searchField = "")
        {
            searchField = searchField.Trim().ToLower();
            return s => s.Id.Equals(Convert.ToInt32(searchField));
        }


        //ajouter les implémentations spécifiques des méthodes ajoutées dans ICompanyService
        //on pourra utiliser le postRepository comme le genericRepository pour faire le job

    }
}