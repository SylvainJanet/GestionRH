using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Repositories
{
    public interface IUserRepository
    {
        User FindByEmail(string email);
        List<User> FindAll(int start, int maxByPage, string searchField);
        long Count(string searchField);
        void Save(User user);
        User Find(int? id);
        void Update(User user);
        void Remove(int id);
        List<User> SearchByNameOrEmail(string searchField);
        List<User> FindByType(UserType userType);
    }
}