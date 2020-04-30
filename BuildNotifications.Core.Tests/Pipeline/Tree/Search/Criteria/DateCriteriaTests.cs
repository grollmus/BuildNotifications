using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Search.Criteria;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search.Criteria
{
    public abstract class DateCriteriaTests
    {
        protected ISearchCriteria CriteriaUnderTest { get; }
        protected static CultureInfo TestCulture => CultureInfo.GetCultureInfo("en");

        protected DateCriteriaTests(BaseSearchCriteria criteriaUnderTest)
        {
            CriteriaUnderTest = criteriaUnderTest;
            criteriaUnderTest.UseSpecificCulture(TestCulture);
        }
        
        protected void ExpectNoMatch(string referenceDate, string input)
        {
            var build = BuildFromReferenceDate(referenceDate);

            var isMatch = CriteriaUnderTest.IsBuildIncluded(build, input);

            Assert.False(isMatch);
        }
        
        protected void ExpectMatch(string referenceDate, string input)
        {
            var build = BuildFromReferenceDate(referenceDate);

            var isMatch = CriteriaUnderTest.IsBuildIncluded(build, input);

            Assert.True(isMatch);
        }

        private void ExpectDateTimeSuggestion(string input, string expectedDateTimeSuggestion)
        {
            var suggestions = CriteriaUnderTest.Suggest(input);

            Assert.Contains(suggestions, suggestion => AsDateTime(suggestion.Suggestion).Equals(AsDateTime(expectedDateTimeSuggestion)));
        }

        protected void ExpectSuggestion(string input, string expectedSuggestion)
        {
            var suggestions = CriteriaUnderTest.Suggest(input);

            Assert.Contains(suggestions, suggestion => suggestion.Suggestion.Equals(expectedSuggestion, StringComparison.InvariantCulture));
        }

        private static DateTime AsDateTime(string stringRepresentation) => DateTime.Parse(stringRepresentation, TestCulture, DateTimeStyles.AssumeLocal);

        [Theory]
        [MemberData(nameof(TodaySuggestionTestData))]
        public void CriteriaSuggestsTodayForInput(string input, string expectedSuggestion) => ExpectDateTimeSuggestion(input, expectedSuggestion);

        [Theory]
        [InlineData("1")]
        [InlineData("1/")]
        [InlineData("10")]
        [InlineData("12")]
        [InlineData("1/5")]
        public void SuggestionsAreDistinct(string input)
        {
            var suggestions = CriteriaUnderTest.Suggest(input).ToList();

            var distinctSuggestions = suggestions.Select(s => DateTime.Parse(s.Suggestion, TestCulture, DateTimeStyles.AssumeLocal)).Distinct();

            Assert.Equal(suggestions.Count, distinctSuggestions.Count());
        }

        public static IEnumerable<object[]> TodaySuggestionTestData()
        {
            var todayAsString = DateTime.Today.ToString("d", TestCulture);
            for (var i = 1; i <= todayAsString.Length; i++)
            {
                yield return new object[] {todayAsString.Substring(0, i), todayAsString};
            }
        }

        protected IBuild BuildFromReferenceDate(string dateStringRepresentationInTestCulture)
        {
            var build = Substitute.For<IBuild>();
            build.QueueTime.Returns(DateTime.Parse(dateStringRepresentationInTestCulture, TestCulture));

            return build;
        }
    }
}