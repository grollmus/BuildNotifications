using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search
{
    public class SearchEngineTests
    {
        private readonly ISearchEngine _searchEngine;

        public SearchEngineTests()
        {
            _searchEngine = new SearchEngine();
            _searchEngine.AddCriteria(new DummyBranchSearchCriteria());
        }

        private class DummyBranchSearchCriteria : ISearchCriteria
        {
            public string LocalizedKeyword => "Branch";
            public string LocalizedDescription => "Unused";

            public IEnumerable<ISearchCriteriaSuggestion> Suggest(string input) => Enumerable.Empty<ISearchCriteriaSuggestion>();

            public bool IsBuildIncluded(IBuild build, string input) => string.IsNullOrWhiteSpace(input) || build.BranchName.Contains(input, StringComparison.OrdinalIgnoreCase);

            public IEnumerable<string> LocalizedExamples => Enumerable.Empty<string>();
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
        [InlineData("")]
        [InlineData("asd")]
        [InlineData("SomeBranch")]
        [InlineData("WordsBeforeBranchWordsAfter")]
        [InlineData("notAKeyword: random text")]
        [InlineData("notAKeyword:randomText")]
        [InlineData(":notA:Keyword:ran:domText:")]
        [InlineData(",:notA:Key,word:ran:dom,Text:")]
        [InlineData("::::::::")]
        public void InputWithoutKeywordResultsInSearchWithDefaultCriteria(string input)
        {
            // arrange
            // act
            var search = _searchEngine.Parse(input);

            // assert
            Assert.Equal(1, search.Blocks.Count);
            var searchBlock = search.Blocks.First();
            Assert.IsType<DefaultSearchCriteria>(searchBlock.SearchCriteria);
            Assert.Equal(input, searchBlock.SearchedText);
        }

        [Theory]
        [InlineData(" branch:", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria))]
        [InlineData(" branch:branch:", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria))]
        [InlineData(" branch:*branch:", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria), typeof(DummyBranchSearchCriteria))]
        [InlineData(" bRanCh::Branch: ", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria), typeof(DummyBranchSearchCriteria))]
        [InlineData("branch: someWord sBranch:", typeof(DummyBranchSearchCriteria), typeof(DummyBranchSearchCriteria))]
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
        [InlineData(" branch:test,test", typeof(DefaultSearchCriteria), typeof(DummyBranchSearchCriteria), typeof(DefaultSearchCriteria))]
        [InlineData("branch:test,test", typeof(DummyBranchSearchCriteria), typeof(DefaultSearchCriteria))]
        [InlineData("test,test", typeof(DefaultSearchCriteria))]
        public void CommaBreaksSpecificCriteria(string input, params Type[] expectedCriterionTypes)
        {
            // arrange
            // act
            var search = _searchEngine.Parse(input);

            // assert
            var enumerable = search.Blocks.Select(c => c.SearchCriteria.GetType()).ToArray();
            Assert.Equal(expectedCriterionTypes, enumerable);
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
    }
}