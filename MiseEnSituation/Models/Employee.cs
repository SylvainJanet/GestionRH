using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class Employee : User
    {
        public DateTime BirthDate { get; set; }

        public Address PersonalAdress { get; set; }

        public string PersonalPhone { get; set; }

        public Company Company { get; set; }

        public bool IsManager { get; set; }

        public List<Skill> Skills { get; set; }

        public List<TrainingCourse> Courses { get; set; }

        public Post Post { get; set; }
    }
}