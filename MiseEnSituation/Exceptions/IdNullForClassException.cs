using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Exceptions
{
    public class IdNullForClassException : Exception
    {
        public IdNullForClassException(Type t) : base(_MyExceptionMessages.IdNullForClass(t))
        {

        }
    }
}