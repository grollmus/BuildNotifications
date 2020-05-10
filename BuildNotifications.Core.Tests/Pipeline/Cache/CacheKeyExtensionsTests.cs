using System;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Cache;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Cache
{
    public class CacheKeyExtensionsTests
    {
        [Fact]
        public void CacheKeyForBuildIsCompositeOfBuildIdAndProjectId()
        {
            var build = Substitute.For<IBuild>();
            var expectedProviderId = Guid.NewGuid();
            var expectedItemId = "123";
            build.ProjectId.Returns(expectedProviderId);
            build.Id.Returns(expectedItemId);

            var result = build.CacheKey();

            Assert.Equal(new CacheKey(expectedProviderId.ToString(), expectedItemId), result);
        }

        [Fact]
        public void CacheKeyForBuildIsCompositeOfDefinitionIdAndProjectId()
        {
            var definition = Substitute.For<IBuildDefinition>();
            var project = Substitute.For<IProject>();

            var expectedProviderId = Guid.NewGuid();
            project.Guid.Returns(expectedProviderId);

            var expectedItemId = "123";
            definition.Id.Returns(expectedItemId);

            var result = definition.CacheKey(project);

            Assert.Equal(new CacheKey(expectedProviderId.ToString(), expectedItemId), result);
        }

        [Fact]
        public void CacheKeyForBuildIsCompositeOfBranchFullNameAndProjectId()
        {
            var branch = Substitute.For<IBranch>();
            var project = Substitute.For<IProject>();

            var expectedProviderId = Guid.NewGuid();
            project.Guid.Returns(expectedProviderId);

            var expectedItemId = "123";
            branch.FullName.Returns(expectedItemId);

            var result = branch.CacheKey(project);

            Assert.Equal(new CacheKey(expectedProviderId.ToString(), expectedItemId), result);
        }
    }
}