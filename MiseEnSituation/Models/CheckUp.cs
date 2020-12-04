using MiseEnSituation.ValidationAttributes;
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

        [EmployeeIsManager]
        public Employee Manager { get; set; }

        [EmployeeIsRH]
        public Employee RH { get; set; }

        public CheckUp()
        {
        }
    }
}