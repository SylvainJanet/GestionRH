using GenericRepositoryAndService.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace GenericRepositoryAndService.Models
{
    /// <summary>
    /// Every classes with at least one property with annotation <see cref="KeyAttribute"/>
    /// have to derive from this class to be properly handled by the
    /// generic repository and service
    /// <br/>
    /// Furthermore,
    /// <list type="bullet">
    /// <item>
    /// For a class t with name "TName", the corresponding repository has to be named "TNameRepository"
    /// </item>
    /// <item>
    /// For a class t with name "TName", the corresponding service has to be named "TNameService"
    /// </item>
    /// </list>
    /// </summary>
    public abstract class EntityWithKeys
    {
        /// <summary>
        /// Converts an object of type <see cref="EntityWithKeys"/> to a string used
        /// to display its keys. The reverse operation is <see cref="DisplayStringToKeys(string)"/>
        /// </summary>
        /// <param name="e">The object of type <see cref="EntityWithKeys"/></param>
        /// <returns>The string displayed</returns>
        public static string KeysToDisplayString(EntityWithKeys e)
        {
            return e.KeysToDisplayString();
        }

        /// <summary>
        /// The reverse operation of <see cref="KeysToDisplayString(EntityWithKeys)"/>. From a string used
        /// to display the key of an object of type <see cref="EntityWithKeys"/>, get the keys as
        /// <see cref="object"/>[].
        /// </summary>
        /// <remarks>
        /// Has to be redifined for every class deriving from <see cref="EntityWithKeys"/>.
        /// </remarks>
        /// <param name="s">The string of keys</param>
        /// <returns>The keys as <see cref="object"/>[]</returns>
        public static object[] DisplayStringToKeys(string s)
        {
            throw new ClassHasToImplementDisplayStringToKeysException(typeof(EntityWithKeys));
        }

        /// <summary>
        /// Create a new definition for <see cref="DisplayStringToKeys(string)"/>
        /// <br/>
        /// <example>
        /// For instance, for a class with a single key 
        /// <br/>
        /// <code>
        /// public override string KeysToDisplayString() <br/>
        /// { <br/>
        /// retrun this.keypropertyname.ToString(); <br/>
        /// }
        /// </code>
        /// </example>
        /// </summary>
        public abstract string KeysToDisplayString();
    }
}