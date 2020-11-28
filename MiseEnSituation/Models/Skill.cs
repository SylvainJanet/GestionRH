using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class Skill : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        public IList<TrainingCourse> Courses { get; set; }

        public IList<Post> Posts { get; set; }

        public IList<Employee> Employees { get; set; }

        public Skill(string description) : this()
        {
            Description = description;
        }

        public Skill()
        {
            Employees = new List<Employee>();
            Courses = new List<TrainingCourse>();
            Posts = new List<Post>();
        }
    }
}