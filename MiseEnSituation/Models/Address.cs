using GenericRepositoryAndService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    [ComplexType]
    public class Address : EntityWithKeys
    {
        [Required]
        public int Number { get; set; }

        [Required]
        [MaxLength(200)]
        public string Street { get; set; }

        [Required]
        [MaxLength(200)]
        public string City { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public int ZipCode { get; set; }

        [Required]
        [MaxLength(200)]
        public string Country { get; set; }

        public Address(int number, string street, string city, int zipCode, string country)
        {
            Number = number;
            Street = street;
            City = city;
            ZipCode = zipCode;
            Country = country;
        }

        public Address()
        {
        }

        public override bool Equals(object obj)
        {
            return obj is Address address &&
                   Number == address.Number &&
                   Street == address.Street &&
                   City == address.City &&
                   ZipCode == address.ZipCode &&
                   Country == address.Country;
        }

        public override int GetHashCode()
        {
            int hashCode = 1806072010;
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Street);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(City);
            hashCode = hashCode * -1521134295 + ZipCode.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Country);
            return hashCode;
        }

        public override string KeysToDisplayString()
        {
            string res = "";
            res += Number.ToString();
            res += "\t" + Street;
            res += "\t" + City;
            res += "\t" + ZipCode.ToString();
            res += "\t" + Country;
            return res;
        }

        public static new object[] DisplayStringToKeys(string s)
        {
            string[] resstring = s.Split('\t');
            int number = Convert.ToInt32(resstring[0]);
            string street = resstring[1];
            string city = resstring[2];
            int zipcode = Convert.ToInt32(resstring[3]);
            string country = resstring[4];
            object[] res = new object[] { number, street, city, zipcode, country };
            return res;
        }
    }
}