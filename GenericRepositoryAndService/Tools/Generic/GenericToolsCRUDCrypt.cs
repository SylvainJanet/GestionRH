using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GenericRepositoryAndService.Tools.Generic
{
    public abstract class GenericToolsCRUDCrypt
    {
        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDCrypt.Crypt``1(``0)"]/*'/>
        public static T Crypt<T>(T t)
        {
            T res = t;
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.GetCustomAttribute(typeof(DataTypeAttribute), false) != null)
                {
                    if (((DataTypeAttribute)property.GetCustomAttribute(typeof(DataTypeAttribute), false)).DataType == DataType.Password)
                    {
                        property.SetValue(res, HashTools.ComputeSha256Hash((string)property.GetValue(res)));
                    }
                }
            }
            return res;
        }

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDCrypt.CryptIfUpdated``1(``0,``0)"]/*'/>
        public static T CryptIfUpdated<T>(T told, T tnew)
        {
            T res = tnew;
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.GetCustomAttribute(typeof(DataTypeAttribute), false) != null)
                {
                    if (((DataTypeAttribute)property.GetCustomAttribute(typeof(DataTypeAttribute), false)).DataType == DataType.Password)
                    {
                        if (property.GetValue(told) != property.GetValue(tnew))
                        {
                            property.SetValue(res, HashTools.ComputeSha256Hash((string)property.GetValue(res)));
                        }
                    }
                }
            }
            return res;
        }

        /// <include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsCRUDCrypt.CryptIfUpdatedOne``1(``0,``0,System.String,System.Object)"]/*'/>
        public static T CryptIfUpdatedOne<T>(T told, T tnew, string propertyName, object newValue)
        {
            T res = tnew;
            PropertyInfo property = typeof(T).GetProperty(propertyName);
            if (property.GetCustomAttribute(typeof(DataTypeAttribute), false) != null)
            {
                if (((DataTypeAttribute)property.GetCustomAttribute(typeof(DataTypeAttribute), false)).DataType == DataType.Password)
                {
                    if (property.GetValue(told) != newValue)
                    {
                        property.SetValue(res, HashTools.ComputeSha256Hash((string)property.GetValue(newValue)));
                    }
                }
            }
            return res;
        }
    }
}