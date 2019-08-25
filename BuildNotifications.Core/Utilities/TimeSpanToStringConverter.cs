using System;
using BuildNotifications.Core.Text;

namespace BuildNotifications.Core.Utilities
{
    /// <summary>
    /// Converts a timespan to a readable string. E.g. 5 min ago
    /// </summary>
    public class TimeSpanToStringConverter
    {
        /// <summary>
        ///     Maximum amount of seconds that may be passed, for a timeSpan to be considered "just now"
        /// </summary>
        public const int JustNowSeconds = 10;

        public const string NDaysAgoTextId = nameof(NDaysAgoTextId);
        public const string OneDayAgoTextId = nameof(OneDayAgoTextId);
        public const string NHoursAgoTextId = nameof(NHoursAgoTextId);
        public const string NMinutesAgoTextId = nameof(NMinutesAgoTextId);
        public const string NSecondsAgoTextId = nameof(NSecondsAgoTextId);
        public const string JustNowTextId = nameof(JustNowTextId);

        public string Convert(TimeSpan value)
        {
            var amount = 1;
            string text;

            // n days
            if (value.Days > 1)
            {
                text = NDaysAgoTextId;
                amount = value.Days;
            }
            // one day
            else if (value.Days == 1)
                text = OneDayAgoTextId;
            // n hours
            else if (value.Hours > 1)
            {
                text = NHoursAgoTextId;
                amount = value.Hours;
            }
            // one hour
            else if (value.Hours == 1)
            {
                text = NMinutesAgoTextId;
                amount = value.Minutes + 60;
            }
            // n minutes
            else if (value.Minutes > 1)
            {
                text = NMinutesAgoTextId;
                amount = value.Minutes;
            }
            else if (value.Seconds + value.Minutes * 60 > JustNowSeconds)
            {
                text = NSecondsAgoTextId;
                amount = value.Seconds + value.Minutes*60;
            }
            // just now
            else
                text = JustNowTextId;

            return string.Format(StringLocalizer.Instance.GetText(text), amount);
        }
    }
}