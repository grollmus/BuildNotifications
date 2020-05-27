using System;
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
        /// <returns>Action that was performed by this operation.</returns>
        void AddOrReplace(CacheKey key, T item);

        /// <summary>
        /// Adds or replaces an item into the cache.
        /// </summary>
        /// <param name="providerId">Provider part of the key to add item for.</param>
        /// <param name="itemId">Item part of the key to add item for.</param>
        /// <param name="item">Item to add.</param>
        /// <returns>Action that was performed by this operation.</returns>
        void AddOrReplace(string providerId, string itemId, T item);

        /// <summary>
        /// Clears all stored data.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determine if an item with a given key exists in the cache.
        /// </summary>
        /// <param name="key">Key to check for.</param>
        /// <returns><c>true</c> if the item exists; otherwise <c>false</c>.</returns>
        bool Contains(CacheKey key);

        /// <summary>
        /// Determine if an item with a given key exists in the cache.
        /// </summary>
        /// <param name="providerId">Provider part of the key to add item for.</param>
        /// <param name="itemId">Item part of the key to add item for.</param>
        /// <returns><c>true</c> if the item exists; otherwise <c>false</c>.</returns>
        bool Contains(string providerId, string itemId);

        /// <summary>
        /// Determine if an item that matches the given predicate exists in the cache.
        /// </summary>
        /// <param name="predicate">Predicate to use for searching the item.</param>
        /// <returns><c>true</c> if at least one item in the cache matches the predicate; otherwise <c>false</c>.</returns>
        bool Contains(Predicate<T> predicate);

        /// <summary>
        /// Determine if an item with a given value exists in the cache.
        /// </summary>
        /// <param name="value">Value to check for.</param>
        /// <returns><c>true</c> if the item exists; otherwise <c>false</c>.</returns>
        bool ContainsValue(T value);

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

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="providerId">Provider part of the key to add item for.</param>
        /// <param name="itemId">Item part of the key to add item for.</param>
        void Remove(string providerId, string itemId);

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="value">Value to remove item for.</param>
        void RemoveValue(T value);

        /// <summary>
        /// Returns all stored values for the given provider.
        /// </summary>
        /// <param name="providerId">Id of the provider to fetch data for.</param>
        /// <returns>List of all values matching the provider.</returns>
        IEnumerable<T> Values(string providerId);

        /// <summary>
        /// Returns the entire content of this cache.
        /// </summary>
        /// <returns>Read only dictionary of all CacheKeys and their associated values.</returns>
        IReadOnlyDictionary<CacheKey, T> CachedValues();

        /// <summary>
        /// Amount of items present within the cache.
        /// </summary>
        int Size { get; }
    }
}