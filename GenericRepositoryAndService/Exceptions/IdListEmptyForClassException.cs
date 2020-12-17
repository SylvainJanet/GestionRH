using System;

namespace GenericRepositoryAndService.Exceptions
{
    public class IdListEmptyForClassException : Exception
    {
        public IdListEmptyForClassException(Type type) : base(_MyExceptionMessages.IdListEmptyForClass(type))
        {

        }
    }
}