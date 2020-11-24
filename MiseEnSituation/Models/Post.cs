using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public enum ContractType
    {
        CDI,
        CDD,
        CONTRAT_PRO,
        ALTERNANCE,
        APPRENTISSAGE
    }
    
    public class Post
    {
        public int? Id { get; set; }

        public DateTime HiringDate { get; set; }

        public ContractType ContractType { get; set; }

        public DateTime EndDate { get; set; }

        public double WeeklyWorkLoad { get; set; }

        public string FileForContract { get; set; }

        public Company Company { get; set; }

        public Employee Manage { get; set; }

        public List<Skill> RequiredSkills { get; set; }

        public List<Employee> Employees { get; set; }
    }
}