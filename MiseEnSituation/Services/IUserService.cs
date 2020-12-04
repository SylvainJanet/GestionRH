using MiseEnSituation.Models;
using System.Collections.Generic;

namespace MiseEnSituation.Services
{
    public interface IUserService
    {
        User CheckLogin(string email, string password, UserType type);
        bool NextExist(int page, int maxByPage, string searchField);
        List<User> FindAll(int page, int maxByPage, string searchField);
        void Save(User user);
        User Find(int? id);
        void Update(User user);
        void Remove(int id);
        List<User> Search(string searchField);
        User FindByType(UserType userType);
    }
}