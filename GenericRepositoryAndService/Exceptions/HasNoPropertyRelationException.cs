using System;

namespace GenericRepositoryAndService.Exceptions
{
    public class HasNoPropertyRelationException : Exception
    {
        public HasNoPropertyRelationException(Type type1, Type type2) : base(_MyExceptionMessages.HasNoPropertyRelation(type1, type2))
        {

        }
    }
}