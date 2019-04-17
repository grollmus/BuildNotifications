using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline
{
    public class PipelineTests
    {
        [Fact]
        public async Task BranchProviderShouldNotBeCalledMultipleTimesWhenDefinedInMultipleProjects()
        {
            // Arrange
            var builder = Substitute.For<ITreeBuilder>();
            var sut = new Core.Pipeline.Pipeline(builder);

            var branchProvider = Substitute.For<IBranchProvider>();

            var project1 = new Project(Substitute.For<IBuildProvider>(), branchProvider, Substitute.For<IProjectConfiguration>());
            var project2 = new Project(Substitute.For<IBuildProvider>(), branchProvider, Substitute.For<IProjectConfiguration>());

            sut.AddProject(project1);
            sut.AddProject(project2);

            // Act
            await sut.Update();

            // Assert
            await branchProvider.Received(1).FetchExistingBranches().GetAsyncEnumerator().DisposeAsync();
        }

        [Fact]
        public async Task BuildProviderShouldNotBeCalledMultipleTimesWhenDefinedInMultipleProjects()
        {
            // Arrange
            var builder = Substitute.For<ITreeBuilder>();
            var sut = new Core.Pipeline.Pipeline(builder);

            var buildProvider = Substitute.For<IBuildProvider>();
            var project1 = new Project(buildProvider, Substitute.For<IBranchProvider>(), Substitute.For<IProjectConfiguration>());
            var project2 = new Project(buildProvider, Substitute.For<IBranchProvider>(), Substitute.For<IProjectConfiguration>());

            sut.AddProject(project1);
            sut.AddProject(project2);

            // Act
            await sut.Update();

            // Assert
            await buildProvider.Received(1).FetchExistingBuildDefinitions().GetAsyncEnumerator().DisposeAsync();
        }

        [Fact]
        public async Task UpdateShouldFetchAllBuildDefinitions()
        {
            // Arrange
            var builder = Substitute.For<ITreeBuilder>();
            var sut = new Core.Pipeline.Pipeline(builder);

            var buildProvider = Substitute.For<IBuildProvider>();
            var project = new Project(buildProvider, Substitute.For<IBranchProvider>(), Substitute.For<IProjectConfiguration>());
            sut.AddProject(project);

            // Act
            await sut.Update();

            // Assert
            await buildProvider.Received(1).FetchExistingBuildDefinitions().GetAsyncEnumerator().DisposeAsync();
        }

        [Fact]
        public async Task UpdateShouldFetchAllBuildsOnlyWhenCalledTheFirstTime()
        {
            // Arrange
            var builder = Substitute.For<ITreeBuilder>();
            var sut = new Core.Pipeline.Pipeline(builder);

            var buildProvider = Substitute.For<IBuildProvider>();

            var project = new Project(buildProvider, Substitute.For<IBranchProvider>(), Substitute.For<IProjectConfiguration>());
            sut.AddProject(project);

            // Act
            await sut.Update();
            await sut.Update();

            // Assert
            await buildProvider.Received(1).FetchAllBuilds().GetAsyncEnumerator().DisposeAsync();
        }
    }
}