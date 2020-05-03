using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.Core.Pipeline.Cache
{
    internal class PipelineCache<T> : IPipelineCache<T>
    {
        public void AddOrReplace(CacheKey key, T item)
        {
            _items[key] = item;
        }

        public void AddOrReplace(string providerId, string itemId, T item)
        {
            AddOrReplace(new CacheKey(providerId, itemId), item);
        }

        public void Remove(CacheKey key)
        {
            _items.Remove(key, out _);
        }

        public void Remove(string providerId, string itemId)
        {
            Remove(new CacheKey(providerId, itemId));
        }

        public bool Contains(CacheKey key)
        {
            return _items.ContainsKey(key);
        }

        public bool Contains(string providerId, string itemId)
        {
            return Contains(new CacheKey(providerId, itemId));
        }

        public bool Contains(Predicate<T> predicate)
        {
            return _items.Values.Any(it => predicate(it));
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

        public IEnumerable<T> Values(string providerId)
        {
            return _items.Where(kvp => kvp.Key.IsProvider(providerId)).Select(kvp => kvp.Value);
        }

        public IEnumerable<T> ContentCopy()
        {
            return new List<T>(_items.Values);
        }

        public IReadOnlyDictionary<CacheKey, T> CachedValues() => _items;

        public void Clear()
        {
            _items.Clear();
        }

        private readonly Dictionary<CacheKey, T> _items = new Dictionary<CacheKey, T>();
    }
}