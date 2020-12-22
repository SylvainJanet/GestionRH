using GenericRepositoryAndService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Model.Models
{
    public class Company : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        
        [ForeignKey("AdressId")]
        public Address Adress { get; set; }

        public int? AdressId { get; set; }

        public Company(string name, Address adress)
        {
            Name = name;
            Adress = adress;
        }

        public Company()
        {
        }
    }
}