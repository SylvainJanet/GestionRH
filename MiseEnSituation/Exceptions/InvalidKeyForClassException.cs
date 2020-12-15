using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Exceptions
{
    public class InvalidKeyForClassException : Exception
    {
        public InvalidKeyForClassException(Type t) : base(_MyExceptionMessages.InvalidKeyForClass(t))
        {

        }
    }
}