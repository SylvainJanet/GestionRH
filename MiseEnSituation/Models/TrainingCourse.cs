using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class TrainingCourse
    {
        public int? Id { get; set; }

        public DateTime StartingDate { get; set; }

        public DateTime EndingDate { get; set; }

        public double DurationInHours { get; set; }

        public double Price { get; set; }

        public List<Employee> EnrolledEmployees { get; set; }

        public List<Skill> TrainedSkills { get; set; }
    }
}