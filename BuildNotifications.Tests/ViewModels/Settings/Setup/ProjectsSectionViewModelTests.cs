using System;
using System.Linq;
using System.Windows;
using BuildNotifications.Core.Config;
using BuildNotifications.Services;
using BuildNotifications.ViewModel.Settings.Setup;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Setup
{
    public class ProjectsSectionViewModelTests
    {
        [Fact]
        public void AddCommandShouldAddNewProject()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();
            Action saveAction = () => { };
            var popupService = Substitute.For<IPopupService>();
            var sut = new ProjectsSectionViewModel(configurationBuilder, configuration, saveAction, popupService);

            // Act
            sut.AddProjectCommand.Execute(null);

            // Assert
            Assert.Single(sut.Projects);
        }

        [Fact]
        public void RemoveCommandShouldRemoveConnection()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();
            Action saveAction = () => { };
            var popupService = Substitute.For<IPopupService>();
            popupService.ShowMessageBox(Arg.Any<string>(), Arg.Any<string>(), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No).Returns(MessageBoxResult.Yes);
            var sut = new ProjectsSectionViewModel(configurationBuilder, configuration, saveAction, popupService);

            var model = Substitute.For<IProjectConfiguration>();
            sut.Projects.Add(new ProjectViewModel(model, configuration));

            // Act
            sut.RemoveProjectCommand.Execute(sut.Projects.First());

            // Assert
            Assert.Empty(sut.Projects);
        }

        [Fact]
        public void RequestingSaveShouldCallSaveMethod()
        {
            // Arrange
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();
            var configuration = Substitute.For<IConfiguration>();
            var saveActionExecuted = false;
            Action saveAction = () => { saveActionExecuted = true; };
            var popupService = Substitute.For<IPopupService>();

            var sut = new ProjectsSectionViewModel(configurationBuilder, configuration, saveAction, popupService);

            var model = Substitute.For<IProjectConfiguration>();
            var project = Substitute.For<ProjectViewModel>(model, configuration);

            sut.AddProjectViewModel(project);

            // Act
            project.SaveRequested += Raise.Event();

            // Assert
            Assert.True(saveActionExecuted);
        }

        [Fact]
        public void StoredProjectsShouldBeContainedInProjectList()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            configuration.Projects.Returns(new IProjectConfiguration[]
            {
                new ProjectConfiguration {ProjectName = "p1"},
                new ProjectConfiguration {ProjectName = "p2"}
            });

            var configurationBuilder = Substitute.For<IConfigurationBuilder>();
            Action saveAction = () => { };
            var popupService = Substitute.For<IPopupService>();
            var sut = new ProjectsSectionViewModel(configurationBuilder, configuration, saveAction, popupService);

            // Act
            var actual = sut.Projects.ToList();

            // Assert
            Assert.Collection(actual,
                c => Assert.Equal("p1", c.DisplayNameTextId),
                c => Assert.Equal("p2", c.DisplayNameTextId));
        }
    }
}