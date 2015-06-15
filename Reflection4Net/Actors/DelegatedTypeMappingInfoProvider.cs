using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Reflection4Net.Extensions;

namespace Reflection4Net.Actors
{
    public class DelegatedTypeMappingInfoProvider : ITypeMappingInfoProvider
    {
        protected Func<Type, IEnumerable<MemberInfo>> _memberProvider;

        public DelegatedTypeMappingInfoProvider(Func<Type, IEnumerable<MemberInfo>> memberProvider)
        {
            ArgumentsGuards.NotNull(() => memberProvider);

            _memberProvider = memberProvider;
        }

        public IEnumerable<MemberInfo> GetOperandMembers(Type sourceType, Type targetType)
        {
            ArgumentsGuards.NotNull(() => sourceType);
            ArgumentsGuards.NotNull(() => targetType);

            if (sourceType != targetType)
                throw new NotSupportedException("sourceType and targetType should be the same for this implementation.");

            return _memberProvider(sourceType);
        }

        public Func<MemberInfo, string> GetMemberNameMapping(Type sourceType, Type targetType)
        {
            return m => m.Name;
        }

        public Func<MemberInfo, TypeConverter> GetTypeConverters(Type sourceType, Type targetType)
        {
            return m => TypeDescriptor.GetConverter(m.MemberType);
        }
    }
}
