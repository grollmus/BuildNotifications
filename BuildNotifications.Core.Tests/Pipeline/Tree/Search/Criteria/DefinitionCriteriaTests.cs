using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree.Search.Criteria;
using BuildNotifications.PluginInterfaces.Builds;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search.Criteria
{
    public class DefinitionCriteriaTests
    {
        private static CultureInfo TestCulture => CultureInfo.GetCultureInfo("en");

        static DefinitionCriteriaTests()
        {
            foreach (var tuple in DefinitionNameBuildTuples())
            {
                var definition = Substitute.For<IBuildDefinition>();
                definition.Name.Returns(tuple.definitionName);
                tuple.build.Definition.Returns(definition);
            }
        }

        private static readonly IBuild DefinitionABuild = Build();
        private static readonly IBuild DefinitionBBuild = Build();
        private static readonly IBuild DefinitionAAndBBuild = Build();

        private static IEnumerable<(string definitionName, IBuild build)> DefinitionNameBuildTuples()
        {
            yield return ("DefinitionA", DefinitionABuild);
            yield return ("DefinitionB", DefinitionBBuild);
            yield return ("DefinitionAB", DefinitionAAndBBuild);
        }

        public static IEnumerable<object[]> BranchNamesAsTestData() => DefinitionNameBuildTuples().Select(t => new object[] {t.definitionName});

        [Theory]
        [InlineData("A", 2)]
        [InlineData("*A", 2)]
        [InlineData("B", 2)]
        [InlineData("AB", 1)]
        [InlineData("*aa*", 0)]
        [InlineData("*Def", 3)]
        [InlineData("*Def*", 3)]
        [InlineData("Def", 3)]
        [InlineData("DefB", 0)]
        [InlineData("Def*B", 2)]
        public void InputResultsInAmountOfFilteredBranches(string input, int expectedAmount)
        {
            var criteria = TestCriteria();

            var result = DefinitionNameBuildTuples().Select(t => t.build).Where(b => criteria.IsBuildIncluded(b, input));

            Assert.Equal(expectedAmount, result.Count());
        }

        [Theory]
        [MemberData(nameof(BranchNamesAsTestData))]
        public void SuggestionsAreBasedOnExistingBranches(string definitionName)
        {
            var criteria = TestCriteria();
            for (var amountOfCharsEntered = 1; amountOfCharsEntered <= definitionName.Length; amountOfCharsEntered++)
            {
                for (var subsetPosition = 0; subsetPosition <= definitionName.Length - amountOfCharsEntered; subsetPosition++)
                {
                    var branchNameSubset = definitionName.Substring(subsetPosition, amountOfCharsEntered);
                    var suggestions = criteria.Suggest(branchNameSubset);

                    Assert.Contains(suggestions, s => s.Suggestion.Equals(definitionName, StringComparison.InvariantCulture));
                }
            }
        }

        private DefinitionCriteria TestCriteria()
        {
            var sut = new DefinitionCriteria(TestPipeline());
            sut.UseSpecificCulture(TestCulture);
            return sut;
        }

        private static IBuild Build()
        {
            var build = Substitute.For<IBuild>();
            return build;
        }

        private IPipeline TestPipeline()
        {
            var pipeline = Substitute.For<IPipeline>();
            pipeline.LastUpdate.Returns(DateTime.Now);

            pipeline.CachedDefinitions().Returns(x => DefinitionNameBuildTuples().Select(t =>
            {
                var definition = Substitute.For<IBuildDefinition>();
                definition.Name.Returns(t.definitionName);

                return definition;
            }).ToList());

            return pipeline;
        }
    }
}