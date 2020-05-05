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
    public class IsCriteriaTests
    {
        private static CultureInfo TestCulture => CultureInfo.GetCultureInfo("en");

        static IsCriteriaTests()
        {
            FailedBuild.Status.Returns(BuildStatus.Failed);
            SucceededBuild.Status.Returns(BuildStatus.Succeeded);
            PartiallySucceededBuild.Status.Returns(BuildStatus.PartiallySucceeded);
            CancelledBuild.Status.Returns(BuildStatus.Cancelled);
            InProgressBuild.Status.Returns(BuildStatus.Running);
            PendingBuild.Status.Returns(BuildStatus.Pending);

            ManualBuild.Reason.Returns(BuildReason.Manual);
            ScheduledBuild.Reason.Returns(BuildReason.Scheduled);
            CiBuild.Reason.Returns(BuildReason.CheckedIn);
            PullRequestBuild.Reason.Returns(BuildReason.PullRequest);

            ByUserBuild.IsRequestedByCurrentUser.Returns(true);
        }

        private const string DefaultRequestedById = "1";

        private static IUser TestUser
        {
            get
            {
                var testUser = Substitute.For<IUser>();
                testUser.Id.Returns(DefaultRequestedById);
                return testUser;
            }
        }

        private static readonly IBuild FailedBuild = Build();
        private static readonly IBuild SucceededBuild = Build();
        private static readonly IBuild PartiallySucceededBuild = Build();
        private static readonly IBuild CancelledBuild = Build();
        private static readonly IBuild InProgressBuild = Build();
        private static readonly IBuild PendingBuild = Build();

        private static readonly IBuild ManualBuild = Build();
        private static readonly IBuild ScheduledBuild = Build();
        private static readonly IBuild CiBuild = Build();
        private static readonly IBuild PullRequestBuild = Build();

        private static readonly IBuild ByUserBuild = Build();

        public static IEnumerable<(string keyword, IBuild build)> KeywordBuildTuples()
        {
            yield return ("Failed", FailedBuild);
            yield return ("Succeeded", SucceededBuild);
            yield return ("Partially succeeded", PartiallySucceededBuild);
            yield return ("Cancelled", CancelledBuild);
            yield return ("In progress", InProgressBuild);
            yield return ("Pending", PendingBuild);

            yield return ("Manual", ManualBuild);
            yield return ("Scheduled", ScheduledBuild);
            yield return ("CI", CiBuild);
            yield return ("Pull request", PullRequestBuild);

            yield return ("By me", ByUserBuild);
        }

        public static IEnumerable<object[]> KeywordsAsTestData() => KeywordBuildTuples().Select(t => new object[] {t.keyword});

        private static IBuild Build()
        {
            var build = Substitute.For<IBuild>();
            build.RequestedBy.Returns(x => TestUser);
            return build;
        }

        [Theory]
        [MemberData(nameof(KeywordsAsTestData))]
        public void KeywordsAreSuggestedIfSubsetsOfTheirKeywordsAreEntered(string keyword)
        {
            var criteria = TestCriteria();
            criteria.MaxAmountOfSuggestions = int.MaxValue;

            for (var amountOfCharsEntered = 1; amountOfCharsEntered <= keyword.Length; amountOfCharsEntered++)
            {
                for (var subsetPosition = 0; subsetPosition <= keyword.Length - amountOfCharsEntered; subsetPosition++)
                {
                    var keywordSubset = keyword.Substring(subsetPosition, amountOfCharsEntered);
                    var suggestions = criteria.Suggest(keywordSubset);

                    Assert.Contains(suggestions, s => s.Suggestion.Equals(keyword, StringComparison.InvariantCulture));
                }
            }
        }

        [Theory]
        [MemberData(nameof(KeywordsAsTestData))]
        public void AllOtherKeywordsExcludeBuild(string keyword)
        {
            var criteria = TestCriteria();
            var allBuilds = KeywordBuildTuples().Select(t => t.build);
            var buildThatShouldNotBeExcluded = KeywordBuildTuples().First(t => t.keyword.Equals(keyword, StringComparison.InvariantCulture)).build;

            var includedBuilds = allBuilds.Where(b => criteria.IsBuildIncluded(b, keyword));

            Assert.DoesNotContain(includedBuilds, b => b.Equals(buildThatShouldNotBeExcluded));
        }

        [Theory]
        [MemberData(nameof(KeywordsAsTestData))]
        public void KeywordDoesIncludeBuild(string keyword)
        {
            var criteria = TestCriteria();
            var buildThatShouldBeIncluded = KeywordBuildTuples().First(t => t.keyword.Equals(keyword, StringComparison.InvariantCulture)).build;

            var result = criteria.IsBuildIncluded(buildThatShouldBeIncluded, keyword);

            Assert.True(result);
        }

        private IsCriteria TestCriteria()
        {
            var sut = new IsCriteria(TestPipeline());
            sut.UseSpecificCulture(TestCulture);
            return sut;
        }

        private IPipeline TestPipeline()
        {
            var pipeline = Substitute.For<IPipeline>();
            pipeline.LastUpdate.Returns(DateTime.Now);

            pipeline.CachedBuilds().Returns(x => KeywordBuildTuples().Select(t => t.build).ToList());

            return pipeline;
        }
    }
}