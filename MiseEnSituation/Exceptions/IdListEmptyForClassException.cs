using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Exceptions
{
    public class IdListEmptyForClassException : Exception
    {
        public IdListEmptyForClassException(Type t) : base(_MyExceptionMessages.IdListEmptyForClass(t))
        {

        }
    }
}