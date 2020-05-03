using System;

namespace BuildNotifications.Core.Pipeline.Cache
{
    internal sealed class CacheKey
    {
        public CacheKey(string providerId, string itemId)
        {
            _providerId = providerId;
            _itemId = itemId;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj!.GetType() != GetType())
                return false;

            return EqualsOtherCacheKey((CacheKey) obj);
        }

        public override int GetHashCode() => HashCode.Combine(_providerId, _itemId);

        public bool IsProvider(string providerId)
        {
            return _providerId == providerId;
        }

        private bool EqualsOtherCacheKey(CacheKey other)
        {
            return
                string.Equals(_providerId, other._providerId, StringComparison.InvariantCulture)
                && string.Equals(_itemId, other._itemId, StringComparison.InvariantCulture);
        }

        private readonly string _providerId;
        private readonly string _itemId;
    }
}