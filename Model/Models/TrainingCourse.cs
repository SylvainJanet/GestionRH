using GenericRepositoryAndService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class TrainingCourse : BaseEntity
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Starting date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Starting date")]
        public DateTime StartingDate { get; set; }

        [Required(ErrorMessage = "Ending date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Ending date")]
        public DateTime EndingDate { get; set; }

        [Required(ErrorMessage = "Duration in hours is required")]
        [Display(Name = "Duration in hours")]
        public double DurationInHours { get; set; }

        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double? Price { get; set; }

        [Display(Name = "Enrolled employees")]
        public IList<Employee> EnrolledEmployees { get; set; }

        [Required(ErrorMessage = "Trained skills is required")]
        [Display(Name = "Trained skills")]
        public IList<Skill> TrainedSkills { get; set; }

        [Display(Name = "Reports finished")]
        public IList<CheckUpReport> ReportsFinished { get; set; }

        [Display(Name = "Reports wished")]
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