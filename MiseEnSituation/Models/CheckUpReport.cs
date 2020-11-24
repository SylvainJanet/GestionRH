using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class CheckUpReport
    {
        public int? Id { get; set; }

        [Required]
        public string Content { get; set; }

        public List<TrainingCourse> FinishedCourses { get; set; }

        public List<TrainingCourse> WishedCourses { get; set; }

        public CheckUpReport(string content)
        {
            Content = content;
            FinishedCourses = new List<TrainingCourse>();
            WishedCourses = new List<TrainingCourse>();
        }

        public CheckUpReport()
        {
            FinishedCourses = new List<TrainingCourse>();
            WishedCourses = new List<TrainingCourse>();
        }
    }
}