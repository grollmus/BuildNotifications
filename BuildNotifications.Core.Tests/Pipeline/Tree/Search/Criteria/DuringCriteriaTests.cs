using System;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Search.Criteria;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search.Criteria;

public class DuringCriteriaTests : DateCriteriaTests
{
    public DuringCriteriaTests()
        : base(new DuringCriteria(MockPipelineWithBuildsFromReferenceDay()))
    {
    }

    [Theory]
    [InlineData("04/2020")]
    [InlineData("4/2020")]
    [InlineData("7/15/2019")]
    [InlineData("6/1/2020")]
    [InlineData("5/16/2020")]
    [InlineData("07/2020")]
    [InlineData("7/2020")]
    [InlineData("6/16/2020")]
    [InlineData("6/14/2021")]
    [InlineData("7/1/2020")]
    [InlineData("  7/1/2020")]
    [InlineData("7/1/2020  ")]
    [InlineData("Today")]
    [InlineData("Yesterday")]
    [InlineData("RandomText")]
    public void CriteriaDoesExcludeBuildForGivenInput(string input) => ExpectNoMatch(ReferenceDate, input);

    [Fact]
    public void CriteriaDoesExcludeBuildForTodayInputThatDoesNotMatchToday() => ExpectNoMatch((ReferenceToday - TimeSpan.FromDays(1)).ToString("d", TestCulture), "Today");

    [Fact]
    public void CriteriaDoesExcludeBuildForYesterdayInputThatDoesNotMatchYesterday() => ExpectNoMatch(ReferenceToday.ToString("d", TestCulture), "Yesterday");

    [Theory]
    [InlineData("  6/15/2020  ")]
    [InlineData("6/15/2020")]
    [InlineData("")]
    [InlineData("   ")]
    public void CriteriaDoesIncludeBuildForGivenInput(string input) => ExpectMatch(ReferenceDate, input);

    [Fact]
    public void CriteriaDoesIncludeBuildForTodayInputThatMatchesToday() => ExpectMatch(ReferenceToday.ToString("d", TestCulture), "Today");

    [Fact]
    public void CriteriaDoesIncludeBuildForYesterdayInputThatMatchesYesterday() => ExpectMatch((ReferenceToday - TimeSpan.FromDays(1)).ToString("d", TestCulture), "Yesterday");

    [Theory]
    [InlineData("T", "Today")]
    [InlineData("t", "Today")]
    [InlineData("Tod", "Today")]
    [InlineData("tOd", "Today")]
    [InlineData("Today", "Today")]
    [InlineData("today", "Today")]
    [InlineData("Y", "Yesterday")]
    [InlineData("y", "Yesterday")]
    [InlineData("Yes", "Yesterday")]
    [InlineData("yEs", "Yesterday")]
    [InlineData("Yesterday", "Yesterday")]
    [InlineData("yesterday", "Yesterday")]
    public void InputsResultInSuggestion(string input, string expectedSuggestion) => ExpectSuggestion(input, expectedSuggestion);

    [Theory]
    [InlineData("6/15/2020")]
    [InlineData("6")]
    [InlineData("6/")]
    [InlineData("6/15")]
    [InlineData(" 6/15")]
    [InlineData(" 6/15 ")]
    [InlineData("1")]
    [InlineData("5")]
    [InlineData("15")]
    [InlineData("20")]
    [InlineData("202")]
    [InlineData("2")]
    [InlineData(" ")]
    public void SuggestionsAreBasedOnReferenceDate(string input)
    {
        var suggestions = CriteriaUnderTest.Suggest(input).ToList();

        var expectedValidDate = DateTime.Parse(ReferenceDate, TestCulture, DateTimeStyles.AssumeLocal);
        Assert.Contains(suggestions, s => DateTime.TryParse(s.Suggestion, TestCulture, DateTimeStyles.AssumeLocal, out var asDateTime) && asDateTime.Equals(expectedValidDate));
    }
}