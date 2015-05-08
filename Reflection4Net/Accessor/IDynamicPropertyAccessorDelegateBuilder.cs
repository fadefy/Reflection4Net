using System;

namespace Reflection4Net.Accessor
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDynamicPropertyAccessorDelegateBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Func<T, string, object> BuildPropertyGetter<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Func<T, string, object, bool> BuildPropertySetter<T>();
    }
}
