using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public class Company
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public Address Adress { get; set; }
    }
}