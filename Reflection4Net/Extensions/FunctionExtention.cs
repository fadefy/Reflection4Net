using Reflection4Net.Cache;
using System;

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
    }
}
