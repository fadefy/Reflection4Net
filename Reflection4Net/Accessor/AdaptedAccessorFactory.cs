using Reflection4Net.Cache;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Reflection4Net.Accessor
{
    /// <summary>
    /// Naming AdapterFactory helps to create and cache naming adapter instances. 
    /// For the same object type, the factory will returns the same instance of AdaptedPropertyAccessor from cache.
    /// </summary>
    [Export("Naming", typeof(IAccessorFactory))]
    public class AdaptedAccessorFactory : IAccessorFactory
    {
        /// <summary>
        /// 
        /// </summary>
        private static IAccessorFactory instance = new AdaptedAccessorFactory();

        /// <summary>
        /// Use RuntimeTypeHandle instead of Type to reduce heap memory usage.
        /// </summary>
        private static ICache<RuntimeTypeHandle, IPropertyAccessor> propertyAdapters = new DictionaryCache<RuntimeTypeHandle, IPropertyAccessor>();

        private AdaptedAccessorFactory() {  }

        /// <summary>
        /// Gets the instance of AdaptedAccessorFactory.
        /// </summary>
        public static IAccessorFactory Instance { get { return instance; } }

        /// <summary>
        /// 
        /// </summary>
        public static ICache<RuntimeTypeHandle, IPropertyAccessor> PropertyAccessorCache
        {

            get { return propertyAdapters; }
            set
            {
                if (propertyAdapters != null)
                {
                    foreach (var pair in propertyAdapters)
                    {
                        value.Cache(pair.Key, pair.Value);
                    }

                    propertyAdapters.Clear();
                }

                propertyAdapters = value;
            }
        }

        /// <summary>
        /// Gets an IPropertyAccessor by an object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>An instance of IPropertyAccessor.</returns>
        public IPropertyAccessor GetAccessor(object value)
        {
            return GetPropertyAccessor(value.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IPropertyAccessor GetAccessor<T>()
        {
            var type = typeof(T);
            IPropertyAccessor adapter;
            lock (propertyAdapters)
            {
                if (!propertyAdapters.TryGetValue(type.TypeHandle, out adapter))
                {
                    adapter = InitializeAccessor<T>();
                }
            }

            return adapter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IPropertyAccessor GetPropertyAccessor(Type type)
        {
            IPropertyAccessor adapter;
            lock (propertyAdapters)
            {
                if (!propertyAdapters.TryGetValue(type.TypeHandle, out adapter))
                {
                    adapter = InitializeAccessor(type);
                }
            }

            return adapter;
        }

        private static IPropertyAccessor InitializeAccessor(Type type)
        {
            var adapter = Activator.CreateInstance(typeof(AdaptedPropertyAccessor<>).MakeGenericType(type)) as IPropertyAccessor;
            propertyAdapters.Cache(type.TypeHandle, adapter);

            return adapter;
        }

        private static IPropertyAccessor InitializeAccessor<T>()
        {
            var adapter = new AdaptedPropertyAccessor<T>();
            propertyAdapters.Cache(typeof(T).TypeHandle, adapter);

            return adapter;
        }
    }
}
