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
    public class DefaultSearchCriteriaTests
    {
        private readonly DefaultSearchCriteria _criteriaToTest;

        private abstract class BaseDummySearch : ISearchCriteria
        {
            private readonly string _searchDummyString;
            private readonly string[] _suggestionsForAnyInputNotEqualDummyString;
            public string LocalizedKeyword => "Unused";

            public string LocalizedDescription => "Unused";

            protected BaseDummySearch(string searchDummyString, params string[] suggestionsForAnyInputNotEqualDummyString)
            {
                _searchDummyString = searchDummyString;
                _suggestionsForAnyInputNotEqualDummyString = suggestionsForAnyInputNotEqualDummyString;
            }

            public IEnumerable<ISearchCriteriaSuggestion> Suggest(string input)
            {
                if (input.Contains(_searchDummyString, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return AsSuggestion(_searchDummyString);
                    yield return AsSuggestion(_searchDummyString + _searchDummyString);
                    yield return AsSuggestion(_searchDummyString + _searchDummyString + _searchDummyString);
                }
                else
                {
                    var suggestions = _suggestionsForAnyInputNotEqualDummyString.Select(AsSuggestion);
                    foreach (var searchCriteriaSuggestion in suggestions)
                    {
                        yield return searchCriteriaSuggestion;
                    }
                }
            }

            public bool IsBuildIncluded(IBuild build, string input) => string.IsNullOrWhiteSpace(input) || input.Contains(_searchDummyString, StringComparison.OrdinalIgnoreCase);

            public IEnumerable<string> LocalizedExamples => _suggestionsForAnyInputNotEqualDummyString;
        }

        private class DummySearchCriteriaA : BaseDummySearch
        {
            public DummySearchCriteriaA() : base("A", "1", "2", "3")
            {
            }
        }

        private class DummySearchCriteriaB : BaseDummySearch
        {
            public DummySearchCriteriaB() : base("B", "4", "5", "6")
            {
            }
        }

        private static ISearchCriteriaSuggestion AsSuggestion(string suggestionText)
        {
            var suggestion = Substitute.For<ISearchCriteriaSuggestion>();
            suggestion.Suggestion.Returns(suggestionText);

            return suggestion;
        }

        public DefaultSearchCriteriaTests()
        {
            _criteriaToTest = new DefaultSearchCriteria(new ISearchCriteria[] {new DummySearchCriteriaA(), new DummySearchCriteriaB()});
        }

        [Fact]
        public void SuggestionsComeFromEachCriteria()
        {
            // arrange
            // act
            var actual = _criteriaToTest.Suggest(string.Empty).Select(s => s.Suggestion).ToList();

            // assert
            Assert.Equal(actual, new[] {"Unused:", "Unused:", "1", "2", "4", "5"});
        }

        [Fact]
        public void SuggestionsFromEachCriteriaAreLimited()
        {
            // arrange
            var referenceA = new DummySearchCriteriaA();
            var referenceB = new DummySearchCriteriaB();
            var totalSuggestions = referenceA.Suggest(string.Empty).Concat(referenceB.Suggest(string.Empty)).Count();
            totalSuggestions += 2; // the search criterions themselves are also suggested within the DefaultCriteria.

            // act
            var actual = _criteriaToTest.Suggest(string.Empty);

            // assert
            Assert.NotEqual(totalSuggestions, actual.Count());
        }

        [Fact]
        public void ExamplesFromEachSearchCriteriaAreLimited()
        {
            // arrange
            var referenceA = new DummySearchCriteriaA();
            var referenceB = new DummySearchCriteriaB();
            var totalSuggestions = referenceA.LocalizedExamples.Concat(referenceB.LocalizedExamples).Count();

            // act
            var actual = _criteriaToTest.LocalizedExamples;

            // assert
            Assert.NotEqual(totalSuggestions, actual.Count());
        }

        [Fact]
        public void ExamplesAreFromEachSearchCriteria()
        {
            // arrange
            var referenceA = new DummySearchCriteriaA();
            var referenceB = new DummySearchCriteriaB();

            // act
            var actual = _criteriaToTest.LocalizedExamples.ToList();

            // assert
            Assert.Contains(actual, e => referenceA.LocalizedExamples.Contains(e));
            Assert.Contains(actual, e => referenceB.LocalizedExamples.Contains(e));
        }

        [Fact]
        public void SuggestionsChangeBasedOnInput()
        {
            // arrange
            var referenceA = new DummySearchCriteriaA();
            var referenceB = new DummySearchCriteriaB();
            var suggestionsA = referenceA.Suggest(string.Empty).Select(s => s.Suggestion);
            var suggestionsB = referenceB.Suggest(string.Empty).Select(s => s.Suggestion);

            // act
            var actual = _criteriaToTest.Suggest("AB").Select(s => s.Suggestion).ToList();

            // assert
            Assert.DoesNotContain(actual, x => suggestionsA.Contains(x));
            Assert.DoesNotContain(actual, x => suggestionsB.Contains(x));
        }

        [Theory]
        [InlineData("A")]
        [InlineData("B")]
        [InlineData("")]
        public void BuildIsIncludedIfAnyCriteriaMatches(string input)
        {
            // arrange
            var build = Substitute.For<IBuild>();

            // act
            var result = _criteriaToTest.IsBuildIncluded(build, input);

            // assert
            Assert.True(result);
        }
    }
}