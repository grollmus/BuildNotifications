using System.Collections.Generic;

namespace BuildNotifications.Core.Pipeline.Cache
{
    /// <summary>
    /// Cache that is used by the pipeline.
    /// </summary>
    /// <typeparam name="T">Type that is cached.</typeparam>
    internal interface IPipelineCache<T>
    {
        /// <summary>
        /// Adds or replaces an item into the cache.
        /// </summary>
        /// <param name="key">Key to add item for.</param>
        /// <param name="item">Item to add.</param>
        /// <returns><c>true</c> if the item was added; <c>false</c> if it was replaced.</returns>
        bool AddOrReplace(CacheKey key, T item);

        /// <summary>
        /// Determine if an item with a given key exists in the cache.
        /// </summary>
        /// <param name="key">Key to check for.</param>
        /// <returns><c>true</c> if the item exists; otherwise <c>false</c>.</returns>
        bool Contains(CacheKey key);

        /// <summary>
        /// Creates a copy of the caches content that can be iterated over.
        /// </summary>
        /// <returns>A copy of the content of the cache.</returns>
        IEnumerable<T> ContentCopy();

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">Key to remove item for.</param>
        void Remove(CacheKey key);
    }
}