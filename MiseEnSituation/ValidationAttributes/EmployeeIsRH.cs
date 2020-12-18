using Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiseEnSituation.ValidationAttributes
{
    public class EmployeeIsRH : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return ((Employee)value).Type.Equals(UserType.RH);
        }
    }
}