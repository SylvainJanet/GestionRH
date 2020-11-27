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

        public List<TrainingCourse> Courses { get; set; }

        public List<Post> Posts { get; set; }

        public List<Employee> Employees { get; set; }

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