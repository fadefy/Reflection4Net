using Reflection4Net.Cache;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Reflection4Net.Extensions
{
    public static class FunctionExtention
    {
        public static Func<TK, TV> CacheIn<TK, TV>(this Func<TK, TV> createValue, ICache<TK, TV> cache)
        {
            return (key) =>
            {
                TV value;
                if (!cache.TryGetValue(key, out value))
                {
                    lock (cache)
                    {
                        value = createValue(key);
                        cache.Cache(key, value);
                    }
                }
                return value;
            };
        }

        public static Action<T1, T2> CastToGenericAction<T1, T2>(this Delegate actionDelegate)
        {
            var target = Expression.Parameter(typeof(T1), "target");
            var value = Expression.Parameter(typeof(T2), "value");
            var parameters = actionDelegate.Method.GetParameters();
            if (actionDelegate.Method.IsStatic)
            {
                var expectedTargetType = parameters.First().ParameterType;
                var expectedValueType = parameters.Skip(1).First().ParameterType;
                var call = Expression.Call(actionDelegate.Method, Expression.Convert(target, expectedTargetType), Expression.Convert(value, expectedValueType));

                return Expression.Lambda<Action<T1, T2>>(call, target, value).Compile();
            }
            else if (parameters.Length == 2)
            {
                var expectedValueType = parameters.First().ParameterType;
                var call = Expression.Call(Expression.Constant(actionDelegate.Target), actionDelegate.Method, target, Expression.Convert(value, expectedValueType));

                return Expression.Lambda<Action<T1, T2>>(call, target, value).Compile();
            }
            else if (parameters.Length == 1)
            {
                var expectedValueType = parameters.First().ParameterType;
                var call = Expression.Call(target, actionDelegate.Method, Expression.Convert(value, expectedValueType));

                return Expression.Lambda<Action<T1, T2>>(call, target, value).Compile();
            }

            throw new NotSupportedException();
        }

        public static Action<T1, T2> CastAsAction<T1, T2>(this Delegate actionDelegate)
        {
            return (a1, a2) => actionDelegate.DynamicInvoke(a1, a2);
        }
    }
}
