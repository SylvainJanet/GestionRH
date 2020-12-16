using MiseEnSituation.Exceptions;
using MiseEnSituation.Models;
using MiseEnSituation.Repositories;
using MiseEnSituation.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public User CheckLogin(string email, string password, UserType type)
        {
            // hasher le mot de passe
            string cryptedPwd = HashTools.ComputeSha256Hash(password);
            
            // récupérer l'utilisateur qui a cet email
            User u = _userRepository.FindByEmail(email);

            // tester si il est correct ou pas
            if (u == null || !u.Password.Equals(cryptedPwd)) 
                throw new IncorrectUserEmailOrPasswordException();
            //if (u == null || !u.Password.Equals(cryptedPwd) || !u.Type.Equals(UserType.ADMIN)) //Si UserType = Employee 
            //    return u;
            //if (u == null || !u.Password.Equals(cryptedPwd) || !u.Type.Equals(UserType.EMPLOYEE)) //Si UserType = Employee 
            //    return u;
            //if (u == null || !u.Password.Equals(cryptedPwd) || !u.Type.Equals(UserType.MANAGER)) //Si UserType = Manager 
            //    return u;
            //if (u == null || !u.Password.Equals(cryptedPwd) || !u.Type.Equals(UserType.RH)) //Si UserType = Rh 
            //    return u;
            return u;
        }

        public User FindByEmail(string email)
        {
            return _userRepository.FindByEmail(email);
        }

        public User Find(int? id)
        {
            return _userRepository.Find(id);
        }

        public List<User> FindAll(int page, int maxByPage, string searchField)
        {
            //return from u in use
            int start = (page - 1) * maxByPage;
            return _userRepository.FindAll(start, maxByPage, searchField);
        }

        public List<User> FindByType(UserType userType)
        {
            return _userRepository.FindByType(userType);
        }

        public bool NextExist(int page, int maxByPage, string searchField)
        {
            return (page * maxByPage) < _userRepository.Count(searchField);
        }

        public void Remove(int id)
        {
            _userRepository.Remove(id);
        }

        public void Save(User user)
        {
            user.Password = HashTools.ComputeSha256Hash(user.Password);
            _userRepository.Save(user);
        }

        public List<User> Search(string searchField)
        {
            searchField = searchField.Trim().ToLower();
            return _userRepository.SearchByNameOrEmail(searchField);
        }

        public void Update(User user)
        {
            // 2 cas : la personne a conservé le même mot de passe
            // soit elle a modifié => on doit crypter
            User x = _userRepository.Find(user.Id);
            if (!x.Password.Equals(user.Password))
            {
                user.Password = HashTools.ComputeSha256Hash(user.Password);
            }
            _userRepository.Update(user);
        }
    }
}