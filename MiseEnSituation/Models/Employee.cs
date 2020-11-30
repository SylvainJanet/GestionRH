using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class Employee : User
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        public Address PersonalAdress { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PersonalPhone { get; set; }

        [Required]
        public Company Company { get; set; }

        [Required]
        public bool IsManager { get; set; }

        [Required]
        public List<Skill> Skills { get; set; }

        public List<TrainingCourse> Courses { get; set; }

        [Required]
        public Post Post { get; set; }

        public Employee(string name, string email, string password, UserType type, DateTime birthDate, string personalPhone, Company company, bool isManager, List<Skill> skills, Post post) : base(name, email, password, type)
        {
            BirthDate = birthDate;
            PersonalPhone = personalPhone;
            Company = company;
            IsManager = isManager;
            Skills = skills;
            Post = post;
            Courses = new List<TrainingCourse>();
        }

        public Employee() : base()
        {
            Skills = new List<Skill>();
            Courses = new List<TrainingCourse>();
        }
    }
}