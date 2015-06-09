using System;
using System.ComponentModel.Composition;

namespace Reflection4Net.Accessor
{
    /// <summary>
    /// Provides the capability to get property value from an object by a given name.
    /// </summary>
    public class AdaptedPropertyAccessor<T> : IDynamicPropertyAccessor
    {
        private IDynamicPropertyAccessorDelegateBuilder delegateBuilder = new DynamicSwitchPropertyAccessorDelegateBuilder();
        private static Lazy<Func<T, string, object>> memberGetter;
        private static Lazy<Func<T, string, object, bool>> memberSetter;

        static AdaptedPropertyAccessor()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delegateBuilder"></param>
        public AdaptedPropertyAccessor()
        {
            InitializeMemberAccessor();
        }

        /// <summary>
        /// 
        /// </summary>
        public IDynamicPropertyAccessorDelegateBuilder DelegateBuilder
        {
            get { return delegateBuilder; }
            set
            {
                delegateBuilder = value;
                memberGetter = null;
                memberSetter = null;
                InitializeMemberAccessor();
            }
        }

        /// <summary>
        /// Gets property value from an object by an given propertyName.
        /// </summary>
        /// <param name="instance">The object to get value from.</param>
        /// <param name="propertyName">The property name by which to get value from.</param>
        /// <returns>Property value fetched from the given instance.</returns>
        public static object GetProperty(T instance, string propertyName)
        {
            return memberGetter.Value(instance, propertyName);
        }

        /// <summary>
        /// Gets property value from an object by an given propertyName.
        /// </summary>
        /// <param name="instance">The object to get value from.</param>
        /// <param name="propertyName">The property name by which to get value from.</param>
        /// <returns>Property value fetched from the given instance.</returns>
        public object GetProperty(object instance, string propertyName)
        {
            return GetProperty((T)instance, propertyName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetProperty(T instance, string propertyName, object value)
        {
            return memberSetter.Value(instance, propertyName, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetProperty(object instance, string propertyName, object value)
        {
            return SetProperty((T)instance, propertyName, value);
        }

        private void InitializeMemberAccessor()
        {
            if (memberGetter == null)
                memberGetter = new Lazy<Func<T, string, object>>(DelegateBuilder.BuildPropertyGetter<T>);
            if (memberSetter == null)
                memberSetter = new Lazy<Func<T, string, object, bool>>(DelegateBuilder.BuildPropertySetter<T>);
        }
    }
}
