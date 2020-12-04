using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Repositories
{
    public class UserRepository : IUserRepository
    {
        private MyDbContext db;

        public UserRepository(MyDbContext db)
        {
            this.db = db;
        }

        public User FindByEmail(string email)
        {
            return db.Users.Where(user => user.Email.Equals(email)).SingleOrDefault();
            //var req = from u in db.Users
            //          where u.Email.Equals(email)
            //          select u;
            //return req.SingleOrDefault();
        }

        public void Add(User u)
        {
            db.Users.Add(u);
        }

        public List<User> FindAll(int start, int maxByPage, string searchField)
        {
            ////db.Users.Where(user => user.Id >= start && )
            //return db.Users.AsNoTracking().OrderBy(u => u.Name)
            //                                .Skip(start)
            //                                .Take(maxByPage)
            //                                .ToList();
            IQueryable<User> req = db.Users.AsNoTracking().OrderBy(u => u.Name);
            if (searchField != null && !searchField.Trim().Equals(""))
                req = req.Where(u => u.Name.ToLower().Contains(searchField) || u.Email.ToLower().Contains(searchField));
            req = req.Skip(start).Take(maxByPage);
            return req.ToList();
        }

        public long Count(string searchField)
        {
            //return (from u in db.Users.AsNoTracking() // pour éviter de mettre en cache le résultat
            // select u.Id).LongCount();
            //// équivalent en SQL natif : select count(id) from Users
            ///
            IQueryable<User> req = db.Users.AsNoTracking();
            if (searchField != null && !searchField.Trim().Equals(""))
                req = req.Where(u => u.Name.ToLower().Contains(searchField) || u.Email.ToLower().Contains(searchField));
            return req.Count();
        }

        public void Save(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
        }

        public User Find(int? id)
        {
            return db.Users.AsNoTracking().SingleOrDefault(u => u.Id == id);
        }

        public void Update(User user)
        {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Remove(int id)
        {
            db.Users.Remove(db.Users.Find(id));
            db.SaveChanges();
        }

        public List<User> SearchByNameOrEmail(string searchField)
        {
            return (from u in db.Users.AsNoTracking() where u.Name.ToLower().Contains(searchField) || u.Email.ToLower().Contains(searchField) select u).ToList();

        }

        public User FindByType(UserType userType)
        {
            return db.Users.Find(userType);
        }
    }
}