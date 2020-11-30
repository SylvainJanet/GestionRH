using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Tools
{
    public abstract class GenericTools
    {
        /// <summary>
        /// Test if a type implements <see cref="IList{}"/> of <typeparamref name="T"/>, and if so, determine <typeparamref name="T"/>.
        /// </summary>
        /// <param name="type">Type to test : check if it is <see cref="IList{T}"/></param>
        /// <param name="innerType">If <paramref name="type"/> is <see cref="IList{T}"/>, returns the inner type <typeparamref name="T"/></param>
        /// <returns>A boolean representing if <paramref name="type"/> is of type <see cref="IList{T}"/></returns>
        public static bool TryListOfWhat(Type type, out Type innerType)
        {
            Contract.Requires(type != null);

            var interfaceTest = new Func<Type, Type>(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>) ? i.GetGenericArguments().Single() : null);

            innerType = interfaceTest(type);
            if (innerType != null)
            {
                return true;
            }

            foreach (var i in type.GetInterfaces())
            {
                innerType = interfaceTest(i);
                if (innerType != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}