using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class Skill
    {
        public int? Id { get; set; }

        public List<Employee> Employees { get; set; }

        public string Description { get; set; }
    }
}