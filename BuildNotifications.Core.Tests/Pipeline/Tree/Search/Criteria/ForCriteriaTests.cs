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

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search.Criteria;

public class ForCriteriaTests
{
    static ForCriteriaTests()
    {
        foreach (var tuple in ForNameBuildTuples())
        {
            tuple.build.BranchName.Returns(tuple.forName);
        }
    }

    private static CultureInfo TestCulture => CultureInfo.GetCultureInfo("en");

    public static IEnumerable<object[]> ForNamesAsTestData() => ForNameBuildTuples().Where(t => t.forName != null).Select(t => new object[] { t.forName! });

    [Theory]
    [InlineData("A", 2)]
    [InlineData("*A", 2)]
    [InlineData("B", 2)]
    [InlineData("AB", 1)]
    [InlineData("*ff*", 0)]
    [InlineData("*For", 3)]
    [InlineData("*For*", 3)]
    [InlineData("For", 3)]
    [InlineData("FoA", 0)]
    [InlineData("Fo*A", 2)]
    [InlineData("For*B", 2)]
    public void InputResultsInAmountOfFilteredBranches(string input, int expectedAmount)
    {
        var criteria = TestCriteria();

        var result = ForNameBuildTuples().Select(t => t.build).Where(b => criteria.IsBuildIncluded(b, input));

        Assert.Equal(expectedAmount, result.Count());
    }

    [Theory]
    [MemberData(nameof(ForNamesAsTestData))]
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

    private static IBuild Build(string? requestedForDisplayName)
    {
        var build = Substitute.For<IBuild>();
        BuildIsRequestedFor(requestedForDisplayName, build);
        return build;
    }

    private static void BuildIsRequestedFor(string? requestedFor, IBuild build)
    {
        var user = Substitute.For<IUser>();

        if (requestedFor == null)
            build.RequestedFor.Returns((IUser?)null);
        else
        {
            user.DisplayName.Returns(requestedFor);
            build.RequestedFor.Returns(user);
        }
    }

    private static IEnumerable<(string? forName, IBuild build)> ForNameBuildTuples()
    {
        yield return ("ForA", ForABuild);
        yield return ("ForB", ForBBuild);
        yield return ("ForAB", ForAbBuild);
        yield return (null, ForNullBuild);
    }

    private ForCriteria TestCriteria()
    {
        var sut = new ForCriteria(TestPipeline());
        sut.UseSpecificCulture(TestCulture);
        return sut;
    }

    private IPipeline TestPipeline()
    {
        var pipeline = Substitute.For<IPipeline>();
        pipeline.LastUpdate.Returns(DateTime.Now);

        pipeline.CachedBuilds().Returns(_ => ForNameBuildTuples().Select(t =>
        {
            var build = Substitute.For<IBuild>();
            BuildIsRequestedFor(t.forName, build);

            return build;
        }).ToList());

        return pipeline;
    }

    private static readonly IBuild ForABuild = Build("ForA");
    private static readonly IBuild ForBBuild = Build("ForB");
    private static readonly IBuild ForAbBuild = Build("ForAB");

    private static readonly IBuild ForNullBuild = Build(null);
}