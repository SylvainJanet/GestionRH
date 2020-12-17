using GenericRepositoryAndService.Repository;
using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>
    {

        public EmployeeRepository(MyDbContext db) : base(db)
        {

        }

        //TODO ajouter les implémentations si ajout de méthodes spécifiques
        //dans l'interface ICompanyRepository

    }
}
