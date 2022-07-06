using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildNotifications.Core;

public static class ObjectExtensions
{
    public static async IAsyncEnumerable<T> AsyncYield<T>(this T obj)
    {
        await Task.CompletedTask;
        yield return obj;
    }

    public static IEnumerable<T> Yield<T>(this T obj)
    {
        return new[] { obj };
    }
}