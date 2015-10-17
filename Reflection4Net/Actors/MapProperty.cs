using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using Reflection4Net.Extensions;

namespace Reflection4Net.Actors
{
    public class MapProperty<T> : AbstractOperation<Action<IDictionary<string, object>, T>>
    {
        public override Action<IDictionary<string, object>, T> Build(ITypeMappingInfoProvider infoProvider)
        {
            var obj = typeof(T).Parameter("obj");
            var map = typeof(IDictionary<string, object>).Parameter("map");

            var guardOfObj = obj.EqualTo((object)null).Then(new ArgumentNullException("obj can't be null.").Throw());
            var guardOfMap = map.EqualTo((object)null).Then(new ArgumentNullException("map can't be null.").Throw());

            var assignments = from member in infoProvider.GetOperandMembers(typeof(T), typeof(T))
                              let memberName = infoProvider.GetMemberNameMapping(typeof(T), typeof(T))(member)
                              let valueConverter = infoProvider.GetTypeConverters(typeof(T), typeof(T))(member)
                              let valueExpr = ReflectionUtils.GetDictionaryValueMethod.CallWith(map, Expression.Constant(memberName))
                              let sourceValue = ReflectionUtils.ConvertFromMethod.CallOn(Expression.Constant(valueConverter), valueExpr)
                              let target = obj.GetPropertyOrField(memberName)
                              select target.AssignFrom(sourceValue.ConvertToType(member.GetMemberType()));

            var body = guardOfMap.Concat(guardOfObj).Concat(assignments).AsABlock();
            Trace.Write(String.Format("Generated CopyTo method for type {0}: {1}", typeof(T), body));

            return body.CompileTo<Action<IDictionary<string, object>, T>>(map, obj);
        }


    }
}
