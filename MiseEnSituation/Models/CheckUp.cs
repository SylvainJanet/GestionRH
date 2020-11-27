using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class CheckUp : BaseEntity
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public CheckUpReport Report { get; set; }

        public Employee Employee { get; set; }

        public Employee Manager { get; set; }

        public Employee RH { get; set; }

        public CheckUp()
        {
        }
    }
}