using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Reflection4Net.Extensions;

namespace Reflection4Net.Actors
{
    public class CopyTo<T> : AbstractOperation<Action<T, T>>
    {
        public override Action<T, T> Build(ITypeMappingInfoProvider infoProvider)
        {
            var x = typeof(T).Parameter("source");
            var y = typeof(T).Parameter("target");

            var guardOfX = x.EqualTo((object)null).Then(new InvalidOperationException("source cannot be null.").Throw());
            var guardOfY = y.EqualTo((object)null).Then(new InvalidOperationException("target cannot be null.").Throw());

            var assignments = from member in infoProvider.GetOperandMembers(typeof(T), typeof(T))
                              let sourceValue = Expression.PropertyOrField(x, member.Name)
                              let target = Expression.PropertyOrField(y, member.Name)
                              select target.AssignFrom(sourceValue);

            var body = guardOfX.Concat(guardOfY).Concat(assignments).AsABlock();
            Trace.WriteLine(String.Format("Generated CopyTo method for type {0}: {1}", typeof(T), body));

            return body.CompileTo<Action<T, T>>(x, y);
        }
    }
}
