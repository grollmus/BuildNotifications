using System;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Search.Criteria;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search.Criteria;

public class AfterCriteriaTests : DateCriteriaTests
{
    public AfterCriteriaTests()
        : base(new AfterCriteria(MockPipelineWithBuildsFromReferenceDay()))
    {
    }

    [Theory]
    [InlineData("07/2020")]
    [InlineData("7/2020")]
    [InlineData("6/15/2020")]
    [InlineData("5/15/2021")]
    [InlineData("6/28/2020")]
    [InlineData("7/14/2020")]
    [InlineData("RandomText")]
    [InlineData("Today")]
    public void CriteriaDoesExcludeBuildForGivenInput(string input) => ExpectNoMatch(ReferenceDate, input);

    [Fact]
    public void CriteriaDoesExcludeBuildForYesterdayInputThatDoesNotMatchToday() => ExpectNoMatch((ReferenceToday - TimeSpan.FromDays(1)).ToString("d", TestCulture), "Yesterday");

    [Theory]
    [InlineData("06/2020")]
    [InlineData("6/2020")]
    [InlineData("6/14/2020")]
    [InlineData("6/16/2019")]
    [InlineData("6/1/2020")]
    [InlineData("  6/1/2020")]
    [InlineData("6/1/2020  ")]
    [InlineData("  6/1/2020  ")]
    [InlineData("")]
    [InlineData("   ")]
    public void CriteriaDoesIncludeBuildForGivenInput(string input) => ExpectMatch(ReferenceDate, input);

    [Fact]
    public void CriteriaDoesIncludeBuildForYesterdayInputThatMatchesToday() => ExpectMatch(ReferenceToday.ToString("d", TestCulture), "Yesterday");

    [Theory]
    [InlineData("Y", "Yesterday")]
    [InlineData("y", "Yesterday")]
    [InlineData("Yes", "Yesterday")]
    [InlineData("yEs", "Yesterday")]
    [InlineData("Yesterday", "Yesterday")]
    [InlineData("yesterday", "Yesterday")]
    public void InputsResultInYesterdaySuggestion(string input, string expectedSuggestion) => ExpectSuggestion(input, expectedSuggestion);

    [Theory]
    [InlineData("6/14/2020")]
    [InlineData("6")]
    [InlineData("6/")]
    [InlineData("6/14")]
    [InlineData(" 6/14")]
    [InlineData(" 6/14 ")]
    [InlineData("4")]
    [InlineData("1")]
    [InlineData("20")]
    [InlineData("202")]
    [InlineData("")]
    [InlineData(" ")]
    public void SuggestionsAreBasedOnReferenceDate(string input)
    {
        var suggestions = CriteriaUnderTest.Suggest(input).ToList();

        // the criteria checks dates after the input. Therefore with a build on the reference date, the first valid date is the day before
        var expectedValidDate = DateTime.Parse(ReferenceDate, TestCulture, DateTimeStyles.AssumeLocal) - TimeSpan.FromDays(1);
        Assert.Contains(suggestions, s => DateTime.TryParse(s.Suggestion, TestCulture, DateTimeStyles.AssumeLocal, out var asDateTime) && asDateTime.Equals(expectedValidDate));
    }
}