using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Notification.Distribution;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Notification;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Notification.Distribution;

[SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public class BaseNotificationDistributorTests
{
    private class TestNotificationDistributor : BaseNotificationDistributor
    {
        protected override IDistributedNotification ToDistributedNotification(INotification notification)
        {
            var distributedNotification = Substitute.For<IDistributedNotification>();
            return distributedNotification;
        }
    }

    private class TestNotification : INotification
    {
        public TestNotification()
        {
            ContentTextId = "contentTextId";
            DisplayContent = "displayContent";
            DisplayTitle = "displayTitle";
            Guid = Guid.NewGuid();
            IssueSource = "issueSource";
            Source = "source";
            Status = BuildStatus.None;
            TitleTextId = "titleTextId";
            Type = NotificationType.Info;
        }

        public IList<IBuildNode> BuildNodes { get; } = new List<IBuildNode>();
        public string ContentTextId { get; }
        public string DisplayContent { get; }
        public string DisplayTitle { get; }
        public Guid Guid { get; }
        public string IssueSource { get; }
        public string Source { get; }
        public BuildStatus Status { get; }
        public string TitleTextId { get; }
        public NotificationType Type { get; }
    }

    [Fact]
    public void AddedProcessorShouldBeContainedInDistributor()
    {
        // Arrange
        var sut = new TestNotificationDistributor();
        var processor = Substitute.For<INotificationProcessor>();

        // Act
        sut.Add(processor);

        // Assert
        Assert.Single(sut);
        Assert.Same(processor, sut.Single());
    }

    [Fact]
    public void AddShouldInitializeProcessor()
    {
        // Arrange
        var sut = new TestNotificationDistributor();
        var processor = Substitute.For<INotificationProcessor>();

        // Act
        sut.Add(processor);

        // Assert
        processor.Received(1).Initialize();
    }

    [Fact]
    public void AddShouldNotThrowWhenProcessorInitializeThrows()
    {
        // Arrange
        var sut = new TestNotificationDistributor();
        var processor = Substitute.For<INotificationProcessor>();
        processor.When(x => x.Initialize()).Do(_ => throw new Exception());

        // Act
        var ex = Record.Exception(() => sut.Add(processor));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void ClearShouldNotThrowWhenProcessorShutdownThrows()
    {
        // Arrange
        var sut = new TestNotificationDistributor();
        var processor = Substitute.For<INotificationProcessor>();
        processor.When(x => x.Shutdown()).Do(_ => throw new Exception());
        sut.Add(processor);

        // Act
        var ex = Record.Exception(() => sut.Clear());

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void ClearShouldShutdownProcessors()
    {
        // Arrange
        var sut = new TestNotificationDistributor();
        var processor = Substitute.For<INotificationProcessor>();
        sut.Add(processor);

        // Act
        sut.Clear();

        // Assert
        processor.Received(1).Shutdown();
    }

    [Fact]
    public void DistributeShouldCallRegisteredProcessors()
    {
        // Arrange
        var sut = new TestNotificationDistributor();

        var processor = Substitute.For<INotificationProcessor>();
        sut.Add(processor);

        var notification = new TestNotification();

        // Act
        sut.Distribute(notification);

        // Assert
        processor.Received(1).Process(Arg.Any<IDistributedNotification>());
    }

    [Fact]
    public void RemoveShouldReturnFalseWhenProcessorIsNotContained()
    {
        // Arrange
        var sut = new TestNotificationDistributor();
        var processor = Substitute.For<INotificationProcessor>();

        // Act
        var actual = sut.Remove(processor);

        // Assert
        Assert.False(actual);
    }
}