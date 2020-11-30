using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Exceptions
{
    public class CascadeCreationInDBException : Exception
    {
        public CascadeCreationInDBException(Type t) : base(_MyExceptionMessages.CascadeCreationInDB(t))
        {
        }
    }
}