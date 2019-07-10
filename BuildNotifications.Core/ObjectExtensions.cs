using System.Collections.Generic;

namespace BuildNotifications.Core
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> Yield<T>(this T obj)
        {
            return new[] {obj};
        }
    }
}