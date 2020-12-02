using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Repositories
{

    public interface ICompanyRepository : IGenericRepository<Company>
    {
        //TODO ajouter les méthodes spécifiques
        //on a déjà hériter des méthodes génériques
    }

}