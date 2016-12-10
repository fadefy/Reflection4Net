using System.Collections;
using System.Collections.Generic;
using Reflection4Net.Extensions;

namespace Reflection4Net.Cache
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class DictionaryCache<K, V> : ICache<K, V>
    {
        private Dictionary<K, V> cache = new Dictionary<K, V>();

        public long Count
        {
            get { return cache.Count; }
        }

        public void Clear()
        {
            cache.Clear();
        }

        public void Remove(K key)
        {
            cache.Remove(key);
        }

        public void Cache(K key, V value)
        {
            ArgumentsGuards.NotNull(() => value);

            cache.Add(key, value);
        }

        public bool TryGetValue(K key, out V value)
        {
            return cache.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return cache.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return cache.GetEnumerator();
        }
    }
}
