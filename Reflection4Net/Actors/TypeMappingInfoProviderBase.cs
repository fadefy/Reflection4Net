using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Reflection4Net.Extensions;

namespace Reflection4Net.Actors
{
    public abstract class TypeMappingInfoProviderBase : ITypeMappingInfoProvider
    {
        public abstract IEnumerable<MemberInfo> GetOperandMembers(Type sourceType, Type targetType);

        public virtual Func<MemberInfo, string> GetMemberNameMapping(Type sourceType, Type targetType)
        {
            return m => m.Name;
        }

        public virtual Func<MemberInfo, TypeConverter> GetTypeConverters(Type sourceType, Type targetType)
        {
            return m => TypeDescriptor.GetConverter(m.GetMemberType());
        }
    }
}
