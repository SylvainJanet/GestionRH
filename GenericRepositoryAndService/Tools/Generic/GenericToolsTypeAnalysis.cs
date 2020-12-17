using GenericRepositoryAndService.Exceptions;
using GenericRepositoryAndService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace GenericRepositoryAndService.Tools.Generic
{
    public abstract class GenericToolsTypeAnalysis
    {
        /// <summary>
        /// Test if a type implements <see cref="IList{}"/> of <typeparamref name="T"/>, and if so, determine <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// Thanks internet for this one.
        /// </remarks>
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

        /// <summary>
        /// Get the names of the properties of <typeparamref name="T"/> that have the annotation 
        /// <see cref="KeyAttribute"/>. The order will be the same as the order in the declaration of <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// If <typeparamref name="T"/> derives from <see cref="BaseEntity"/>, returns <see langword="null"/>.
        /// </remarks>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns>The properties names that are keys, in the same order as in the declaration of <typeparamref name="T"/>. If
        /// <typeparamref name="T"/> derives from <see cref="BaseEntity"/>, returns <see langword="null"/>.</returns>
        public static string[] KeyPropertiesNames<T>()
        {
            if (typeof(T).IsSubclassOf(typeof(BaseEntity)))
                return null;
            return typeof(T).GetProperties().Where(p => 
                                                   p.GetCustomAttribute(typeof(KeyAttribute), false) != null
                                                   )
                                            .Select(p => p.Name)
                                            .ToArray();
        }

        /// <summary>
        /// Tells if <typeparamref name="T"/> is in a relationship with any other class in DB.
        /// </summary>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns><see langword="true"/> if <typeparamref name="T"/> is in any relationship, <see langword="false"/> otherwise.</returns>
        public static bool HasDynamicDBTypeOrListType<T>()
        {
            return DynamicDBListTypes<T>().Count != 0 || DynamicDBTypes<T>().Count != 0;
        }

        /// <summary>
        /// An object <c>obj</c> of class <typeparamref name="T"/> has properties <c>obj.PropName</c> of
        /// class <see cref="IList"/>&lt;<c>ClassType</c>&gt; where <c>ClassType</c> is in a <see cref="DbSet"/> of the generic repository. 
        /// <br/>
        /// This is every { PropName : ClassType }
        /// </summary>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns>The dictionary</returns>
        public static Dictionary<string, Type> DynamicDBListTypes<T>()
        {
            return DynamicDBListTypesForType(typeof(T));
        }

        /// <summary>
        /// An object <c>obj</c> of class <typeparamref name="T"/> has properties <c>obj.PropName</c> of
        /// class <c>ClassType</c> which is in a <see cref="DbSet"/> of the generic repository. 
        /// <br/>
        /// This is every { PropName : ClassType }
        /// </summary>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <returns>The dictionary</returns>
        public static Dictionary<string, Type> DynamicDBTypes<T>()
        {
            return DynamicDBTypesForType(typeof(T));
        }

        /// <summary>
        /// An object <c>obj</c> of class <typeparamref name="T"/> has properties <c>obj.PropName</c> of
        /// class <see cref="IList"/>&lt;<c>ClassType</c>&gt; where <c>ClassType</c> is in a <see cref="DbSet"/>. 
        /// <br/>
        /// This is every { PropName : ClassType }
        /// </summary>
        /// <param name="t">The type invistigated.</param>
        /// <returns>The dictionary</returns>
        public static Dictionary<string, Type> DynamicDBListTypesForType(Type t)
        {
            Dictionary<string, Type> res = new Dictionary<string, Type>();
            foreach (PropertyInfo property in t.GetProperties())
            {
                if (TryListOfWhat(property.PropertyType, out Type innerType))
                {
                    if (innerType.IsSubclassOf(typeof(BaseEntity)) ||
                        innerType.GetProperties().Where(p => 
                                                        p.GetCustomAttribute(typeof(KeyAttribute), false) != null
                                                        )
                                                 .Count() > 0)
                    {
                        res.Add(property.Name, innerType);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// An object <c>obj</c> of class <typeparamref name="T"/> has properties <c>obj.PropName</c> of
        /// class <c>ClassType</c> which is in a <see cref="DbSet"/>. 
        /// <br/>
        /// This is every { PropName : ClassType }
        /// </summary>
        /// <param name="t">The type invistigated.</param>
        /// <returns>The dictionary</returns>
        public static Dictionary<string, Type> DynamicDBTypesForType(Type t)
        {
            Dictionary<string, Type> res = new Dictionary<string, Type>();
            foreach (PropertyInfo property in t.GetProperties())
            {
                if (property.PropertyType.IsSubclassOf(typeof(BaseEntity)) ||
                    property.PropertyType.GetProperties().Where(p => 
                                                                p.GetCustomAttribute(typeof(KeyAttribute), false) != null
                                                                )
                                                         .Count() > 0)
                {
                    res.Add(property.Name, property.PropertyType);
                }
            }
            return res;
        }

        /// <summary>
        /// Checks if <paramref name="objs"/> is either
        /// <list type="bullet">
        /// <item>
        /// an integer, the Id, if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// an array (of proper length) of objects (of proper types) corresponding to the keys of <typeparamref name="T"/>.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type invistigated.</typeparam>
        /// <param name="objs">The array of object tested.</param>
        /// <exception cref="InvalidKeyForClassException"/>
        public static void CheckIfObjectIsKey<T>(object[] objs)
        {
            if (typeof(T).IsSubclassOf(typeof(BaseEntity)))
            {
                if (objs.Length != 1 || !(objs[0] is int))
                    throw new InvalidKeyForClassException(typeof(T));
            }
            else
            {
                if (objs.Length != KeyPropertiesNames<T>().Length)
                    throw new InvalidKeyForClassException(typeof(T));
                int len = KeyPropertiesNames<T>().Length;
                for (int i = 0; i < len; i++)
                {
                    object ototest = objs[i];
                    string propname = KeyPropertiesNames<T>()[i];
                    if (!typeof(T).GetProperty(propname).PropertyType.IsAssignableFrom(ototest.GetType()))
                        throw new InvalidKeyForClassException(typeof(T));
                }
            }
        }

        /// <summary>
        /// From an array of objects, supposed to be either Id or keys, get the Id
        /// </summary>
        /// <typeparam name="T">The type investifated</typeparam>
        /// <param name="objs">The objects to cast to int as an Id</param>
        /// <returns>The Id</returns>
        /// <exception cref="InvalidKeyForClassException"/>
        public static int? ObjectsToId<T>(object[] objs)
        {
            if (!typeof(T).IsSubclassOf(typeof(BaseEntity)))
                throw new InvalidKeyForClassException(typeof(T));
            if ((objs.Length != 1) || !(objs[0] is int))
                throw new InvalidKeyForClassException(typeof(T));
            return (int?)objs[0];
        }

        /// <summary>
        /// From an object <paramref name="t"/> of type <typeparamref name="T"/>, get either
        /// <list type="bullet">
        /// <item>
        /// the id of <paramref name="t"/> if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// otherwise the array of keys of <paramref name="t"/>
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="t">The object</param>
        /// <returns>Either the Id of <paramref name="t"/> or its keys values</returns>
        public static object[] GetKeysValues<T>(T t)
        {
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                return new object[] { ((int?)typeof(T).GetProperty("Id").GetValue(t)).Value };
            }
            else
            {
                object[] res = new object[KeyPropertiesNames<T>().Length];
                int i = 0;
                foreach (string propname in KeyPropertiesNames<T>())
                {
                    object value = typeof(T).GetProperty(propname).GetValue(t);
                    res[i] = Convert.ChangeType(value, 
                                                typeof(T).GetProperty(propname).PropertyType);
                    i++;
                }
                return res;
            }
        }

        /// <summary>
        /// Call the generic method <see cref="GetKeysValues{T}(T)"/> dynamically. 
        /// <br/>
        /// That is, get either the Id or the keys values
        /// of an object <paramref name="item"/> of type <paramref name="typeofElement"/>.
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="typeofElement">The type of the object</param>
        /// <returns>Either the Id or the keys values of <paramref name="item"/></returns>
        public static object[] GetKeysValuesForType(object item, Type typeofElement)
        {
            MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                             .MakeGenericMethod(new[] { typeofElement });
            return (object[])methodGetKeysValues.Invoke(typeof(GenericToolsTypeAnalysis), new object[] { 
                                                                                                        item 
                                                                                                        });
        }

        /// <summary>
        /// Assuming <typeparamref name="T"/> does not derive from <see cref="BaseEntity"/>, check
        /// if <paramref name="objs"/> is an array of keys.
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="objs">The array that must be many keys values.</param>
        /// <exception cref="InvalidKeyForClassException"/>
        private static void CheckIfObjsIsManyKeys<T>(object[] objs)
        {
            int nbrkeys = KeyPropertiesNames<T>().Length;
            if (objs.Length == 0 || objs.Length % nbrkeys != 0)
                throw new InvalidKeyForClassException(typeof(T));
            int nbrobjs = objs.Length / nbrkeys;
            for (int i = 0; i < nbrobjs; i++)
            {
                object[] keystotest = new object[nbrkeys];
                for (int j = 0; j < nbrkeys; j++)
                {
                    keystotest[j] = objs[i * nbrkeys + j];
                }
                CheckIfObjectIsKey<T>(objs);
            }
        }

        /// <summary>
        /// From a one dimensionnal array of keys values <paramref name="objs"/>, get an array
        /// of array containing key values.
        /// <br/>
        /// <example>
        /// From <c>{ key1value1, key2value1, key1value2, key2value2 }</c> get
        /// <c>{ { key1value1, key2value1 } , { key1value2, key2value2 } }</c>
        /// </example>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="objs">The array of keys values</param>
        /// <returns>The array of array containing key values.</returns>
        public static object[][] GetManyKeys<T>(object[] objs)
        {
            int nbrkeys = KeyPropertiesNames<T>().Length;
            int nbrobjs = objs.Length / nbrkeys;
            object[][] res = new object[nbrobjs][];
            for (int i = 0; i < nbrobjs; i++)
            {
                res[i] = new object[nbrkeys];
                for (int j = 0; j < nbrkeys; j++)
                {
                    res[i][j] = objs[i * nbrkeys + j];
                }
            }
            return res;
        }

        /// <summary>
        /// Checks if <paramref name="objs"/> is either
        /// <list type="bullet">
        /// <item>
        /// many Ids if <typeparamref name="T"/> derives from <see cref="BaseEntity"/>
        /// </item>
        /// <item>
        /// or many keys values.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="objs">The array that must either be many Ids or manys keys values.</param>
        /// <exception cref="IdListEmptyForClassException"/>
        /// <exception cref="InvalidKeyForClassException" />
        public static void CheckIfObjsIsManyKeysOrIds<T>(params object[] objs)
        {
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                if (objs.Length == 0)
                    throw new IdListEmptyForClassException(typeof(T));
                for (int i = 0; i < objs.Length; i++)
                {
                    if (!typeof(int?).IsAssignableFrom(objs[i].GetType()) && 
                        !typeof(int?).IsAssignableFrom(objs[i].GetType()) && 
                        !int.TryParse(objs[i].ToString(), out _)
                        )
                        throw new InvalidKeyForClassException(typeof(T));
                }
            }
            else
            {
                CheckIfObjsIsManyKeys<T>(objs);
            }
        }

        /// <summary>
        /// From an array <paramref name="objs"/> of objects containing many Ids,
        /// get an array <see cref="int"/>?[] containing the Ids.
        /// </summary>
        /// <param name="objs">The array to convert.</param>
        /// <returns>The array of Ids.</returns>
        public static int?[] GetManyIds(params object[] objs)
        {
            int?[] ids = new int?[objs.Length];
            for (int i = 0; i < objs.Length; i++)
            {
                try
                {
                    ids[i] = (int?)objs[i];
                }
                catch
                {
                    int.TryParse((string)objs[i], out int j);
                    ids[i] = (int?)j;
                }
            }
            return ids;
        }

        /// <summary>
        /// Return <see langword="true"/> if and only if <paramref name="t1"/> has a property of type either <paramref name="t2"/>
        /// or <see cref="IList"/>&lt;<paramref name="t2"/>&gt;. That is to say that <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and that <paramref name="t1"/> has a property concerning this relationship.
        /// </summary>
        /// <param name="t1">The type for which properties have to be invistigated</param>
        /// <param name="t2">The type to check whether or not is is in a relationship with <paramref name="t1"/>
        /// for which <paramref name="t1"/> has a property.</param>
        /// <returns><see langword="true"/> if <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and <paramref name="t1"/> has a property concerning this relationship. <see langword="false"/>
        /// otherwise</returns>
        private static bool HasPropertyRelation(Type t1, Type t2)
        {
            return HasPropertyRelationNotList(t1, t2) || HasPropertyRelationList(t1, t2);
        }

        /// <summary>
        /// Return <see langword="true"/> if and only if <paramref name="t1"/> has a property of type <paramref name="t2"/>
        /// That is to say that <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and that <paramref name="t1"/> has a property concerning this relationship of type <paramref name="t2"/>
        /// </summary>
        /// <param name="t1">The type for which properties have to be invistigated</param>
        /// <param name="t2">The type to check whether or not is is in a relationship with <paramref name="t1"/>
        /// for which <paramref name="t1"/> has a property of type <paramref name="t2"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and <paramref name="t1"/> has a property concerning this relationship of type <paramref name="t2"/>. 
        /// <see langword="false"/> otherwise</returns>
        public static bool HasPropertyRelationNotList(Type t1, Type t2)
        {
            return DynamicDBTypesForType(t1).Values.Contains(t2);
        }

        /// <summary>
        /// Return <see langword="true"/> if and only if <paramref name="t1"/> has a property of type <see cref="IList"/>&lt;<paramref name="t2"/>&gt;
        /// That is to say that <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and that <paramref name="t1"/> has a property concerning this relationship of type 
        /// <see cref="IList"/>&lt;<paramref name="t2"/>&gt;
        /// </summary>
        /// <param name="t1">The type for which properties have to be invistigated</param>
        /// <param name="t2">The type to check whether or not is is in a relationship with <paramref name="t1"/>
        /// for which <paramref name="t1"/> has a property of type <see cref="IList"/>&lt;<paramref name="t2"/>&gt;.</param>
        /// <returns><see langword="true"/> if <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and <paramref name="t1"/> has a property concerning this relationship of type 
        /// <see cref="IList"/>&lt;<paramref name="t2"/>&gt;. <see langword="false"/> otherwise</returns>
        public static bool HasPropertyRelationList(Type t1, Type t2)
        {
            return DynamicDBListTypesForType(t1).Values.Contains(t2);
        }

        /// <summary>
        /// Returns whether or not the type <paramref name="t1"/> has a property representing a relationship
        /// with <paramref name="t2"/>, that is to say of type <paramref name="t2"/> or <see cref="IList"/>&lt;<paramref name="t2"/>&gt;
        /// which has the annotation <see cref="RequiredAttribute"/>.
        /// </summary>
        /// <param name="t1">The type for which properties have to be invistigated</param>
        /// <param name="t2">The type to check whether or not is is in a relationship with <paramref name="t1"/>
        /// for which <paramref name="t1"/> has a property with the annotation <see cref="RequiredAttribute"/>.</param>
        /// <returns></returns>
        private static bool HasPropertyRelationRequired(Type t1, Type t2)
        {
            if (HasPropertyRelationNotList(t1, t2))
            {
                List<string> PropNames = DynamicDBTypesForType(t1).Where(propnametype => 
                                                                         propnametype.Value == t2
                                                                         )
                                                                  .Select(propnametype => 
                                                                          propnametype.Key
                                                                          )
                                                                  .ToList();
                return t1.GetProperties().Where(p => 
                                                PropNames.Contains(p.Name)
                                                )
                                         .Where(p => 
                                                p.GetCustomAttribute(typeof(RequiredAttribute), false) != null
                                                )
                                         .Count() != 0;
            }
            if (HasPropertyRelationList(t1, t2))
            {
                List<string> PropNames = DynamicDBListTypesForType(t1).Where(propnametype => 
                                                                             propnametype.Value == t2
                                                                             )
                                                                      .Select(propnametype => 
                                                                              propnametype.Key
                                                                              )
                                                                      .ToList();
                return t1.GetProperties().Where(p => 
                                                PropNames.Contains(p.Name)
                                                )
                                         .Where(p =>
                                                p.GetCustomAttribute(typeof(RequiredAttribute), false) != null
                                                )
                                         .Count() != 0;
            }
            return false;
        }

        /// <summary>
        /// Get all types t that are in relation with <typeparamref name="T"/> so that :
        /// <list type="bullet">
        /// <item>
        /// t has a property representing that relationship, that is to say either of type <typeparamref name="T"/> or <see cref="IList{T}"/>
        /// </item>
        /// <item>
        /// <typeparamref name="T"/> has no property representing that relationship.
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <returns>The list of types in relationship with <typeparamref name="T"/> that have a property for the relation and
        /// <typeparamref name="T"/> has no such property.</returns>
        public static IEnumerable<Type> GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty<T>()
        {
            List<Type> res = new List<Type>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                                                                 .Where(myType => 
                                                                        myType.IsClass && 
                                                                        !myType.IsAbstract && 
                                                                        (
                                                                            myType.IsSubclassOf(typeof(BaseEntity)) || 
                                                                            myType.IsSubclassOf(typeof(EntityWithKeys))
                                                                        )
                                                                       ))
            {
                if (HasPropertyRelation(type, typeof(T)))
                    if (!HasPropertyRelation(typeof(T), type))
                        res.Add(type);
            }
            return res;
        }

        /// <summary>
        /// Get all types t that are in relation with <typeparamref name="T"/> so that :
        /// <list type="bullet">
        /// <item>
        /// t has a property representing that relationship, that is to say either of type <typeparamref name="T"/> or <see cref="IList{T}"/>
        /// </item>
        /// <item>
        /// that property is has the annotation <see cref="RequiredAttribute"/>
        /// </item>
        /// </list>
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <returns>The list of types in relationship with <typeparamref name="T"/> that have a property for the relation 
        /// with the annotation <see cref="RequiredAttribute"/></returns>
        public static IEnumerable<Type> GetTypesInRelationWithTHavingRequiredTProperty<T>()
        {
            List<Type> res = new List<Type>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                                                                 .Where(myType => 
                                                                        myType.IsClass && 
                                                                        !myType.IsAbstract &&
                                                                        (
                                                                            myType.IsSubclassOf(typeof(BaseEntity)) || 
                                                                            myType.IsSubclassOf(typeof(EntityWithKeys))
                                                                        )
                                                                       ))
            {
                if (HasPropertyRelationRequired(type, typeof(T)))
                    res.Add(type);
            }
            return res;
        }

        /// <summary>
        /// Return <see langword="true"/> if and only if <paramref name="t1"/> has many property of type either <paramref name="t2"/>
        /// or <see cref="IList"/>&lt;<paramref name="t2"/>&gt;. That is to say that <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and that <paramref name="t1"/> has more than one property concerning this relationship.
        /// </summary>
        /// <param name="t1">The type for which properties have to be invistigated</param>
        /// <param name="t2">The type to check whether or not is is in a relationship with <paramref name="t1"/>
        /// for which <paramref name="t1"/> has more than one property.</param>
        /// <returns><see langword="true"/> if <paramref name="t1"/> and <paramref name="t2"/> are in
        /// a relationship and <paramref name="t1"/> has more than one property concerning this relationship. <see langword="false"/>
        /// otherwise</returns>
        private static bool HasManyProperties(Type t1, Type t2)
        {
            int countNotList = DynamicDBTypesForType(t1).Values.Where(t => 
                                                                      t == t2
                                                                      )
                                                               .Count();
            int countList = DynamicDBListTypesForType(t1).Values.Where(t => 
                                                                       t == t2
                                                                       )
                                                                .Count();
            return countNotList + countList > 1;
        }

        /// <summary>
        /// Get the list of types t that are many relationships with <typeparamref name="T"/>, that is to say that
        /// <typeparamref name="T"/> has many properties of types either t or <see cref="IList"/>&lt;t&gt;.
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <returns>The list of types</returns>
        public static IEnumerable<Type> GetTypesForWhichTHasManyProperties<T>()
        {
            List<Type> res = new List<Type>();
            foreach (Type type in DynamicDBTypes<T>().Values.Concat(DynamicDBListTypes<T>().Values))
            {
                if (HasManyProperties(typeof(T), type))
                    res.Add(type);
            }
            return res;
        }

        /// <summary>
        /// Get the list of types t that are in exactly one relationship with <typeparamref name="T"/>, that is to say that
        /// <typeparamref name="T"/> has exactly one property of types either t or <see cref="IList"/>&lt;t&gt;.
        /// </summary>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <returns>The list of types</returns>
        public static IEnumerable<Type> GetTypesForWhichTHasOneProperty<T>()
        {
            List<Type> res = new List<Type>();
            foreach (Type type in DynamicDBTypes<T>().Values.Concat(DynamicDBListTypes<T>().Values))
            {
                if (!HasManyProperties(typeof(T), type))
                    res.Add(type);
            }
            return res;
        }

        /// <summary>
        /// Get the number of keys of type <paramref name="q"/> if appropriate
        /// </summary>
        /// <param name="q">The type invistigated</param>
        /// <returns>The number of keys of type <paramref name="q"/>, 1 if it has an Id, and 0 otherwise.</returns>
        public static int NbrOfKeys(Type q)
        {
            if (q.IsSubclassOf(typeof(BaseEntity)))
                return 1;
            if (q.IsSubclassOf(typeof(EntityWithKeys)))
            {
                return q.GetProperties().Where(p => 
                                               p.GetCustomAttribute(typeof(KeyAttribute), false) != null
                                               )
                                        .Count();
            }
            return 0;
        }

        /// <summary>
        /// For two types <paramref name="t1"/>, <paramref name="t2"/>, get a Dictionnary of property names (key, value) such that :
        /// <list type="bullet">
        /// <item>
        /// There is a relationship between <paramref name="t1"/> and <paramref name="t2"/> 
        /// </item>
        /// <item>
        /// The corresponding property of <paramref name="t1"/> has name key
        /// </item>
        /// <item>
        /// The corresponding property of <paramref name="t2"/> is required and has name value
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="t1">The first type</param>
        /// <param name="t2">The second type</param>
        /// <returns>The dictionnary</returns>
        public static Dictionary<string, string> PropNamesForRelationWithTWithRequired(Type t1, Type t2)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            int jchecked = -1;
            for (int i = 0; i < t1.GetProperties().Length; i++)
            {
                if (t1.GetProperties()[i].PropertyType == t2 || 
                    t1.GetProperties()[i].PropertyType == typeof(IList<>).MakeGenericType(t2))
                {
                    for (int j = jchecked + 1; j < t2.GetProperties().Length; j++)
                    {
                        if (t2.GetProperties()[j].PropertyType == t1 || 
                            t2.GetProperties()[j].PropertyType == typeof(IList<>).MakeGenericType(t1))
                        {
                            if (t2.GetProperties()[j].GetCustomAttribute(typeof(RequiredAttribute), false) != null)
                            {
                                res.Add(t1.GetProperties()[i].Name, t2.GetProperties()[j].Name);
                            }
                            jchecked = j;
                            break;
                        }
                    }
                }
            }
            return res;
        }
    }
}