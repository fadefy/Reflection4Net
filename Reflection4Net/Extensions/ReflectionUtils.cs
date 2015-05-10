using System;
using System.Collections.Generic;
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
    }
}
