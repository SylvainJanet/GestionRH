using GenericRepositoryAndService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Model.Models
{
    public class CheckUp : BaseEntity
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [ForeignKey("ReportId")]
        public CheckUpReport Report { get; set; }
        public int? ReportId { get; set; }

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

        public CheckUp(DateTime date, CheckUpReport report, Employee employee, int? employeeId, Employee manager, int? managerId, Employee rH, int? rHId)
        {
            Date = date;
            Report = report;
            Employee = employee;
            EmployeeId = employeeId;
            Manager = manager;
            ManagerId = managerId;
            RH = rH;
            RHId = rHId;
        }
    }
}