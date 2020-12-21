using GenericRepositoryAndService.Exceptions;
using GenericRepositoryAndService.Models;
using System;
using System.Collections;
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
        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.TryListOfWhat(System.Type,System.Type@)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.KeyPropertiesNames``1"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.HasDynamicDBTypeOrListType``1"]/*'/>
        public static bool HasDynamicDBTypeOrListType<T>()
        {
            return DynamicDBListTypes<T>().Count != 0 || DynamicDBTypes<T>().Count != 0;
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.DynamicDBListTypes``1"]/*'/>
        public static Dictionary<string, Type> DynamicDBListTypes<T>()
        {
            return DynamicDBListTypesForType(typeof(T));
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.DynamicDBTypes``1"]/*'/>
        public static Dictionary<string, Type> DynamicDBTypes<T>()
        {
            return DynamicDBTypesForType(typeof(T));
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.DynamicDBListTypesForType(System.Type)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.DynamicDBTypesForType(System.Type)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.CheckIfObjectIsKey``1(System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.ObjectsToId``1(System.Object[])"]/*'/>
        public static int? ObjectsToId<T>(object[] objs)
        {
            if (!typeof(T).IsSubclassOf(typeof(BaseEntity)))
                throw new InvalidKeyForClassException(typeof(T));
            if ((objs.Length != 1) || !(objs[0] is int))
                throw new InvalidKeyForClassException(typeof(T));
            return (int?)objs[0];
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.GetKeysValues``1(``0)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.GetKeysValuesForType(System.Object,System.Type)"]/*'/>
        public static object[] GetKeysValuesForType(object item, Type typeofElement)
        {
            MethodInfo methodGetKeysValues = typeof(GenericToolsTypeAnalysis).GetMethod("GetKeysValues", BindingFlags.Public | BindingFlags.Static)
                                                                             .MakeGenericMethod(new[] { typeofElement });
            return (object[])methodGetKeysValues.Invoke(typeof(GenericToolsTypeAnalysis), new object[] { 
                                                                                                        item 
                                                                                                        });
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.CheckIfObjsIsManyKeys``1(System.Object[])"]/*'/>
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
                CheckIfObjectIsKey<T>(keystotest);
            }
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.GetManyKeys``1(System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.CheckIfObjsIsManyKeysOrIds``1(System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.GetManyIds(System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.HasPropertyRelation(System.Type,System.Type)"]/*'/>
        private static bool HasPropertyRelation(Type t1, Type t2)
        {
            return HasPropertyRelationNotList(t1, t2) || HasPropertyRelationList(t1, t2);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.HasPropertyRelationNotList(System.Type,System.Type)"]/*'/>
        public static bool HasPropertyRelationNotList(Type t1, Type t2)
        {
            return DynamicDBTypesForType(t1).Values.Contains(t2);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.HasPropertyRelationList(System.Type,System.Type)"]/*'/>
        public static bool HasPropertyRelationList(Type t1, Type t2)
        {
            return DynamicDBListTypesForType(t1).Values.Contains(t2);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.HasPropertyRelationRequired(System.Type,System.Type)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingTPropertyTAndTNotHavingProperty``1"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.GetTypesInRelationWithTHavingRequiredTProperty``1"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.HasManyProperties(System.Type,System.Type)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.GetTypesForWhichTHasManyProperties``1"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.GetTypesForWhichTHasOneProperty``1"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.NbrOfKeys(System.Type)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsTypeAnalysis.PropNamesForRelationWithTWithRequired(System.Type,System.Type)"]/*'/>
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