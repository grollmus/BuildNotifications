using System;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.TestMocks;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree
{
    public class BuildNodeTests
    {
        [Fact]
        public void ConstructorShouldSetCorrectValues()
        {
            // Arrange
            var build = Substitute.For<IBuild>();
            build.Progress.Returns(123);
            build.LastChangedTime.Returns(DateTime.Now);
            build.QueueTime.Returns(DateTime.Today);
            build.Status.Returns(BuildStatus.Pending);

            // Act
            var sut = new BuildNode(build);

            // Assert
            Assert.Equal(build.LastChangedTime, sut.LastChangedTime);
            Assert.Equal(build.QueueTime, sut.QueueTime);
            Assert.Equal(build.Progress, sut.Progress);
            Assert.Equal(build.Status, sut.Status);
            Assert.Same(build, sut.Build);
        }

        [Fact]
        public void UpdateWithValuesFromShouldDoNothginWhenNodeIsNotBuildNode()
        {
            // Arrange
            var build = Substitute.For<IBuild>();
            build.Progress.Returns(123);
            build.LastChangedTime.Returns(DateTime.Now);
            build.QueueTime.Returns(DateTime.Today);
            build.Status.Returns(BuildStatus.Pending);
            var sut = new BuildNode(build);

            // Act
            sut.UpdateWithValuesFrom(new BranchGroupNode("test", false));

            // Assert
            Assert.Equal(build.LastChangedTime, sut.LastChangedTime);
            Assert.Equal(build.QueueTime, sut.QueueTime);
            Assert.Equal(build.Progress, sut.Progress);
            Assert.Equal(build.Status, sut.Status);
            Assert.Same(build, sut.Build);
        }

        [Fact]
        public void UpdateWithValuesFromShouldSetCorrectProperties()
        {
            // Arrange
            var build = Substitute.For<IBuild>();
            build.Progress.Returns(123);
            build.LastChangedTime.Returns(DateTime.Now);
            build.QueueTime.Returns(DateTime.Today);
            build.Status.Returns(BuildStatus.Pending);

            var sut = new BuildNode(new MockBuild());

            var buildNode = new BuildNode(build);

            // Act
            sut.UpdateWithValuesFrom(buildNode);

            // Assert
            Assert.Equal(build.LastChangedTime, sut.LastChangedTime);
            Assert.Equal(build.QueueTime, sut.QueueTime);
            Assert.Equal(build.Progress, sut.Progress);
            Assert.Equal(build.Status, sut.Status);
            Assert.NotSame(build, sut.Build);
        }
    }
}