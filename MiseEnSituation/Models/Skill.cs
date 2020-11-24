using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class Skill
    {
        public int? Id { get; set; }

        public List<Employee> Employees { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        public Skill(string description)
        {
            Description = description;
        }

        public Skill()
        {
        }
    }
}