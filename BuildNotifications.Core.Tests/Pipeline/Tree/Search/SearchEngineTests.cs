using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search;

public class SearchEngineTests
{
    public SearchEngineTests()
    {
        _searchEngine = new SearchEngine();
        _searchEngine.AddCriteria(new DummyBranchSearchCriteria());
    }

    private readonly ISearchEngine _searchEngine;

    private class DummyBranchSearchCriteria : ISearchCriteria
    {
        public string LocalizedKeyword(CultureInfo forCultureInfo) => "Branch";
        public string LocalizedDescription(CultureInfo forCultureInfo) => "Unused";

        public IEnumerable<ISearchCriteriaSuggestion> Suggest(string input) => Enumerable.Empty<ISearchCriteriaSuggestion>();

        public bool IsBuildIncluded(IBuild build, string input) => string.IsNullOrWhiteSpace(input) || build.BranchName.Contains(input, StringComparison.OrdinalIgnoreCase);

        public IEnumerable<string> LocalizedExamples => Enumerable.Empty<string>();
    }

    private static IEnumerable<IBuild> DummyBuilds()
    {
        yield return DummyBuild("SomeBranch");
        yield return DummyBuild("SomeBranch");
        yield return DummyBuild("SomeOtherBranch");
        yield return DummyBuild("SomeOtherBranch");
    }

    private static IBuild DummyBuild(string branchName)
    {
        var build = Substitute.For<IBuild>();
        build.BranchName.Returns(branchName);

        return build;
    }

    [Theory]
    [InlineData(" ")]
    [InlineData(" t e s t ")]
    [InlineData(" t    e    s    \tt ")]
    [InlineData("t ")]
    [InlineData(" branch:")]
    [InlineData(" branch:branch:")]
    [InlineData(" branch:*branch:")]
    [InlineData(" bRanCh::Branch: ")]
    [InlineData("branch: someWord sBranch:")]
    [InlineData("branch: someWordBranch:test")]
    [InlineData("branch: someWord, Branch:test")]
    [InlineData("  branch: som  eWord,  Branch:  tes  t")]
    [InlineData("  branch,: som,  e,Wo,,,rd,  Bra,nch:  tes  t")]
    [InlineData("")]
    public void CombinedEnteredTextPropertiesShouldResultInOriginalInput(string input)
    {
        // arrange
        var search = _searchEngine.Parse(input);

        // act
        var blocks = search.Blocks;
        var combined = string.Join("", blocks.Select(b => (string.IsNullOrEmpty(b.SearchCriteria.LocalizedKeyword(CultureInfo.InvariantCulture)) ? string.Empty : b.SearchCriteria.LocalizedKeyword(CultureInfo.InvariantCulture) + ":") + b.EnteredText));

        // assert
        Assert.Equal(input, combined, StringComparer.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(" branch:test,test", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria), typeof(DefaultSearchCriteria))]
    [InlineData("branch:test,test", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria), typeof(DefaultSearchCriteria))]
    [InlineData("test, test", typeof(DefaultSearchCriteria), typeof(DefaultSearchCriteria))]
    [InlineData("test,test", typeof(DefaultSearchCriteria), typeof(DefaultSearchCriteria))]
    [InlineData(" test , test ", typeof(DefaultSearchCriteria), typeof(DefaultSearchCriteria))]
    [InlineData(",:notA:Key,word:ran:domText:,", typeof(DefaultSearchCriteria), typeof(DefaultSearchCriteria), typeof(DefaultSearchCriteria), typeof(DefaultSearchCriteria))]
    public void CommaBreaksSpecificCriteria(string input, params Type[] expectedCriterionTypes)
    {
        // arrange
        // act
        var search = _searchEngine.Parse(input);

        // assert
        var enumerable = search.Blocks.Select(c => c.SearchCriteria.GetType()).ToArray();
        Assert.Equal(expectedCriterionTypes, enumerable);
    }

    [Fact]
    public void EmptyInputDoesNotFilterBuilds()
    {
        // arrange
        var builds = DummyBuilds().ToList();
        var input = string.Empty;
        var search = _searchEngine.Parse(input);

        // act
        var filteredBuilds = search.ApplySearch(builds);

        // assert
        Assert.Equal(builds.Count, filteredBuilds.Count());
    }

    [Theory]
    [InlineData("branch:SomeBranch,", 1)]
    [InlineData("branch:SomeBranch, ", 1)]
    [InlineData(",branch:test,test", 2)]
    [InlineData("w,branch:test,test", 2)]
    [InlineData("test, test", 1)]
    [InlineData("test,test", 1)]
    [InlineData(" test , test ", 1)]
    public void EnteredTextDoesIncludesSpecificToGeneralSeparator(string input, int expectedAmount)
    {
        // arrange
        var search = _searchEngine.Parse(input);

        // act
        var blocks = search.Blocks;

        // assert
        Assert.Equal(blocks.Count(block => block.EnteredText.Contains(SearchEngine.SpecificToGeneralSeparator, StringComparison.InvariantCulture)), expectedAmount);
    }

    [Theory]
    [InlineData("branch:SomeBranch", 2)]
    [InlineData("branch:", 4)] // empty string branch search will include everything
    [InlineData("", 4)] // same as above, but this time triggered via the default criteria
    [InlineData("SomeBranch", 2)] // default will include branch search
    public void InputResultsInCertainNumberOfBuilds(string input, int expectedCount)
    {
        // arrange
        var builds = DummyBuilds().ToList();
        var search = _searchEngine.Parse(input);

        // act
        var filteredBuilds = search.ApplySearch(builds);

        // assert
        Assert.Equal(expectedCount, filteredBuilds.Count());
    }

    [Theory]
    [InlineData(" branch:", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria))]
    [InlineData(" branch:branch:", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria), typeof(DummyBranchSearchCriteria))]
    [InlineData(" branch:*branch:", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria), typeof(DummyBranchSearchCriteria))]
    [InlineData(" bRanCh::Branch: ", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria), typeof(DummyBranchSearchCriteria))]
    [InlineData("branch: someWord sBranch:", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria), typeof(DummyBranchSearchCriteria))]
    [InlineData("", typeof(DefaultSearchCriteria))]
    public void InputWithKeywordResultsInCertainSearchBlocks(string input, params Type[] expectedCriterionTypes)
    {
        // arrange
        // act
        var search = _searchEngine.Parse(input);

        // assert
        var enumerable = search.Blocks.Select(c => c.SearchCriteria.GetType()).ToArray();
        Assert.Equal(expectedCriterionTypes, enumerable);
    }

    [Theory]
    [InlineData("")]
    [InlineData("asd")]
    [InlineData("SomeBranch")]
    [InlineData("WordsBeforeBranchWordsAfter")]
    [InlineData("notAKeyword: random text")]
    [InlineData("notAKeyword:randomText")]
    [InlineData(":notA:Keyword:ran:domText:")]
    [InlineData("::::::::")]
    public void InputWithoutKeywordResultsInSearchWithDefaultCriteria(string input)
    {
        // arrange
        // act
        var search = _searchEngine.Parse(input);

        // assert
        Assert.Equal(1, search.Blocks.Count);
        var searchBlock = search.Blocks[0];
        Assert.IsType<DefaultSearchCriteria>(searchBlock.SearchCriteria);
        Assert.Equal(input, searchBlock.SearchedTerm);
    }

    [Theory]
    [InlineData("branch:SomeBranch,test")]
    [InlineData("branch:SomeBranch,")]
    [InlineData("branch:SomeBranch, ,,, ,")]
    [InlineData("branch:test,test")]
    [InlineData("test, test")]
    [InlineData("test,test")]
    [InlineData(" test , test ")]
    public void SearchedTermNeverIncludesSpecificToGeneralSeparator(string input)
    {
        // arrange
        var search = _searchEngine.Parse(input);

        // act
        var blocks = search.Blocks;

        // assert
        Assert.All(blocks, block => Assert.False(block.SearchedTerm.Contains(SearchEngine.SpecificToGeneralSeparator, StringComparison.InvariantCulture)));
    }

    [Theory]
    [InlineData(" ", "")]
    [InlineData(" t e s t ", "t e s t")]
    [InlineData(" t    e    s    \tt ", "t e s t")]
    [InlineData("t ", "t")]
    public void SearchedTermRemovesRedundantWhitespace(string input, string expectedTerm)
    {
        // arrange
        var search = _searchEngine.Parse(input);

        // act
        var blocks = search.Blocks;
        var searchBlock = blocks[0];

        // assert
        Assert.Equal(expectedTerm, searchBlock.SearchedTerm);
    }
}