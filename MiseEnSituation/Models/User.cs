using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public enum UserType
    {
        ADMIN,
        EMPLOYEE,
        MANAGER,
        RH
    }

    public class User
    {
        [Key] // pas obligatoire si le nom de l'attribut est Id ou UserId
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Index(IsUnique=true)]
        [MaxLength(150)] // un index de type string ne peut pas avoir une taille quelconque
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string ProPhone { get; set; }

        [Required]
        public UserType Type { get; set; }

        public User(string name, string email, string password, UserType type)
        {
            Name = name;
            Email = email;
            Password = password;
            Type = type;
            CreationDate = DateTime.Now;
        }
    }
}