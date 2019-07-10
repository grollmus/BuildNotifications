using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BuildNotifications.Core.Pipeline.Cache
{
    internal class PipelineCache<T> : IPipelineCache<T>
    {
        /// <inheritdoc />
        public void AddOrReplace(CacheKey key, T item)
        {
            _items[key] = item;
        }

        /// <inheritdoc />
        public void Remove(CacheKey key)
        {
            _items.Remove(key, out _);
        }

        /// <inheritdoc />
        public bool Contains(CacheKey key)
        {
            return _items.ContainsKey(key);
        }

        /// <inheritdoc />
        public IEnumerable<T> ContentCopy()
        {
            return new List<T>(_items.Values);
        }

        private readonly Dictionary<CacheKey, T> _items = new Dictionary<CacheKey, T>();
    }
}