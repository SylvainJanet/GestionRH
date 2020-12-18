using GenericRepositoryAndService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GenericRepositoryAndService.Tools.Generic
{
    public abstract class GenericToolsExpressionTrees
    {
        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.PropertyEquals``1(System.Reflection.PropertyInfo,System.Object)"]/*'/>
        public static Expression<Func<TItem, bool>> PropertyEquals<TItem>(PropertyInfo property, object value)
        {
            var param = Expression.Parameter(typeof(TItem));
            var body = Expression.Equal(
                                            Expression.Property(param, property),
                                            Expression.Constant(value, property.PropertyType)
                                        );
            return Expression.Lambda<Func<TItem, bool>>(body, param);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.GetKey``2(System.Reflection.PropertyInfo)"]/*'/>
        public static Expression<Func<TItem, TKey>> GetKey<TItem, TKey>(PropertyInfo property)
        {
            var param = Expression.Parameter(typeof(TItem));
            var body = Expression.Property(param, property);
            var bodyconverted = Expression.Convert(body, typeof(TKey));
            return Expression.Lambda<Func<TItem, TKey>>(bodyconverted, param);
        }

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.ExpressionPropPropEquals``1(System.Reflection.PropertyInfo,System.Reflection.PropertyInfo,System.Object)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.PropertyKeysNotNull``1(System.Reflection.PropertyInfo)"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.ExpressionWhereKeysAre``1(System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.ExpressionListWherePropListCountainsElementWithGivenKeys``2(System.Reflection.PropertyInfo,System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.ExpressionListWherePropListCountainsOnlyElementWithGivenKeys``2(System.Reflection.PropertyInfo,System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.ExpressionListRemoveElementWithGivenKeys``1(System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.ExpressionListRemoveElementWithGivenId``1(System.Nullable{System.Int32})"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.ExpressionListWhereMultiplePropListCountainsElementWithGivenKeys``2(System.Collections.Generic.List{System.String},System.Object[])"]/*'/>
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

        ///<include file='docs.xml' path='doc/members/member[@name="M:GenericRepositoryAndService.Tools.Generic.GenericToolsExpressionTrees.ExpressionListWhereOtherTypePropListNotContains``2(``0,System.String,System.Int32)"]/*'/>
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