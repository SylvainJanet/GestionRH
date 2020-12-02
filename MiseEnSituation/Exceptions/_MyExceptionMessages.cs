using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Exceptions
{
    public abstract class _MyExceptionMessages
    {
        public static string IdNullForClass(Type t)
        {
            return "Object of class " + t.Name + " has null Id";
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

        public static string CannotWriteReadOnlyProperty(Type t, string propertyName)
        {
            return "Cannot write property " + propertyName + "of class " + t.Name + " : it is ReadOnly";
        }
    }
}