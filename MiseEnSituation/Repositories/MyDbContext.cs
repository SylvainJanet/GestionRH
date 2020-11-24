using MiseEnSituation.Models;
using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace MiseEnSituation.Repositories
{
    public class MyDbContext : DbContext
    {
        public MyDbContext()
            : base("name=MyDbContext")
        {
            Database.Log = l => Debug.Write(l);
        }
        public virtual DbSet<User> Users { get; set; }
    }
}