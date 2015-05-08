using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Reflection4Net.Accessor
{
    /// <summary>
    /// 
    /// </summary>
    public class DynamicSwitchPropertyAccessorDelegateBuilder : IDynamicPropertyAccessorDelegateBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Func<T, string, object> BuildPropertyGetter<T>()
        {
            var type = typeof(T);
            var instance = Expression.Parameter(type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            // var nameHash = memberName.GetHashCode();
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));

            // switch (memberName.GetHashCode()) {
            var cases = new List<SwitchCase>();
            // case: class.Name.GetHashCode():
            //      return instance;
            cases.Add(Expression.SwitchCase(Expression.Convert(instance, typeof(object)),
                GetClassNamesHashes(type).Select(h => Expression.Constant(h, typeof(int)))));
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(GetPropertyOrder))
            {
                var propertyPairs = GetPropertyNamesHashes(propertyInfo).ToList();
                if (propertyPairs.Select(p => p.Key).Distinct().Count() < propertyPairs.Count())
                    throw new InvalidProgramException("Duplicated Property Name detected.");

                foreach (var propertyPair in propertyPairs)
                {
                    // Build the expression to fetch property value from complex type by a given property path.
                    var property = BuildPropertyExpression(instance, propertyInfo, propertyPair);
                    var catchNullReference = Expression.Catch(typeof(NullReferenceException), Expression.Constant(null, typeof(object)));
                    var tryRead = Expression.TryCatch(Expression.Convert(property, typeof(object)), catchNullReference);
                    // case property.Name.GetHashCode():
                    //    try {
                    //        return property[.Path][.Path] as object;
                    //    // return null when any property in the path is null.
                    //    } catch(NullReferenceException ex) {
                    //        return null;
                    //    }
                    cases.Add(Expression.SwitchCase(tryRead, Expression.Constant(propertyPair.Key, typeof(int))));
                }
            }
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            Debug.WriteLine(String.Format("Generate Accessor Method for class {0} defined {1} properties.", type.FullName, cases.Count));

            return Expression.Lambda<Func<T, string, object>>(methodBody, instance, memberName).Compile();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Func<T, string, object, bool> BuildPropertySetter<T>()
        {
            var type = typeof(T);
            var instance = Expression.Parameter(type, "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var value = Expression.Parameter(typeof(object), "value");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>
                            {
                // Add a default case that do nothing.
                // In case of the class contains no writable properties.
                Expression.SwitchCase(Expression.Constant(false, typeof(bool)), Expression.Constant(-1, typeof(Int32))),
            };
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(GetPropertyOrder))
            {
                // Property must be writeable.
                if (!propertyInfo.CanWrite)
                    continue;

                var propertyPairs = GetPropertyNamesHashes(propertyInfo).ToList();
                if (propertyPairs.Select(p => p.Key).Distinct().Count() < propertyPairs.Count())
                    throw new InvalidProgramException("Duplicated Property Name detected.");

                foreach (var propertyPair in propertyPairs)
                {
                    var property = BuildPropertyExpression(instance, propertyInfo, propertyPair);
                    var catchNullReference = Expression.Catch(typeof(NullReferenceException), Expression.Constant(false, typeof(bool)));
                    var tryRead = Expression.TryCatch(Expression.Block(typeof(bool), Expression.Assign(property, Expression.Convert(value, property.Type)), Expression.Constant(true, typeof(bool))), catchNullReference);
                    // case property.Name.GetHashCode():
                    //    try {
                    //        property[.Path][.Path] = (P)value;
                    //        return true;
                    //    // return null when any property in the path is null.
                    //    } catch(NullReferenceException ex) {
                    //        return false;
                    //    }
                    cases.Add(Expression.SwitchCase(tryRead, Expression.Constant(propertyPair.Key, typeof(int))));
                }
            }

            var switchEx = Expression.Switch(nameHash, Expression.Constant(false, typeof(bool)), cases.ToArray());
            var methodBody = Expression.Block(typeof(bool), new[] { nameHash }, calHash, switchEx);

            Debug.WriteLine(String.Format("Generate Accessor Method for class {0} defined {1} properties.", type.FullName, cases.Count));

            return Expression.Lambda<Func<T, string, object, bool>>(methodBody, instance, memberName, value).Compile();
        }

        private static MemberExpression BuildPropertyExpression(Expression instance, MemberInfo propertyInfo, KeyValuePair<int, string> propertyPair)
        {
            var property = Expression.Property(instance, propertyInfo.Name);
            if (!String.IsNullOrEmpty(propertyPair.Value))
            {
                property = propertyPair.Value.Split('.').Aggregate(property, (p, path) => Expression.Property(p, path));
            }

            return property;
        }

        private static IEnumerable<KeyValuePair<int, string>> GetPropertyNamesHashes(PropertyInfo propertyInfo)
        {
            TypeDescriptor.GetProperties(propertyInfo.ReflectedType);
            var isIgnored = propertyInfo.GetCustomAttributes(typeof(AdaptedNameIgnoreAttribute), true).Any();
            if (!isIgnored)
            {
                var attributes = propertyInfo.GetCustomAttributes(typeof(AdaptedNameAttribute), true);
                // If an AdaptedNameAttribute is defined for a property. Then only the adaptedname could be used.
                // Otherwise, the adapted name may conflict with another existing property. For example
                // public class Dummy
                // {
                //     [AdaptedName("Value")]
                //     public Name { get; set }
                //     [AdaptedName("Name")]
                //     public Value { get; set }
                // }
                // The correct behavior is just switch the value of Name and Value property when reading them.
                // So, the original property name must be ignored when the AdaptedName attribute applied.
                if (attributes.Any())
                {
                    foreach (AdaptedNameAttribute attribute in attributes)
                    {
                        yield return new KeyValuePair<int, string>(attribute.Name.GetHashCode(), attribute.PropertyPath);
                    }
                }
                else
                {
                    yield return new KeyValuePair<int, string>(propertyInfo.Name.GetHashCode(), null);
                }
            }
        }

        private static int GetPropertyOrder(PropertyInfo property)
        {
            var attributes = property.GetCustomAttributes(typeof(AdaptedNameAttribute), true).Cast<AdaptedNameAttribute>().ToList();

            if (attributes.Any())
            {
                return attributes.Min(a => a.Order);
            }

            return Int32.MaxValue;
        }

        private static IEnumerable<int> GetClassNamesHashes(Type type)
        {
            yield return type.Name.GetHashCode();
            foreach (AdaptedNameAttribute attribute in type.GetCustomAttributes(typeof(AdaptedNameAttribute), true))
            {
                yield return attribute.Name.GetHashCode();
            }
        }
    }
}
