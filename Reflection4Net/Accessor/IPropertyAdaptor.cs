using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Reflection4Net.Accessor
{
    public interface IPropertyAdaptor
    {
        IEnumerable<PropertyDescriptor> GetProperties<T>(string propertyName);
    }
}
