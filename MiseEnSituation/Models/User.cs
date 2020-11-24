using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class User
    {
        [Key] // pas obligatoire si le nom de l'attribut est Id ou UserId
        public int? Id { get; set; }

        public string Name { get; set; }

        [Index(IsUnique=true)]
        [MaxLength(150)] // un index de type string ne peut pas avoir une taille quelconque
        public string Email { get; set; }

        public string Password { get; set; }

        //[DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        public User()
        {
            CreationDate = DateTime.Now;
        }
    }
}