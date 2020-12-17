using System;

namespace GenericRepositoryAndService.Exceptions
{
    public class CannotWriteReadOnlyPropertyException : Exception
    {
        public CannotWriteReadOnlyPropertyException(Type type, string propertyName) : base(_MyExceptionMessages.CannotWriteReadOnlyProperty(type, propertyName))
        {

        }
    }
}