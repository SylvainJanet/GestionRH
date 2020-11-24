using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        [DataType(DataType.Date)]
        public DateTime HiringDate { get; set; }

        [Required]
        public ContractType ContractType { get; set; }

        public DateTime EndDate { get; set; }

        [Required]
        public double WeeklyWorkLoad { get; set; }

        [Required]
        public string FileForContract { get; set; }

        [Required]
        public Company Company { get; set; }

        public Employee Manager { get; set; }

        [Required]
        public List<Skill> RequiredSkills { get; set; }

        public List<Employee> Employees { get; set; }

        public Post(DateTime hiringDate, ContractType contractType, double weeklyWorkLoad, string fileForContract, Company company, List<Skill> requiredSkills)
        {
            RequiredSkills = requiredSkills;
            HiringDate = hiringDate;
            ContractType = contractType;
            WeeklyWorkLoad = weeklyWorkLoad;
            FileForContract = fileForContract;
            Company = company;
            Employees = new List<Employee>();
        }

        public Post()
        {
            Employees = new List<Employee>();
            RequiredSkills = new List<Skill>();
        }
    }
}