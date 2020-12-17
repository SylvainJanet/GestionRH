using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GenericRepositoryAndService.Tools.Generic
{
    public abstract class GenericToolsCRUDCrypt
    {
        /// <summary>
        /// Crypt every password of <paramref name="t"/> of type <typeparamref name="T"/> using <see cref="HashTools.ComputeSha256Hash(string)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="t">The object</param>
        /// <returns>The object crypted</returns>
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

        /// <summary>
        /// Crypt every password that have changed from <paramref name="told"/> to <paramref name="tnew"/> of type <typeparamref name="T"/> using
        /// <see cref="HashTools.ComputeSha256Hash(string)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="told">The old value of the item, still in DB</param>
        /// <param name="tnew">The new value of the item, with passwords to crypt if changed</param>
        /// <returns>The item crypted</returns>
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

        /// <summary>
        /// Crypt the property <paramref name="propertyName"/> of the object <paramref name="tnew"/> of type <typeparamref name="T"/>
        /// if its value changed using <see cref="HashTools.ComputeSha256Hash(string)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="told">The old value of the item, still in DB</param>
        /// <param name="tnew">The new value of the item</param>
        /// <param name="propertyName">The name of the property to change</param>
        /// <param name="newValue">The new value</param>
        /// <returns>The object crypted</returns>
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