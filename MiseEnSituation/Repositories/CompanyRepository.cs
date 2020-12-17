using GenericRepositoryAndService.Repository;
using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class CompanyRepository : GenericRepository<Company>
    {

        public CompanyRepository(MyDbContext DataContext) : base(DataContext)
        {

        }

        //TODO ajouter les implémentations si ajout de méthodes spécifiques
        //dans l'interface ICompanyRepository

    }
}