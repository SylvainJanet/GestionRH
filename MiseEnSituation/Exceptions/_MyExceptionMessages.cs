using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Exceptions
{
    public abstract class _MyExceptionMessages
    {
        public static string IdNull(Type t)
        {
            return "Object of class " + t.Name + " has null Id";
        }
    }
}