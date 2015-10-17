using System;
using System.ComponentModel;

namespace Reflection4Net.Conversion
{
    public class TypeDescriptorConverter : ITypeConverter
    {
        public object Convert(object source, Type targetType)
        {
            var converter = TypeDescriptor.GetConverter(targetType);
            if (converter.CanConvertFrom(source.GetType()))
            {
                return converter.ConvertFrom(source);
            }
            else
            {
                return null;
            }
        }

        public T Convert<T, S>(S source)
        {
            return (T)Convert(source, typeof(T));
        }
    }
}
