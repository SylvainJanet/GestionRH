using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class CheckUp
    {
        public int? Id { get; set; }

        public CheckUpReport Report { get; set; }

        public List<Employee> Employees { get; set; }

        public Employee Manager { get; set; }

        public Employee RH { get; set; }
    }
}