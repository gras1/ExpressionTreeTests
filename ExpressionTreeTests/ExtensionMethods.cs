using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionTreeTests
{
    public static class ExpressionOperator
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source">Root Object - it must be a reference type or a sub class of IEnumerable</param>
        /// <param name="expression">Labmda expression to set the property value returned</param>
        /// <param name="defaultValue">The default value in the case the property is not reachable </param>
        /// <returns></returns>
        public static TResult NullSafeGetValue<TSource, TResult>(this TSource source, Expression<Func<TSource, TResult>> expression, TResult defaultValue)
        {
            var value = GetValue(expression, source);
            return value == null ? defaultValue : (TResult)value;
        }

        public static LambdaExpression CreateExpression(Type type, string propertyName)
        {
            var param = Expression.Parameter(type, "x");
            Expression body = param;
            foreach (var member in propertyName.Split('.'))
            {
                if (member.EndsWith("]"))
                {
                    // ReSharper disable StringIndexOfIsCultureSpecific.1
                    var index = Convert.ToInt32(member.Substring(member.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    var nonArrayMember = member.Substring(0, member.IndexOf("["));
                    PropertyInfo info = type.GetProperty(nonArrayMember);
                    if (info == null)
                    {
                        //go up the expression tree to set type correctly
                        type = body.Type;
                        info = type.GetProperty(nonArrayMember);
                    }
                    // ReSharper restore StringIndexOfIsCultureSpecific.1

                    body = Expression.Property(body, nonArrayMember);
                    body = Expression.Call(body, info.PropertyType.GetMethod("get_Item"), new Expression[] { Expression.Constant(index) });
                }
                else
                {
                    body = Expression.PropertyOrField(body, member);
                }
            }
            return Expression.Lambda(body, param);
        }

        private static string GetFullPropertyPathName<TSource, TResult>(Expression<Func<TSource, TResult>> expression)
        {
            return expression.Body.ToString().Replace(expression.Parameters[0] + ".", string.Empty);
        }

        private static object GetValue<TSource, TResult>(Expression<Func<TSource, TResult>> expression, TSource source)
        {
            string fullPropertyPathName = GetFullPropertyPathName(expression);
            return GetNestedPropertyValue(fullPropertyPathName, source);
        }

        private static object GetNestedPropertyValue(string name, object obj)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null)
                {
                    return null;
                }
                var type = obj.GetType();
                if (obj is IEnumerable)
                {
                    type = (obj as IEnumerable).GetType();
                    var methodInfo = type.GetMethod("get_Item");
                    var index = int.Parse(part.Split('(')[1].Replace(")", string.Empty));
                    try
                    {
                        obj = methodInfo.Invoke(obj, new object[] { index });
                    }
                    catch (Exception)
                    {
                        obj = null;
                    }
                }
                else
                {
                    PropertyInfo info = type.GetProperty(part);
                    if (info == null)
                    {
                        return null;
                    }
                    obj = info.GetValue(obj, null);
                }
            }
            return obj;
        }
    }
}
