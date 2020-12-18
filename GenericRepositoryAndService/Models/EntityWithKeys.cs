using GenericRepositoryAndService.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace GenericRepositoryAndService.Models
{
    /// <include file='docs.xml' path='doc/members/member[@name="T:GenericRepositoryAndService.Models.EntityWithKeys"]/*'/>
    public abstract class EntityWithKeys
    {
        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Models.EntityWithKeys.KeysToDisplayString(GenericRepositoryAndService.Models.EntityWithKeys)"]/*'/>
        public static string KeysToDisplayString(EntityWithKeys e)
        {
            return e.KeysToDisplayString();
        }

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Models.EntityWithKeys.DisplayStringToKeys(System.String)"]/*'/>
        public static object[] DisplayStringToKeys(string s)
        {
            throw new ClassHasToImplementDisplayStringToKeysException(typeof(EntityWithKeys));
        }

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Models.EntityWithKeys.KeysToDisplayString"]/*'/>
        public abstract string KeysToDisplayString();
    }
}