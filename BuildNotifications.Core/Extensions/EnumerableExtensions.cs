using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.Core
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> list)
        {
            return list.Where(x => x != null);
        }
      
        internal static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> list)
        {
            return list.SelectMany(x => x);
        }
    }
}