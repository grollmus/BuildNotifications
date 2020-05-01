using System;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Search.Criteria;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search.Criteria
{
    public class BeforeCriteriaTests : DateCriteriaTests
    {
        public BeforeCriteriaTests() : base(new BeforeCriteria())
        {
        }

        [Theory]
        [InlineData("T", "Today")]
        [InlineData("t", "Today")]
        [InlineData("Tod", "Today")]
        [InlineData("tOd", "Today")]
        [InlineData("Today", "Today")]
        [InlineData("today", "Today")]
        public void InputsResultInTodaySuggestion(string input, string expectedSuggestion) => ExpectSuggestion(input, expectedSuggestion);

        private const string ReferenceDate = "6/15/2020";

        [Theory]
        [InlineData("04/2020")]
        [InlineData("4/2020")]
        [InlineData("6/15/2020")]
        [InlineData("7/15/2019")]
        [InlineData("6/1/2020")]
        [InlineData("5/16/2020")]
        [InlineData("Today")]
        [InlineData("RandomText")]
        public void CriteriaDoesExcludeBuildForGivenInput(string input) => ExpectNoMatch(ReferenceDate, input);

        [Theory]
        [InlineData("07/2020")]
        [InlineData("7/2020")]
        [InlineData("6/16/2020")]
        [InlineData("6/14/2021")]
        [InlineData("7/1/2020")]
        [InlineData("  7/1/2020")]
        [InlineData("7/1/2020  ")]
        [InlineData("  7/1/2020  ")]
        [InlineData("")]
        [InlineData("   ")]
        public void CriteriaDoesIncludeBuildForGivenInput(string input) => ExpectMatch(ReferenceDate, input);
        
        [Fact]
        public void CriteriaDoesIncludeBuildForTodayInputThatMatchesToday() => ExpectMatch((DateTime.Today - TimeSpan.FromDays(1)).ToString("d", TestCulture), "Today");

        [Fact]
        public void CriteriaDoesExcludeBuildForTodayInputThatDoesNotMatchToday() => ExpectNoMatch(DateTime.Today.ToString("d", TestCulture), "Today");

        [Theory]
        [InlineData("1")]
        [InlineData("1/")]
        [InlineData("10")]
        [InlineData("12")]
        [InlineData("1/5")]
        public void SuggestionsAreSortedDescending(string input)
        {
            var suggestions = CriteriaUnderTest.Suggest(input).ToList();

            var asDateTimes = suggestions.Select(s => DateTime.Parse(s.Suggestion, TestCulture, DateTimeStyles.AssumeLocal)).ToList();
            var sortedRecentToOldest = asDateTimes.OrderByDescending(x => x);

            Assert.Equal(sortedRecentToOldest, asDateTimes);
        }
    }
}