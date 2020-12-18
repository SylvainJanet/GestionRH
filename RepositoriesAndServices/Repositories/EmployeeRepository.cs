using GenericRepositoryAndService.Repository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RepositoriesAndServices.Repositories
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
