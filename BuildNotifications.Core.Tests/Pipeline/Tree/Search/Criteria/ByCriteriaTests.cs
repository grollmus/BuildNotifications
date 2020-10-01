using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree.Search.Criteria;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search.Criteria
{
    public class ByCriteriaTests
    {
        private static CultureInfo TestCulture => CultureInfo.GetCultureInfo("en");

        static ByCriteriaTests()
        {
            foreach (var tuple in ByNameBuildTuples())
            {
                tuple.build.BranchName.Returns(tuple.forName);
            }
        }

        private static readonly IBuild ByABuild = Build("ByA");
        private static readonly IBuild ByBBuild = Build("ByB");
        private static readonly IBuild ByAbBuild = Build("ByAB");

        private static IEnumerable<(string forName, IBuild build)> ByNameBuildTuples()
        {
            yield return ("ByA", ByABuild);
            yield return ("ByB", ByBBuild);
            yield return ("ByAB", ByAbBuild);
        }

        public static IEnumerable<object[]> ByNamesAsTestData() => ByNameBuildTuples().Select(t => new object[] {t.forName!});

        [Theory]
        [InlineData("A", 2)]
        [InlineData("*A", 2)]
        [InlineData("B", 3)]
        [InlineData("AB", 1)]
        [InlineData("*ff*", 0)]
        [InlineData("*By", 3)]
        [InlineData("*By*", 3)]
        [InlineData("By", 3)]
        [InlineData("BA", 0)]
        [InlineData("B*A", 2)]
        [InlineData("By*B", 2)]
        public void InputResultsInAmountOfFilteredBranches(string input, int expectedAmount)
        {
            var criteria = TestCriteria();

            var result = ByNameBuildTuples().Select(t => t.build).Where(b => criteria.IsBuildIncluded(b, input));

            Assert.Equal(expectedAmount, result.Count());
        }

        [Theory]
        [MemberData(nameof(ByNamesAsTestData))]
        public void SuggestionsAreBasedOnExistingNames(string branchName)
        {
            var criteria = TestCriteria();
            for (var amountOfCharsEntered = 1; amountOfCharsEntered <= branchName.Length; amountOfCharsEntered++)
            {
                for (var subsetPosition = 0; subsetPosition <= branchName.Length - amountOfCharsEntered; subsetPosition++)
                {
                    var branchNameSubset = branchName.Substring(subsetPosition, amountOfCharsEntered);
                    var suggestions = criteria.Suggest(branchNameSubset);

                    Assert.Contains(suggestions, s => s.Suggestion.Equals(branchName, StringComparison.InvariantCulture));
                }
            }
        }

        private ByCriteria TestCriteria()
        {
            var sut = new ByCriteria(TestPipeline());
            sut.UseSpecificCulture(TestCulture);
            return sut;
        }

        private static IBuild Build(string requestedByDisplayName)
        {
            var build = Substitute.For<IBuild>();
            BuildIsRequestedBy(requestedByDisplayName, build);
            return build;
        }

        private IPipeline TestPipeline()
        {
            var pipeline = Substitute.For<IPipeline>();
            pipeline.LastUpdate.Returns(DateTime.Now);

            pipeline.CachedBuilds().Returns(x => ByNameBuildTuples().Select(t =>
            {
                var build = Substitute.For<IBuild>();
                BuildIsRequestedBy(t.forName, build);

                return build;
            }).ToList());

            return pipeline;
        }

        private static void BuildIsRequestedBy(string requestedBy, IBuild build)
        {
            var user = Substitute.For<IUser>();

            user.DisplayName.Returns(requestedBy);
            build.RequestedBy.Returns(user);
        }
    }
}