using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericRepositoryAndService.Exceptions
{
    public class ClassHasToImplementDisplayStringToKeysException : Exception
    {
        public ClassHasToImplementDisplayStringToKeysException(Type type) : base(_MyExceptionMessages.ClassHasToImplementDisplayStringToKeys(type))
        {

        }
    }
}
