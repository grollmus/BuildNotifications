using System;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.ViewModel.Settings.Setup;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Setup
{
    public class ConnectionsSectionViewModelTests
    {
        [Fact]
        public void AddCommandShouldAddNewConnection()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var pluginRepository = Substitute.For<IPluginRepository>();
            Action saveAction = () => { };

            var sut = new ConnectionsSectionViewModel(configuration, pluginRepository, saveAction);

            // Act
            sut.AddConnectionCommand.Execute(null);

            // Assert
            Assert.Single(sut.Connections);
        }

        [Fact]
        public void RemoveCommandShouldRemoveConnectionFromList()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var pluginRepository = Substitute.For<IPluginRepository>();
            var saveActionExecuted = false;
            Action saveAction = () => { saveActionExecuted = true; };

            var sut = new ConnectionsSectionViewModel(configuration, pluginRepository, saveAction);

            sut.Connections.Add(new ConnectionViewModel(new ConnectionData(), pluginRepository));

            // Act
            sut.RemoveConnectionCommand.Execute(sut.Connections.First());

            // Assert
            Assert.Empty(sut.Connections);
            Assert.True(saveActionExecuted);
        }

        [Fact]
        public void RequestingSaveShouldCallSaveMethod()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var pluginRepository = Substitute.For<IPluginRepository>();
            var saveActionExecuted = false;
            Action saveAction = () => { saveActionExecuted = true; };

            var sut = new ConnectionsSectionViewModel(configuration, pluginRepository, saveAction);

            var connection = Substitute.For<ConnectionViewModel>(new ConnectionData(), pluginRepository);

            sut.AddConnectionViewModel(connection);

            // Act
            connection.SaveRequested += Raise.Event();

            // Assert
            Assert.True(saveActionExecuted);
        }

        [Fact]
        public void StoredConnectionsShouldBeContainedInConnectionList()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            configuration.Connections.Returns(new[]
            {
                new ConnectionData {Name = "c1"},
                new ConnectionData {Name = "c2"}
            });

            var pluginRepository = Substitute.For<IPluginRepository>();
            Action saveAction = () => { };

            var sut = new ConnectionsSectionViewModel(configuration, pluginRepository, saveAction);

            // Act
            var actual = sut.Connections.ToList();

            // Assert
            Assert.Collection(actual,
                c => Assert.Equal("c1", c.Name),
                c => Assert.Equal("c2", c.Name));
        }

        [Fact]
        public void TestFinishedEventShouldBeDispatchedThrough()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var pluginRepository = Substitute.For<IPluginRepository>();
            Action saveAction = () => { };

            var sut = new ConnectionsSectionViewModel(configuration, pluginRepository, saveAction);

            var testConnectionViewModel = Substitute.For<TestConnectionViewModel>(pluginRepository);
            var connection = Substitute.For<ConnectionViewModel>(new ConnectionData(), pluginRepository, testConnectionViewModel);

            sut.SelectedConnection = connection;

            // Act
            var evt = Assert.RaisesAny<EventArgs>(
                e => sut.TestFinished += e,
                e => sut.TestFinished -= e,
                () => connection.TestConnection.TestFinished += Raise.Event());

            // Assert
            Assert.NotNull(evt);
            Assert.Same(sut, evt.Sender);
            Assert.Equal(EventArgs.Empty, evt.Arguments);
        }
    }
}