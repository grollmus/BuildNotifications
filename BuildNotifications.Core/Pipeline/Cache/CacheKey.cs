namespace BuildNotifications.Core.Pipeline.Cache
{
    internal sealed class CacheKey
    {
        public CacheKey(int providerId, int itemId)
        {
            _providerId = providerId;
            _itemId = itemId;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return EqualsOtherCacheKey((CacheKey) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (_providerId * 397) ^ _itemId;
            }
        }

        private bool EqualsOtherCacheKey(CacheKey other)
        {
            return _providerId == other._providerId && _itemId == other._itemId;
        }

        private readonly int _providerId;
        private readonly int _itemId;
    }
}