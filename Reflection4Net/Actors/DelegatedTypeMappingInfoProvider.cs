using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflection4Net.Extensions;

namespace Reflection4Net.Actors
{
    public class DelegatedTypeMappingInfoProvider : TypeMappingInfoProviderBase
    {
        private static ITypeMappingInfoProvider allProperties = new DelegatedTypeMappingInfoProvider(t => t.GetProperties());
        private static ITypeMappingInfoProvider allInstanceProperties = new DelegatedTypeMappingInfoProvider(t => t.GetProperties(BindingFlags.Instance));
        private static ITypeMappingInfoProvider allInstanceWritablePropertries = new DelegatedTypeMappingInfoProvider(t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite));

        protected Func<Type, IEnumerable<MemberInfo>> _memberProvider;

        public DelegatedTypeMappingInfoProvider(Func<Type, IEnumerable<MemberInfo>> memberProvider)
        {
            ArgumentsGuards.NotNull(() => memberProvider);

            _memberProvider = memberProvider;
        }

        public override IEnumerable<MemberInfo> GetOperandMembers(Type sourceType, Type targetType)
        {
            ArgumentsGuards.NotNull(() => sourceType);
            ArgumentsGuards.NotNull(() => targetType);

            return _memberProvider(sourceType);
        }

        public static ITypeMappingInfoProvider AllProperties
        {
            get { return allProperties; }
        }

        public static ITypeMappingInfoProvider AllInstanceProperties
        {
            get { return allInstanceProperties; }
        }

        public static ITypeMappingInfoProvider AllInstanceWritableProperties
        {
            get { return allInstanceWritablePropertries; }
        }
    }
}
