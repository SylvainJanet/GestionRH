using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Exceptions
{
    public class CannotWriteReadOnlyPropertyException : Exception
    {
        public CannotWriteReadOnlyPropertyException(Type t, string propertyName) : base(_MyExceptionMessages.CannotWriteReadOnlyProperty(t, propertyName))
        {

        }
    }
}