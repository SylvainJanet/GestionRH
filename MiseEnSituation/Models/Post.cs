using GenericRepositoryAndService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    
    public class Post : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime HiringDate { get; set; }

        [Required]
        public ContractType ContractType { get; set; }

        public DateTime EndDate { get; set; }

        public double WeeklyWorkLoad { get; set; }

        [Required]
        public string FileForContract { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public int CompanyId { get; set; }

        public Employee Manager { get; set; }

        public List<Skill> RequiredSkills { get; set; }

        public List<Employee> Employees { get; set; }

        public Post(string description, DateTime hiringDate, ContractType contractType, double weeklyWorkLoad, string fileForContract, Company company, List<Skill> requiredSkills) : this()
        {
            Description = description;
            RequiredSkills = requiredSkills;
            HiringDate = hiringDate;
            ContractType = contractType;
            WeeklyWorkLoad = weeklyWorkLoad;
            FileForContract = fileForContract;
            Company = company;
        }

        public Post()
        {
            Employees = new List<Employee>();
            RequiredSkills = new List<Skill>();
        }
    }
}