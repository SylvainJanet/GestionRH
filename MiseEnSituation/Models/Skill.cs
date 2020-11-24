using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class Skill
    {
        public int? Id { get; set; }

        public List<Employee> Employees { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        public List<TrainingCourse> Courses { get; set; }

        public Skill(string description)
        {
            Description = description;
            Employees = new List<Employee>();
            Courses = new List<TrainingCourse>();
        }

        public Skill()
        {
            Employees = new List<Employee>();
            Courses = new List<TrainingCourse>();
        }
    }
}