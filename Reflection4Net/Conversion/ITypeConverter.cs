using System;

namespace Reflection4Net.Conversion
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface ITypeConverter
    {
        T Convert<T, S>(S source);

        object Convert(object source, Type targetType);
    }
}
