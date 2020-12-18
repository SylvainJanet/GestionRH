using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Exceptions
{
    public class IncorrectUserEmailOrPasswordException : Exception
    {
        public IncorrectUserEmailOrPasswordException() : base(_MyExceptionMessages.IncorrectUserEmailOrPassword)
        {

        }
    }
}