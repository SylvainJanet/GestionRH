using System;

namespace GenericRepositoryAndService.Exceptions
{
    public class CascadeCreationInDBException : Exception
    {
        public CascadeCreationInDBException(Type type) : base(_MyExceptionMessages.CascadeCreationInDB(type))
        {
        }
    }
}