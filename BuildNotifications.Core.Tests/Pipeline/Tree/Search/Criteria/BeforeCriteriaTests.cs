using System;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Search.Criteria;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search.Criteria;

public class BeforeCriteriaTests : DateCriteriaTests
{
    public BeforeCriteriaTests()
        : base(new BeforeCriteria(MockPipelineWithBuildsFromReferenceDay()))
    {
    }

    [Theory]
    [InlineData("04/2020")]
    [InlineData("4/2020")]
    [InlineData("6/15/2020")]
    [InlineData("7/15/2019")]
    [InlineData("6/1/2020")]
    [InlineData("5/16/2020")]
    [InlineData("RandomText")]
    public void CriteriaDoesExcludeBuildForGivenInput(string input) => ExpectNoMatch(ReferenceDate, input);

    [Fact]
    public void CriteriaDoesExcludeBuildForTodayInputThatDoesNotMatchToday() => ExpectNoMatch(ReferenceToday.ToString("d", TestCulture), "Today");

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
    [InlineData("Today")]
    public void CriteriaDoesIncludeBuildForGivenInput(string input) => ExpectMatch(ReferenceDate, input);

    [Fact]
    public void CriteriaDoesIncludeBuildForTodayInputThatMatchesToday() => ExpectMatch((ReferenceToday - TimeSpan.FromDays(1)).ToString("d", TestCulture), "Today");

    [Theory]
    [InlineData("T", "Today")]
    [InlineData("t", "Today")]
    [InlineData("Tod", "Today")]
    [InlineData("tOd", "Today")]
    [InlineData("Today", "Today")]
    [InlineData("today", "Today")]
    public void InputsResultInTodaySuggestion(string input, string expectedSuggestion) => ExpectSuggestion(input, expectedSuggestion);

    [Theory]
    [InlineData("6/16/2020")]
    [InlineData("6")]
    [InlineData("6/")]
    [InlineData("6/16")]
    [InlineData(" 6/16")]
    [InlineData(" 6/16 ")]
    [InlineData(" 16 ")]
    [InlineData(" 6 ")]
    [InlineData(" 1 ")]
    [InlineData("")]
    [InlineData(" ")]
    public void SuggestionsAreBasedOnReferenceDate(string input)
    {
        var suggestions = CriteriaUnderTest.Suggest(input).ToList();

        // the criteria checks dates before the input. Therefore with a build on the reference date, the first valid date is the day after
        var expectedValidDate = DateTime.Parse(ReferenceDate, TestCulture, DateTimeStyles.AssumeLocal) + TimeSpan.FromDays(1);
        Assert.Contains(suggestions, s => DateTime.TryParse(s.Suggestion, TestCulture, DateTimeStyles.AssumeLocal, out var asDateTime) && asDateTime.Equals(expectedValidDate));
    }
}