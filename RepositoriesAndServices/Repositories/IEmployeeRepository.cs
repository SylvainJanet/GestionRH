using GenericRepositoryAndService.Repository;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RepositoriesAndServices.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        //TODO ajouter les méthodes spécifiques
        //on a déjà hériter des méthodes génériques
    }
}