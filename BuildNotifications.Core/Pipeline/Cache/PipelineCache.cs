using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BuildNotifications.Core.Pipeline.Cache
{
    internal class PipelineCache<T> : IPipelineCache<T>
    {
        /// <inheritdoc />
        public CacheAction AddOrReplace(CacheKey key, T item)
        {
            var action = CacheAction.Add;
            _items.AddOrUpdate(key, cacheKey => item, (cacheKey, oldItem) =>
            {
                if (Equals(oldItem, item))
                {
                    action = CacheAction.None;
                    return oldItem;
                }

                action = CacheAction.Update;
                return item;
            });

            return action;
        }

        /// <inheritdoc />
        public void Remove(CacheKey key)
        {
            _items.TryRemove(key, out _);
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

        private readonly ConcurrentDictionary<CacheKey, T> _items = new ConcurrentDictionary<CacheKey, T>();
    }
}