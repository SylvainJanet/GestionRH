using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Exceptions
{
    public class IdNullException : Exception
    {
        public IdNullException(Type t) : base(_MyExceptionMessages.IdNull(t))
        {

        }
    }
}