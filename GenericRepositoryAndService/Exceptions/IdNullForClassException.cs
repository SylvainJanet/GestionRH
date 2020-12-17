using System;

namespace GenericRepositoryAndService.Exceptions
{
    public class IdNullForClassException : Exception
    {
        public IdNullForClassException(Type type) : base(_MyExceptionMessages.IdNullForClass(type))
        {

        }
    }
}