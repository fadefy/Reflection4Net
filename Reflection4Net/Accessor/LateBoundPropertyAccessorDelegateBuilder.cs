using Reflection4Net.Cache;
using System;
using System.Reflection;
using Reflection4Net.Extensions;

namespace Reflection4Net.Accessor
{
    /// <summary>
    /// 
    /// </summary>
    public class LateBoundPropertyAccessorDelegateBuilder : IDynamicPropertyAccessorDelegateBuilder
    {
        private class AccessorDelegateCache<T>
        {
            internal static ICache<string, Func<T, object>> GetterCache = new DictionaryCache<string, Func<T, object>>();
            internal static ICache<string, Action<T, object>> SetterCache = new DictionaryCache<string, Action<T, object>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Func<T, string, object> BuildPropertyGetter<T>()
        {
            return GetPropertyValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Func<T, string, object, bool> BuildPropertySetter<T>()
        {
            return SetPropertyValue;
        }

        private object GetPropertyValue<T>(T instance, string propertyName)
        {
            Func<T, object> propertyAccessor = null;
            if (AccessorDelegateCache<T>.GetterCache.TryGetValue(propertyName, out propertyAccessor))
            {
                return propertyAccessor(instance);
            }
            else
            {
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                if (propertyInfo != null && propertyInfo.CanRead)
                {
                    propertyAccessor = (Func<T, object>)Delegate.CreateDelegate(typeof(Func<T, object>), propertyInfo.GetGetMethod());
                    AccessorDelegateCache<T>.GetterCache.Cache(propertyName, propertyAccessor);

                    return propertyAccessor(instance);
                }
                else
                {
                    return null;
                }
            }
        }

        private bool SetPropertyValue<T>(T instance, string propertyName, object newValue)
        {
            Action<T, object> propertySetter = null;
            if (AccessorDelegateCache<T>.SetterCache.TryGetValue(propertyName, out propertySetter))
            {
                propertySetter(instance, newValue);

                return true;
            }
            else
            {
                var propertyInfo = typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
                if (propertyInfo == null)
                    return false;

                if (propertyInfo.CanWrite)
                {
                    var delegateType = typeof(Action<,>).MakeGenericType(typeof(T), propertyInfo.PropertyType);
                    propertySetter = Delegate.CreateDelegate(delegateType, propertyInfo.GetSetMethod()).CastToGenericAction<T, object>();
                    AccessorDelegateCache<T>.SetterCache.Cache(propertyName, propertySetter);
                    propertySetter(instance, newValue);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
