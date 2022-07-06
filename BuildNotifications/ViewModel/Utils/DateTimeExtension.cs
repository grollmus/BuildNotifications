using System;

namespace BuildNotifications.ViewModel.Utils;

internal static class DateTimeExtension
{
    public static TimeSpan TimespanToNow(this DateTime date) => DateTime.Now - date;

    public static TimeSpan TimespanToUtcNow(this DateTime date) => DateTime.UtcNow - date;
}