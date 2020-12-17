using System;

namespace GenericRepositoryAndService.Exceptions
{
    public class PropertyNameNotFoundException : Exception
    {
        public PropertyNameNotFoundException(Type type, string nameNotFound) : base(_MyExceptionMessages.PropertyNameNotFoundForClass(type, nameNotFound))
        {

        }
    }
}