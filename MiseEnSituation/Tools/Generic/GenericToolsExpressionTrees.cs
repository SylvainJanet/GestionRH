using MiseEnSituation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace MiseEnSituation.Tools.Generic
{
    public abstract class GenericToolsExpressionTrees
    {
        /// <summary>
        /// Construct the expression tree for a lambda expression (<c>o => o.prop == value</c>) dynamically
        /// <br/>
        /// Purpose : LINQ to SQL interpretation will be successfull. 
        /// Something using reflection such as 
        /// <br/>
        /// <c>o => o.GetType().GetProperty("prop").GetValue(o)==value</c>
        /// <br/>
        /// doesn't work : LINQ to ENTITY won't work.
        /// <br/>
        /// The object is of type <typeparamref name="TItem"/>, value is of type <typeparamref name="TValue"/>, 
        /// the property to check is <paramref name="property"/> and the value is <paramref name="value"/>.
        /// </summary>
        /// <remarks>
        /// Thanks internet for this one (had to modify it a little bit though)
        /// </remarks>
        /// <typeparam name="TItem">The type of the object the lambda expression will test whether or not the 
        /// property <paramref name="property"/> is equal to <paramref name="value"/></typeparam>
        /// <typeparam name="TValue">Tje type of <paramref name="value"/></typeparam>
        /// <param name="property">The property</param>
        /// <param name="value">The value</param>
        /// <returns>The lambda expression properly constructed so that LINQ to SQL interpretation will be
        /// successfull</returns>
        public static Expression<Func<TItem, bool>> PropertyEquals<TItem>(PropertyInfo property, object value)
        {
            var param = Expression.Parameter(typeof(TItem));
            var body = Expression.Equal(
                                            Expression.Property(param, property),
                                            Expression.Constant(value, property.PropertyType)
                                        );
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        /// <summary>
        /// Construct the expression tree for a lambda expression (<c>o => o.prop</c>) dynamically
        /// <br/>
        /// Purpose : LINQ to SQL interpretation will be successfull. 
        /// Something using reflection such as 
        /// <br/>
        /// <c>o => o.GetType().GetProperty("prop").GetValue(o)</c>
        /// <br/>
        /// doesn't work : LINQ to ENTITY won't work.
        /// <br/>
        /// The object is of type <typeparamref name="TItem"/>, 
        /// the property to specify is <paramref name="property"/> and is of type <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of the object the lambda expression will specify the property
        /// <paramref name="property"/>.</typeparam>
        /// <typeparam name="TKey">The type of the property <paramref name="property"/></typeparam>
        /// <param name="property">The property to specify</param>
        /// <returns>The lambda expression properly constructed so that LINQ to SQL interpretation will be
        /// successfull</returns>
        public static Expression<Func<TItem, TKey>> GetKey<TItem, TKey>(PropertyInfo property)
        {
            var param = Expression.Parameter(typeof(TItem));
            var body = Expression.Property(param, property);
            var bodyconverted = Expression.Convert(body, typeof(TKey));
            return Expression.Lambda<Func<TItem, TKey>>(bodyconverted, param);
        }

        /// <summary>
        /// Build the expression tree for <c>o => (o.prop1.prop2 == value)</c> where o is of type <typeparamref name="TItem"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of item at the root of the expression tree</typeparam>
        /// <param name="prop1">The first property to access</param>
        /// <param name="prop2">The second property to access</param>
        /// <param name="value">The value</param>
        /// <returns>The expression tree</returns>
        public static Expression<Func<TItem, bool>> ExpressionPropPropEquals<TItem>(PropertyInfo prop1, PropertyInfo prop2, object value)
        {
            var param = Expression.Parameter(typeof(TItem));
            var exp = Expression.Property(param, prop1);
            var body = Expression.Equal(
                                            Expression.Property(exp, prop2),
                                            Expression.Constant(value, prop2.PropertyType)
                                        );
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        /// <summary>
        /// Build the expression tree for <c>o => o.prop != null</c> where o is of type <typeparamref name="TItem"/>.
        /// </summary>
        /// <typeparam name="TItem">The type of item at the root of the expression tree</typeparam>
        /// <param name="prop">The property to access</param>
        /// <returns>The expression tree</returns>
        public static Expression<Func<TItem, bool>> PropertyKeysNotNull<TItem>(PropertyInfo prop)
        {
            var param = Expression.Parameter(typeof(TItem));
            var exp = Expression.Property(param, prop);
            var body = Expression.IsFalse(Expression.Equal(
                                                            exp, 
                                                            Expression.Constant(null)
                                                           ));
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        /// <summary>
        /// Get an expression tree with principal root of type <typeparamref name="T"/>, checking whether or not
        /// the element has either Id or keys equal to <paramref name="objs"/>.
        /// <br/>
        /// Essentially, does either <c>t => t.Id == id</c> or <c>t => t.key1 == key1value &amp;&amp; ... &amp;&amp; t.keyn = keynvalue</c>
        /// </summary>
        /// <typeparam name="T">The type in question</typeparam>
        /// <param name="objs">Either the Id or the keys</param>
        /// <returns>The expression tree</returns>
        public static Expression<Func<T, bool>> ExpressionWhereKeysAre<T>(params object[] objs)
        {
            var param = Expression.Parameter(typeof(T));
            Expression body;
            if (typeof(BaseEntity).IsAssignableFrom(typeof(T)))
            {
                int id = (int)objs[0];
                body = Expression.Equal(Expression.Property(
                                                                param, 
                                                                typeof(T).GetProperty("Id")
                                                            ),
                                        Expression.Constant(
                                                                id, 
                                                                typeof(T).GetProperty("Id").PropertyType
                                                            ));
            }
            else
            {
                body = Expression.Equal(Expression.Property(
                                                                param, 
                                                                typeof(T).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[0])
                                                            ),
                                        Expression.Constant(
                                                                objs[0], 
                                                                typeof(T).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[0]).PropertyType
                                                            ));
                for (int i = 1; i < objs.Length; i++)
                {
                    body = Expression.And(body,
                                          Expression.Equal(Expression.Property(
                                                                                param, 
                                                                                typeof(T).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[i])
                                                                               ),
                                                           Expression.Constant(
                                                                                objs[i], 
                                                                                typeof(T).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[i]).PropertyType)
                                                                               ));
                }
            }
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        /// <summary>
        /// Get an expression tree having root of type <typeparamref name="Q"/>. These elements have a property <paramref name="prop"/>
        /// which is a list of elements of type <typeparamref name="T"/>. 
        /// <br/>
        /// This returns essentially : <c>q => q.prop.Where(t => t.keysorId == objs).Count() >= 1</c>
        /// <br/>
        /// That is to say, for an element of type <typeparamref name="Q"/> so that their property <paramref name="prop"/> is
        /// a <see cref="IList{T}"/>, whether or not it contains an element with either Id or Key given by <paramref name="objs"/>.
        /// </summary>
        /// <typeparam name="T">The most nested type</typeparam>
        /// <typeparam name="Q">The type of the expression tree root</typeparam>
        /// <param name="prop">The property of <typeparamref name="Q"/> in question</param>
        /// <param name="objs">Either the Id or the Key</param>
        /// <returns>The expression tree.</returns>
        public static Expression<Func<Q, bool>> ExpressionListWherePropListCountainsElementWithGivenKeys<T, Q>(PropertyInfo prop, params object[] objs)
        {
            var param = Expression.Parameter(typeof(Q));
            // exp = q => q.prop
            var exp = Expression.Property(param, prop);

            MethodInfo methodExpressionWhereKeysAre = typeof(GenericToolsExpressionTrees).GetMethod(
                                                                                                    "ExpressionWhereKeysAre", 
                                                                                                    BindingFlags.Public | BindingFlags.Static
                                                                                                    )
                                                                                         .MakeGenericMethod(typeof(T));
            // exp2 = t => t.Id == id OR exp2 = t => t.key1 == key1value && ... && t.keyn == keynvalue
            Expression exp2 = (Expression)methodExpressionWhereKeysAre.Invoke(
                                                                                typeof(GenericToolsExpressionTrees), 
                                                                                new object[] { 
                                                                                                objs 
                                                                                              }
                                                                              );

            MethodInfo methodWhere = typeof(Enumerable).GetMethods()
                                                       .Where(m => m.Name == "Where")
                                                       .ToList()[0]
                                                       .MakeGenericMethod(typeof(T));
            // body = q => q.prop.Where(exp2)
            Expression body = Expression.Call(methodWhere, exp, exp2);

            MethodInfo methodCount = typeof(Enumerable).GetMethods()
                                                       .Where(m => 
                                                              m.Name == "Count" && 
                                                              m.GetParameters().Length == 1)
                                                       .Single()
                                                       .MakeGenericMethod(typeof(T));
            // body = q => q.prop.Where(exp2).Count()
            body = Expression.Call(methodCount, body);

            // body = q => q.prop.Where(exp2).Count() >= 1
            body = Expression.GreaterThanOrEqual(body, Expression.Constant(1));
            return Expression.Lambda<Func<Q, bool>>(body, param);
        }

        /// <summary>
        /// Get an expression tree having root of type <typeparamref name="Q"/>. These elements have a property <paramref name="prop"/>
        /// which is a list of elements of type <typeparamref name="T"/>. 
        /// <br/>
        /// This returns essentially : <c>q => q.prop.Where(t => t.keysorId == objs).Count() == 1 &amp;&amp; q.prop.Count() == 1</c>
        /// <br/>
        /// That is to say, for an element of type <typeparamref name="Q"/> so that their property <paramref name="prop"/> is
        /// a <see cref="IList{T}"/>, whether or not it contains only an element with either Id or Key given by <paramref name="objs"/>.
        /// </summary>
        /// <typeparam name="T">The most nested type</typeparam>
        /// <typeparam name="Q">The type of the expression tree root</typeparam>
        /// <param name="prop">The property of <typeparamref name="Q"/> in question</param>
        /// <param name="objs">Either the Id or the Key</param>
        /// <returns>The expression tree.</returns>
        public static Expression<Func<Q, bool>> ExpressionListWherePropListCountainsOnlyElementWithGivenKeys<T, Q>(PropertyInfo prop, params object[] objs)
        {
            var param = Expression.Parameter(typeof(Q));
            // exp = q => q.prop
            var exp = Expression.Property(param, prop);

            MethodInfo methodExpressionWhereKeysAre = typeof(GenericToolsExpressionTrees).GetMethod(
                                                                                                    "ExpressionWhereKeysAre", 
                                                                                                    BindingFlags.Public | BindingFlags.Static
                                                                                                    )
                                                                                         .MakeGenericMethod(typeof(T));
            // exp2 = t => t.Id == id OR exp2 = t => t.key1 == key1value && ... && t.keyn == keynvalue
            Expression exp2 = (Expression)methodExpressionWhereKeysAre.Invoke(
                                                                                typeof(GenericToolsExpressionTrees), 
                                                                                new object[] { 
                                                                                                objs 
                                                                                             }
                                                                              );

            MethodInfo methodWhere = typeof(Enumerable).GetMethods()
                                                       .Where(m => 
                                                              m.Name == "Where"
                                                              )
                                                       .ToList()[0]
                                                       .MakeGenericMethod(typeof(T));
            // body = q => q.prop.Where(exp2)
            Expression body = Expression.Call(methodWhere, exp, exp2);

            MethodInfo methodCount = typeof(Enumerable).GetMethods()
                                                       .Where(m => 
                                                              m.Name == "Count" && 
                                                              m.GetParameters().Length == 1)
                                                       .Single()
                                                       .MakeGenericMethod(typeof(T));
            // body = q => q.prop.Where(exp2).Count()
            body = Expression.Call(methodCount, body);

            // body = q => q.prop.Where(exp2).Count() == 1
            body = Expression.Equal(body, Expression.Constant(1));

            body = Expression.And(
                                    body,
                                    Expression.Equal(
                                                        Expression.Call(
                                                                            methodCount, 
                                                                            Expression.Property(param, prop)
                                                                        ), 
                                                        Expression.Constant(1)
                                                    )
                                 );
            return Expression.Lambda<Func<Q, bool>>(body, param);
        }

        /// <summary>
        /// Create an expression tree with principal root of type <typeparamref name="T"/> so that
        /// one of the keys is different than the given <paramref name="objs"/>
        /// <br/>
        /// Essentially gives <c>t => t.key1 != key1value || ... || t.keyn != keynvalue</c>
        /// </summary>
        /// <remarks>It is assumed that <paramref name="objs"/> are keys and not an Id. See <see cref="ExpressionListRemoveElementWithGivenId{T}(int?)"/>
        /// to see the other case.</remarks>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="objs">Either the Id or the Keys</param>
        /// <returns>The expression tree</returns>
        public static Expression<Func<T, bool>> ExpressionListRemoveElementWithGivenKeys<T>(params object[] objs)
        {
            var param = Expression.Parameter(typeof(T));
            Expression body = Expression.IsFalse(Expression.Equal(
                                                                    Expression.Property(
                                                                                            param, 
                                                                                            typeof(T).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[0])
                                                                                        ),
                                                                    Expression.Constant(
                                                                                            objs[0], 
                                                                                            typeof(T).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[0]).PropertyType
                                                                                        )
                                                                  ));
            for (int i = 1; i < objs.Length; i++)
            {
                body = Expression.Or(
                                        body,
                                        Expression.IsFalse(Expression.Equal(
                                                                                Expression.Property(param, typeof(T).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[i])),
                                                                                Expression.Constant(
                                                                                                        objs[i], 
                                                                                                        typeof(T).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<T>()[i]).PropertyType
                                                                                                    )
                                                                            ))
                                    );
            }
            // t => t.key1 != value1 || t.key2 != value2 ...
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        /// <summary>
        /// Create an expression tree with principal root of type <typeparamref name="T"/> so that the
        /// Id is different from the given <paramref name="id"/> 
        /// <br/>
        /// Essentially gives <c>t => t.Id != id</c>
        /// </summary>
        /// <remarks>It is assumed that the Id is given, not keys. See <see cref="ExpressionListRemoveElementWithGivenKeys{T}(object[])"/>
        /// to see the other case.</remarks>
        /// <typeparam name="T">The type invistigated</typeparam>
        /// <param name="objs">Either the Id or the Keys</param>
        /// <returns>The expression tree</returns>
        public static Expression<Func<T, bool>> ExpressionListRemoveElementWithGivenId<T>(int? id)
        {
            var param = Expression.Parameter(typeof(T));
            var body = Expression.IsFalse(Expression.Equal(
                                                            Expression.Property(
                                                                                    param, 
                                                                                    typeof(T).GetProperty("Id")
                                                                                ),
                                                            Expression.Constant(
                                                                                    id, 
                                                                                    typeof(int?)
                                                                                )
                                                           ));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        /// <summary>
        /// Get an expression tree having root of type <typeparamref name="Q"/>. These elements have properties <paramref name="prop"/>
        /// which is a list of elements of type <typeparamref name="T"/>. 
        /// <br/>
        /// This returns essentially : <c>q => q.prop1.Where(t => t.keysorId == objs).Count() >= 1 &amp;&amp; ... &amp;&amp; q.propn.Where(t => t.keysorId == objs).Count() >= 1</c>
        /// <br/>
        /// That is to say, for an element of type <typeparamref name="Q"/> so that their properties <paramref name="propnames"/> is
        /// a <see cref="IList{T}"/>, whether or not all of them contains an element with either Id or Key given by <paramref name="objs"/>.
        /// </summary>
        /// <typeparam name="T">The most nested type</typeparam>
        /// <typeparam name="Q">The type of the expression tree root</typeparam>
        /// <param name="propnames">The properties names of <typeparamref name="Q"/> in question</param>
        /// <param name="objs">Either the Id or the Key</param>
        /// <returns>The expression tree.</returns>
        public static Expression<Func<Q, bool>> ExpressionListWhereMultiplePropListCountainsElementWithGivenKeys<T, Q>(List<string> propnames, params object[] objs)
        {
            var param = Expression.Parameter(typeof(Q));
            // exp = q => q.prop0
            var exp = Expression.Property(
                                            param, 
                                            typeof(Q).GetProperty(propnames[0])
                                          );

            MethodInfo methodExpressionWhereKeysAre = typeof(GenericToolsExpressionTrees).GetMethod(
                                                                                                    "ExpressionWhereKeysAre", 
                                                                                                    BindingFlags.Public | BindingFlags.Static
                                                                                                    )
                                                                                         .MakeGenericMethod(typeof(T));
            // exp2 = t => t.Id == id OR exp2 = t => t.key1 == key1value && ... && t.keyn == keynvalue
            Expression exp2 = (Expression)methodExpressionWhereKeysAre.Invoke(
                                                                                typeof(GenericToolsExpressionTrees), 
                                                                                new object[] { 
                                                                                                objs 
                                                                                             }
                                                                             );

            MethodInfo methodWhere = typeof(Enumerable).GetMethods()
                                                       .Where(m => 
                                                              m.Name == "Where"
                                                              )
                                                       .ToList()[0]
                                                       .MakeGenericMethod(typeof(T));
            // body = q => q.prop0.Where(exp2)
            Expression body = Expression.Call(methodWhere, exp, exp2);

            MethodInfo methodCount = typeof(Enumerable).GetMethods()
                                                       .Where(m => 
                                                              m.Name == "Count" && 
                                                              m.GetParameters().Length == 1)
                                                       .Single()
                                                       .MakeGenericMethod(typeof(T));
            // body = q => q.prop0.Where(exp2).Count()
            body = Expression.Call(methodCount, body);

            // body = q => q.prop0.Where(exp2).Count() >= 1
            body = Expression.GreaterThanOrEqual(body, Expression.Constant(1));

            for (int i = 1; i < propnames.Count; i++)
            {
                var exp3 = Expression.Property(param, typeof(Q).GetProperty(propnames[i]));
                Expression exp4 = (Expression)methodExpressionWhereKeysAre.Invoke(
                                                                                    typeof(GenericToolsExpressionTrees), 
                                                                                    new object[] { 
                                                                                                    objs 
                                                                                                 }
                                                                                 );
                Expression exp5 = Expression.Call(methodWhere, exp3, exp4);
                Expression exp6 = Expression.Call(methodCount, exp5);
                Expression exp7 = Expression.GreaterThanOrEqual(exp6, Expression.Constant(1));
                body = Expression.AndAlso(body, exp7);

            }
            return Expression.Lambda<Func<Q, bool>>(body, param);
        }

        /// <summary>
        /// Get an expression tree having principal root of type <typeparamref name="Q"/>, for testing whether or not an element
        /// <paramref name="newItem"/> of type <typeparamref name="T"/> with property <paramref name="tPropName"/> of type <see cref="IList{Q}"/>
        /// does not contain the element tested.
        /// <br/>
        /// Essentially, does <c>q => !newitem.tpropname.Where(qq => qq.Id == q.Id).Count() == 1)</c>
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="newItem"/></typeparam>
        /// <typeparam name="Q">The type of the elements invistigated</typeparam>
        /// <param name="newItem">The object</param>
        /// <param name="tPropName">The name of the property of <typeparamref name="T"/> in question</param>
        /// <param name="nbr">The number of keys for objects of type <typeparamref name="Q"/> if appropriate</param>
        /// <returns>The expression tree</returns>
        public static Expression<Func<Q, bool>> ExpressionListWhereOtherTypePropListNotContains<T, Q>(T newItem, string tPropName, int nbr = 1)
        {
            var param = Expression.Parameter(typeof(Q));
            var newItemcst = Expression.Constant(newItem, typeof(T));
            Expression body = Expression.Property(
                                                    newItemcst, 
                                                    typeof(T).GetProperty(tPropName)
                                                 );

            var param2 = Expression.Parameter(typeof(Q));
            Expression exp2;
            if (typeof(BaseEntity).IsAssignableFrom(typeof(Q)))
            {
                exp2 = Expression.Equal(
                                            Expression.Property(
                                                                param2, 
                                                                typeof(Q).GetProperty("Id")
                                                                ),
                                            Expression.Property(
                                                                param, 
                                                                typeof(Q).GetProperty("Id")
                                                                )
                                        );
            }
            else
            {
                exp2 = Expression.Equal(
                                            Expression.Property(
                                                                    param2, 
                                                                    typeof(Q).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<Q>()[0])
                                                                ),
                                            Expression.Property(
                                                                    param, 
                                                                    typeof(Q).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<Q>()[0])
                                                                )
                                        );
                for (int i = 1; i < nbr; i++)
                {
                    exp2 = Expression.And(
                                            exp2,
                                            Expression.Equal(
                                                                Expression.Property(
                                                                                        param2, 
                                                                                        typeof(Q).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<Q>()[i])
                                                                                    ),
                                                                Expression.Property(
                                                                                        param, 
                                                                                        typeof(Q).GetProperty(GenericToolsTypeAnalysis.KeyPropertiesNames<Q>()[i])
                                                                                    )
                                                             )
                                          );
                }
            }

            MethodInfo methodWhere = typeof(Enumerable).GetMethods()
                                                       .Where(m => 
                                                              m.Name == "Where"
                                                              )
                                                       .ToList()[0]
                                                       .MakeGenericMethod(typeof(Q));


            body = Expression.Call(
                                    methodWhere, 
                                    body, 
                                    Expression.Lambda<Func<Q, bool>>(exp2, param2)
                                  );

            MethodInfo methodCount = typeof(Enumerable).GetMethods()
                                                       .Where(m => 
                                                              m.Name == "Count" && 
                                                              m.GetParameters().Length == 1
                                                              )
                                                       .Single()
                                                       .MakeGenericMethod(typeof(Q));

            body = Expression.Call(methodCount, body);

            body = Expression.Equal(body, Expression.Constant(1));

            body = Expression.Not(body);

            return Expression.Lambda<Func<Q, bool>>(body, param);
        }
    }
}