﻿using GenericRepositoryAndService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class Company : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        
        public Address Adress { get; set; }

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