using System.Collections.Generic;

namespace Reflection4Net.Cache
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="KeyType"></typeparam>
    /// <typeparam name="ValueType"></typeparam>
    public interface ICache<KeyType, ValueType> : IEnumerable<KeyValuePair<KeyType, ValueType>>
    {
        long Count { get; }

        void Clear();

        void Remove(KeyType key);

        void Cache(KeyType key, ValueType value);

        bool TryGetValue(KeyType key, out ValueType value);
    }
}
