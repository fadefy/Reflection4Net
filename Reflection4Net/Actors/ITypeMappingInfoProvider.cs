using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Reflection4Net.Actors
{
    public interface ITypeMappingInfoProvider
    {
        IEnumerable<MemberInfo> GetOperandMembers(Type sourceType, Type targetType);

        Func<MemberInfo, string> GetMemberNameMapping(Type sourceType, Type targetType);

        Func<MemberInfo, TypeConverter> GetTypeConverters(Type sourceType, Type targetType);
    }
}
