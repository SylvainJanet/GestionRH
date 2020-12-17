using GenericRepositoryAndService.Repository;
using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        //TODO ajouter les méthodes spécifiques
        //on a déjà hériter des méthodes génériques
    }
}