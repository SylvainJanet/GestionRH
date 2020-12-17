using GenericRepositoryAndService.Models;
using MiseEnSituation.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class CheckUp : BaseEntity
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public CheckUpReport Report { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }

        // [EmployeeIsManager]
        [ForeignKey("ManagerId")]
        public Employee Manager { get; set; }
        public int? ManagerId { get; set; }

       // [EmployeeIsRH]
        [ForeignKey("RHId")]
        public Employee RH { get; set; }
        public int? RHId { get; set; }

        public CheckUp()
        {
        }

    }
}