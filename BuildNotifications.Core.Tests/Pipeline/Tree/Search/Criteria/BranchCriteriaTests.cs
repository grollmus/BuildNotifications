using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree.Search.Criteria;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search.Criteria;

public class BranchCriteriaTests
{
    static BranchCriteriaTests()
    {
        foreach (var tuple in BranchNameBuildTuples())
        {
            tuple.build.BranchName.Returns(tuple.branchName);
        }
    }

    private static CultureInfo TestCulture => CultureInfo.GetCultureInfo("en");

    public static IEnumerable<object[]> BranchNamesAsTestData() => BranchNameBuildTuples().Select(t => new object[] { t.branchName });

    [Theory]
    [InlineData("A", 3)]
    [InlineData("*A", 3)]
    [InlineData("B", 3)]
    [InlineData("AB", 1)]
    [InlineData("*aa*", 0)]
    [InlineData("*Bra", 3)]
    [InlineData("*Bra*", 3)]
    [InlineData("Bra", 3)]
    [InlineData("BrA", 3)]
    [InlineData("Bra*B", 2)]
    public void InputResultsInAmountOfFilteredBranches(string input, int expectedAmount)
    {
        var criteria = TestCriteria();

        var result = BranchNameBuildTuples().Select(t => t.build).Where(b => criteria.IsBuildIncluded(b, input));

        Assert.Equal(expectedAmount, result.Count());
    }

    [Theory]
    [MemberData(nameof(BranchNamesAsTestData))]
    public void SuggestionsAreBasedOnExistingBranches(string branchName)
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

    private static IEnumerable<(string branchName, IBuild build)> BranchNameBuildTuples()
    {
        yield return ("BranchA", BranchABuild);
        yield return ("BranchB", BranchBBuild);
        yield return ("BranchAB", BranchAAndBBuild);
    }

    private static IBuild Build(string branchName)
    {
        var build = Substitute.For<IBuild>();
        var branch = Substitute.For<IBranch>();
        branch.DisplayName.Returns(branchName);
        build.Branch.Returns(branch);

        return build;
    }

    private BranchCriteria TestCriteria()
    {
        var sut = new BranchCriteria(TestPipeline());
        sut.UseSpecificCulture(TestCulture);
        return sut;
    }

    private IPipeline TestPipeline()
    {
        var pipeline = Substitute.For<IPipeline>();
        pipeline.LastUpdate.Returns(DateTime.Now);

        pipeline.CachedBranches().Returns(_ => BranchNameBuildTuples().Select(t =>
        {
            var branch = Substitute.For<IBranch>();
            branch.DisplayName.Returns(t.branchName);

            return branch;
        }).ToList());

        return pipeline;
    }

    private static readonly IBuild BranchABuild = Build("BranchA");
    private static readonly IBuild BranchBBuild = Build("BranchB");
    private static readonly IBuild BranchAAndBBuild = Build("BranchAB");
}