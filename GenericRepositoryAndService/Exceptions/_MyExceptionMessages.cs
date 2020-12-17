using System;

namespace GenericRepositoryAndService.Exceptions
{
    public abstract class _MyExceptionMessages
    {
        public static string IdNullForClass(Type t)
        {
            return "Object of class " + t.Name + " has null Id";
        }

        public static string HasNoPropertyRelation(Type t1, Type t2)
        {
            return "Class " + t1.Name + " has no property related with class " + t2.Name;
        }

        public static string CannotWriteReadOnlyProperty(Type t, string propertyName)
        {
            return "Cannot write property " + propertyName + "of class " + t.Name + " : it is ReadOnly";
        }

        public static string CascadeCreationInDB(Type t)
        {
            return "The class " + t.Name + "cannot be added or changed in DB that way : cascade creation will occur";
        }

        public static string InvalidArgumentsForClass(Type t)
        {
            return "Invalid arguments for class " + t.Name;
        }

        public static string IdListEmptyForClass(Type t)
        {
            return "List of Ids is empty for class " + t.Name;
        }

        public static string PropertyNameNotFoundForClass(Type t, string nameNotFound)
        {
            return "Property with name " + nameNotFound + " not found for class " + t.Name;
        }

        public static string InvalidKeyForClass(Type t)
        {
            return "Invalid key for class " + t.Name;
        }

        public static string ClassHasToImplementDisplayStringToKeys(Type t)
        {
            return "A class deriving from " + t.Name + " has to implement a new definition for the function DisplayStringToKeys";
        }
    }
}