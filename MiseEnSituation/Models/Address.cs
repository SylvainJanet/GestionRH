using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class Address
    {
        public int Number { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public int ZipCode { get; set; }

        public string Country { get; set; }

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
    }
}