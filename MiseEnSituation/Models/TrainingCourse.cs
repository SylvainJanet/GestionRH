using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class TrainingCourse : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartingDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndingDate { get; set; }

        [Required]
        public double DurationInHours { get; set; }

        public double? Price { get; set; }

        public IList<Employee> EnrolledEmployees { get; set; }

        [Required]
        public IList<Skill> TrainedSkills { get; set; }

        public IList<CheckUpReport> ReportsFinished { get; set; }

        public IList<CheckUpReport> ReportsWished { get; set; }

        public TrainingCourse(string name, DateTime startingDate, DateTime endingDate, double durationInHours, List<Skill> trainedSkills) : this()
        {
            Name = name;
            StartingDate = startingDate;
            EndingDate = endingDate;
            DurationInHours = durationInHours;
            TrainedSkills = trainedSkills;
        }

        public TrainingCourse()
        {
            EnrolledEmployees = new List<Employee>();
            TrainedSkills = new List<Skill>();
            ReportsFinished = new List<CheckUpReport>();
            ReportsWished = new List<CheckUpReport>();
        }
    }
}