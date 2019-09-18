using System;
using System.Collections.Generic;
using System.Globalization;
using BuildNotifications.Core.Text;
using BuildNotifications.Core.Utilities;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Xunit;

namespace BuildNotifications.Core.Tests.Utilities
{
    public class TimeSpanToStringConverterTests
    {
#nullable disable
        // see https://github.com/xunit/xunit/issues/1897
        public static IEnumerable<object[]> TestCases2
        {
            [UsedImplicitly]
            get
            {
                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 2, 0, 0, 0), GetTimeString(nameof(TimeSpanToStringConverter.OneDayAgoTextId))};
                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 2, 23, 0, 0), GetTimeString(nameof(TimeSpanToStringConverter.OneDayAgoTextId))};

                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 1, 1, 0, 0), GetTimeString(nameof(TimeSpanToStringConverter.NMinutesAgoTextId), 60)};
                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 1, 2, 0, 0), GetTimeString(nameof(TimeSpanToStringConverter.NHoursAgoTextId), 2)};
                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 1, 1, 59, 0), GetTimeString(nameof(TimeSpanToStringConverter.NMinutesAgoTextId), 119)};

                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 1, 0, 1, 0), GetTimeString(nameof(TimeSpanToStringConverter.NSecondsAgoTextId), 60)};
                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 1, 0, 2, 0), GetTimeString(nameof(TimeSpanToStringConverter.NMinutesAgoTextId), 2)};
                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 1, 0, 1, 59), GetTimeString(nameof(TimeSpanToStringConverter.NSecondsAgoTextId), 119)};

                yield return new object[] {BaseDateTime, BaseDateTime, GetTimeString(nameof(TimeSpanToStringConverter.JustNowTextId))};
                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 1, 0, 0, 59), GetTimeString(nameof(TimeSpanToStringConverter.NSecondsAgoTextId), 59)};
                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 1, 0, 0, TimeSpanToStringConverter.JustNowSeconds), GetTimeString(nameof(TimeSpanToStringConverter.JustNowTextId))};
                yield return new object[] {BaseDateTime, new DateTime(2000, 1, 1, 0, 0, TimeSpanToStringConverter.JustNowSeconds + 1), GetTimeString(nameof(TimeSpanToStringConverter.NSecondsAgoTextId), TimeSpanToStringConverter.JustNowSeconds + 1)};
            }
        }
#nullable enable

        private static DateTime BaseDateTime { get; } = new DateTime(2000, 1, 1, 0, 0, 0);

        [Theory]
        [MemberData(nameof(TestCases2))]
        public void CalculateShouldWorkForAllCases(DateTime from, DateTime to, string expected)
        {
            // Arrange
            Resources.Culture = TestCulture;
            var converter = new TimeSpanToStringConverter();
            var timeSpan = to - from;

            // Act
            var actual = converter.Convert(timeSpan);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        private static string GetTimeString(string textId, int amount)
        {
            var baseText = GetTimeString(textId);
            return string.Format(baseText, amount);
        }

        private static string GetTimeString(string textId)
        {
            return StringLocalizer.Instance.GetText(textId);
        }

        private static readonly CultureInfo TestCulture = CultureInfo.InvariantCulture;

    }
}