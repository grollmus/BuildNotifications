using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree.Search.Criteria;
using BuildNotifications.PluginInterfaces.Builds;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search.Criteria
{
    public class BaseSearchCriteriaTests
    {
        [Theory]
        [InlineData(" h", "h")]
        [InlineData("h   ", "h")]
        [InlineData("  h   ", "h")]
        public void InputIsTrimmedOfSpacesForIsBuildIncluded(string input, string expectedResult)
        {
            var criteria = new TestSearchCriteria(Substitute.For<IPipeline>());

            criteria.IsBuildIncluded(Substitute.For<IBuild>(), input);

            Assert.Equal(expectedResult, criteria.IsBuildIncludedInternalReceivedCalls.First().input);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(" ", "")]
        [InlineData(" h", "h")]
        [InlineData("h   ", "h")]
        [InlineData("  h   ", "h")]
        public void InputIsTrimmedOfSpacesForSuggestInternal(string input, string expectedResult)
        {
            var criteria = new TestSearchCriteria(Substitute.For<IPipeline>());

            criteria.Suggest(input);

            Assert.Equal(expectedResult, criteria.SuggestInternalReceivedCalls.First().input);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("    ")]
        public void IsBuildIncludedInternalIsNotCalledForWhitespaceInput(string input)
        {
            var criteria = new TestSearchCriteria(Substitute.For<IPipeline>());

            criteria.IsBuildIncluded(Substitute.For<IBuild>(), input);

            Assert.Equal(0, criteria.IsBuildIncludedInternalReceivedCalls.Count);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("    ")]
        public void IsBuildIncludedInternalIsTrueForWhitespaceInput(string input)
        {
            var criteria = new TestSearchCriteria(Substitute.For<IPipeline>());

            var result = criteria.IsBuildIncluded(Substitute.For<IBuild>(), input);

            Assert.True(result);
        }

        [Fact]
        public void UpdateCacheIsOnlyCalledOnceWhenPipelineUpdateIsMoreRecent()
        {
            var pipeline = Substitute.For<IPipeline>();
            pipeline.LastUpdate.Returns(DateTime.Now);
            var criteria = new TestSearchCriteria(pipeline);

            criteria.Suggest("validSearchTerm");
            criteria.Suggest("validSearchTerm");
            criteria.Suggest("validSearchTerm");
            var receivedCalls = criteria.UpdateCacheForSuggestionsReceivedCalls.Count;

            Assert.Equal(1, receivedCalls);
        }

        [Fact]
        public void UpdateCacheIsNotCalledWhenPipelineHasNoRecentUpdate()
        {
            var pipeline = Substitute.For<IPipeline>();
            var criteria = new TestSearchCriteria(Substitute.For<IPipeline>());
            var anyBuild = Substitute.For<IBuild>();
            pipeline.LastUpdate.Returns(DateTime.MinValue);

            criteria.IsBuildIncluded(anyBuild, string.Empty);
            var receivedCalls = criteria.UpdateCacheForSuggestionsReceivedCalls.Count;

            Assert.Equal(0, receivedCalls);
        }

        private class TestSearchCriteria : BaseSearchCriteria
        {
            public TestSearchCriteria(IPipeline pipeline) : base(string.Empty, string.Empty, pipeline)
            {
            }

            protected override IEnumerable<string> SuggestInternal(string input, StringMatcher stringMatcher)
            {
                SuggestInternalReceivedCalls.Add((input, stringMatcher));
                return Enumerable.Empty<string>();
            }

            protected override void UpdateCacheForSuggestions(IPipeline pipeline) => UpdateCacheForSuggestionsReceivedCalls.Add(pipeline);

            protected override bool IsBuildIncludedInternal(IBuild build, string input)
            {
                IsBuildIncludedInternalReceivedCalls.Add((build, input));
                return true;
            }

            protected override IEnumerable<string> Examples() => Enumerable.Empty<string>();

            public IList<(string input, StringMatcher stringMatcher)> SuggestInternalReceivedCalls { get; } = new List<(string input, StringMatcher stringMatcher)>();

            public IList<IPipeline> UpdateCacheForSuggestionsReceivedCalls { get; } = new List<IPipeline>();

            public IList<(IBuild build, string input)> IsBuildIncludedInternalReceivedCalls { get; } = new List<(IBuild build, string input)>();
        }
    }
}