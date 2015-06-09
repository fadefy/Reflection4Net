using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Reflection4Net.Extensions
{
    public static class ReflectionUtils
    {
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
    }
}
