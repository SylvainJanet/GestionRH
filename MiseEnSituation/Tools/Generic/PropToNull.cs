using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiseEnSituation.Tools.Generic
{
    public class PropToNull
    {
        public string PropertyName { get; set; }

        public PropToNull(string propertyName)
        {
            PropertyName = propertyName;
        }
    }
}