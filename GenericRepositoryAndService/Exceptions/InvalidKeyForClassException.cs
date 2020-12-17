using System;

namespace GenericRepositoryAndService.Exceptions
{
    public class InvalidKeyForClassException : Exception
    {
        public InvalidKeyForClassException(Type type) : base(_MyExceptionMessages.InvalidKeyForClass(type))
        {

        }
    }
}