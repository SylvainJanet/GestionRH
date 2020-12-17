using System;

namespace GenericRepositoryAndService.Exceptions
{
    public class InvalidArgumentsForClassException : Exception
    {
        public InvalidArgumentsForClassException(Type type) : base(_MyExceptionMessages.InvalidArgumentsForClass(type))
        {
        }
    }
}