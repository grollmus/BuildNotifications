using System;

namespace BuildNotifications.ViewModel.Utils
{
    internal static class DateTimeExtension
    {
        public static TimeSpan TimespanToNow(this DateTime date)
        {
            return DateTime.Now - date;
        }
    }
}
