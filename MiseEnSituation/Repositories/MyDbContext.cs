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
            //Database.Log = l => Debug.Write(l);
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<CheckUp> CheckUps { get; set; }
        public virtual DbSet<CheckUpReport> CheckUpReports { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<TrainingCourse> TrainingCourses { get; set; }
    }
}