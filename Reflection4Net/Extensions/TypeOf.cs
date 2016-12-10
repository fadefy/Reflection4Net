using System;
using Reflection4Net.Accessor;
using Reflection4Net.Actors;

namespace Reflection4Net.Extensions
{
    public class TypeOf<T>
    {
        public IPropertyAccessor<T, R> GetAccessor<R>(string propertyName)
        {
            return null;
        }

        static TypeOf()
        {
            var actor = new CopyTo<T>();
            CopyTo = actor.Build(DelegatedTypeMappingInfoProvider.AllInstanceWritableProperties);
        }

        public static Action<T, T> CopyTo { get; private set; }
    }
}
