namespace BuildNotifications.Core.Pipeline.Cache;

/// <summary>
/// Action that has been performed by a cache operation.
/// </summary>
internal enum CacheAction
{
    /// <summary>
    /// No action performed. The item was already in the cache and has not been changed.
    /// </summary>
    None,

    /// <summary>
    /// Item was added to the cache.
    /// </summary>
    Add,

    /// <summary>
    /// Item was updated in the cache
    /// </summary>
    Update,

    /// <summary>
    /// Item has been removed from the cache.
    /// </summary>
    Remove
}