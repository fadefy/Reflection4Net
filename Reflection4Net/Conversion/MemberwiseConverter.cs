using System;
using System.ComponentModel;

namespace Reflection4Net.Conversion
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class MemberwiseConverter : ITypeConverter
    {
        public T Convert<T, S>(S source)
        {
            throw new NotImplementedException();
        }

        public object Convert(object source, Type targetType)
        {
            throw new NotImplementedException();
        }
    }
}
