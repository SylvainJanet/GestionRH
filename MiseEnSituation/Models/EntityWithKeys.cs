using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Models
{
    public abstract class EntityWithKeys
    {
        public static string KeysToDisplayString(EntityWithKeys e)
        {
            return e.KeysToDisplayString();
        }

        public static object[] DisplayStringToKeys(string s)
        {
            throw new NotImplementedException();
        }

        public abstract string KeysToDisplayString();
    }
}