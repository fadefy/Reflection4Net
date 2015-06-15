using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Reflection4Net.Extensions;

namespace Reflection4Net.Actors
{
    public class Equals<T> : AbstractOperation<Func<T, T, bool>>
    {
        public override Func<T, T, bool> Build(ITypeMappingInfoProvider infoProvider)
        {
            var x = typeof(T).Parameter("x");
            var y = typeof(T).Parameter("y");
            var membersCompare = (from member in infoProvider.GetOperandMembers(typeof(T), typeof(T))
                                  let memberX = x.GetPropertyOrFieldAsType<Object>(member.Name)
                                  let memberY = y.GetPropertyOrFieldAsType<Object>(member.Name)
                                  select memberX.EqualTo(memberY)).ToArray();
            if (membersCompare.Length == 0)
                throw new InvalidOperationException(String.Format("Type {0} doesn't contain any member to compare.", typeof(T)));

            var xEqualsY = membersCompare.Aggregate((result, memberEqual) => result.AndAlso(memberEqual));
            var neitherIsNull = x.NotEqualTo(null).AndAlso(y.NotEqualTo(null));
            var equals = Expression.Condition(x.ReferenceEqualTo(y), Expression.Constant(true), Expression.Condition(neitherIsNull, xEqualsY, Expression.Constant(false)));

            Trace.Write(String.Format("Generated Equals method for type {0}: {1}", typeof(T), equals));

            return equals.CompileTo<Func<T, T, bool>>(x, y);
        }
    }
}
