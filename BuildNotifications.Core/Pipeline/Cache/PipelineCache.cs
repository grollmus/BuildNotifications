using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.Core.Pipeline.Cache
{
    internal class PipelineCache<T> : IPipelineCache<T>
    {
        /// <inheritdoc />
        public void AddOrReplace(CacheKey key, T item)
        {
            _items[key] = item;
        }

        public void AddOrReplace(int providerId, int itemId, T item)
        {
            AddOrReplace(new CacheKey(providerId, itemId), item);
        }

        /// <inheritdoc />
        public void Remove(CacheKey key)
        {
            _items.Remove(key, out _);
        }

        public void Remove(int providerId, int itemId)
        {
            Remove(new CacheKey(providerId, itemId));
        }

        /// <inheritdoc />
        public bool Contains(CacheKey key)
        {
            return _items.ContainsKey(key);
        }

        public bool Contains(int providerId, int itemId)
        {
            return Contains(new CacheKey(providerId, itemId));
        }

        public bool ContainsValue(T value)
        {
            return _items.ContainsValue(value);
        }

        public void RemoveValue(T value)
        {
            var entries = _items.Where(it => it.Value?.Equals(value) == true);
            foreach (var kvp in entries)
            {
                _items.Remove(kvp.Key);
            }
        }

        /// <inheritdoc />
        public IEnumerable<T> ContentCopy()
        {
            return new List<T>(_items.Values);
        }

        public void Clear() => _items.Clear();

        private readonly Dictionary<CacheKey, T> _items = new Dictionary<CacheKey, T>();
    }
}