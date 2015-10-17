using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Reflection4Net.Extensions
{
    public static class ReflectionUtils
    {
        private static MethodInfo _getDictionaryValue;
        private static MethodInfo _convertFrom;

        static ReflectionUtils()
        {
            _getDictionaryValue = typeof(ReflectionUtils).GetMethod("GetDictionaryValue", BindingFlags.Static | BindingFlags.Public);
            _convertFrom = typeof(TypeConverter).GetMethod("ConvertFrom", new[] { typeof(object) });
        }

        public static MethodInfo GetDictionaryValueMethod
        {
            get { return _getDictionaryValue; }
        }

        public static MethodInfo ConvertFromMethod
        {
            get { return _convertFrom; }
        }

        public static IDictionary<string, string> GetPropertyMapBy<T>(this Type type, Func<T, string> nameExtractor)
            where T : Attribute
        {
            Func<PropertyInfo, string> getMappedName = p =>
            {
                var attribute = p.TryFindSingleAttribute<T>();
                return attribute == null ? p.Name : nameExtractor(attribute);
            };
            return type.GetProperties().ToDictionary(p => p.Name, getMappedName);
        }

        public static T TryFindSingleAttribute<T>(this MemberInfo info)
            where T : Attribute
        {
            return info.GetCustomAttributes(typeof(T), true).SingleOrDefault() as T;
        }

        [Pure]
        public static IEnumerable<FieldInfo> GetFieldsWithOut<T>(this Type type)
            where T : Attribute
        {
            return from field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                   let attribute = field.GetCustomAttributes(typeof(T), false)
                   where !attribute.Any()
                   select field;
        }

        public static string GetMemberName(this Expression<Func<object>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null && expression.Body is UnaryExpression)
            {
                memberExpression = (expression.Body as UnaryExpression).Operand as MemberExpression;
            }
            if (memberExpression == null)
                throw new ArgumentException("expression is not a valid member expression", "expression");

            return memberExpression.Member.Name;
        }

        public static object GetMemberValue(this Expression<Func<object>> expression)
        {
            return expression.CompileTo<Func<object>>()();
        }

        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo)
            {
                return (memberInfo as PropertyInfo).PropertyType;
            }
            else if (memberInfo is FieldInfo)
            {
                return (memberInfo as FieldInfo).FieldType;
            }
            throw new InvalidOperationException(String.Format("Member {0} doesn't have a return type.", memberInfo));
        }

        public static object GetDictionaryValue(IDictionary<string, object> map, string name)
        {
            object value;
            if (map.TryGetValue(name, out value))
                return value;
            return null;
        }
    }
}
