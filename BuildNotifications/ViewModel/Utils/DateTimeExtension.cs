using System;

namespace BuildNotifications.ViewModel.Utils
{
    internal static class DateTimeExtension
    {
        public static TimeSpan TimespanToNow(this DateTime date)
        {
            return DateTime.Now - date;
        }

        public static TimeSpan TimespanToUtcNow(this DateTime date)
        {
            return DateTime.UtcNow - date;
        }
    }
}