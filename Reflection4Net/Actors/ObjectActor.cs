using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Reflection4Net.Extensions;

namespace Reflection4Net.Builder
{
    public class ObjectActor<T>
    {
        protected MethodInfo ReferenceEqual = typeof(Object).GetMethod("ReferenceEquals", BindingFlags.Static | BindingFlags.Public);
        protected MethodInfo GetHash = typeof(Object).GetMethod("GetHashCode", BindingFlags.Instance | BindingFlags.Public);

        public static Expression NullObject = Expression.Constant(new object());

        [Pure]
        public virtual Func<T, T, bool> GenerateEquals()
        {
            var x = typeof(T).Parameter("x");
            var y = typeof(T).Parameter("y");
            var referenceEqual = ReferenceEqual.CallWith(x, y);
            var membersCompare = (from member in GetMembersForEquality()
                                  let memberX = x.GetPropertyOrFieldAsType<Object>(member.Name)
                                  let memberY = y.GetPropertyOrFieldAsType<Object>(member.Name)
                                  select memberX.EqualTo(memberY) as Expression).ToArray();
            if (membersCompare.Length == 0)
                throw new InvalidOperationException(String.Format("Type {0} doesn't contain any member to compare.", typeof(T)));

            var xEqualsY = membersCompare.Aggregate((result, memberEqual) => result.AndAlso(memberEqual));
            var neitherIsNull = Expression.AndAlso(x.NotEqualTo(null), y.NotEqualTo(null));
            var equals = Expression.Condition(referenceEqual, Expression.Constant(true), Expression.Condition(neitherIsNull, xEqualsY, Expression.Constant(false)));

            Trace.Write(String.Format("Generated Equals method for type {0}: {1}", typeof(T), equals));

            return equals.CompileTo<Func<T, T, bool>>(x, y);
        }

        [Pure]
        public virtual Func<T, int> GenerateGetHash()
        {
            var instance = typeof(T).Parameter("instance");
            var hashValues = from member in GetMembersForEquality()
                             let memberValue = instance.GetPropertyOrFieldAsType<Object>(member.Name).Coalesce(NullObject)
                             select GetHash.CallOn(memberValue) as Expression;
            var aggregatedHash = hashValues.Aggregate((result, hash) => result.ExclusiveOr(hash));

            Trace.Write(String.Format("Generated GetHash method for type {0}: {1}", typeof(T), aggregatedHash));

            return aggregatedHash.CompileTo<Func<T, int>>(instance);
        }

        [Pure]
        public virtual IEnumerable<MemberInfo> GetMembersForEquality()
        {
            return typeof(T).GetFieldsWithOut<NonSerializedAttribute>();
        }

        [Pure]
        public virtual IEnumerable<MemberInfo> GetMembersForCopy()
        {
            return typeof(T).GetFieldsWithOut<NonSerializedAttribute>();
        }
    }
}
