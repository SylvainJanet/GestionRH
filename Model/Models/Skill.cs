using GenericRepositoryAndService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model.Models
{
    public class Skill : BaseEntity
    {
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(200)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Training courses")]
        public IList<TrainingCourse> Courses { get; set; }

        [Display(Name = "Posts")]
        public IList<Post> Posts { get; set; }

        [Display(Name = "Employees")]
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