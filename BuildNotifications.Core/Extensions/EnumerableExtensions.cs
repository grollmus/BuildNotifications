using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.Core
{
    internal static class EnumerableExtensions
    {
        internal static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> list)
        {
            return list.SelectMany(x => x);
        }
    }
}