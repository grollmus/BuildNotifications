using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Notification
{
    public class NotificationFactoryTests
    {
        public NotificationFactoryTests()
        {
            _ciDefinition = Substitute.For<IBuildDefinition>();
            _ciDefinition.Name.Returns(Ci);

            _nightlyDefinition = Substitute.For<IBuildDefinition>();
            _nightlyDefinition.Name.Returns(Nightly);

            _cloudDefinition = Substitute.For<IBuildDefinition>();
            _cloudDefinition.Name.Returns(Cloud);

            _mobileDefinition = Substitute.For<IBuildDefinition>();
            _mobileDefinition.Name.Returns(Mobile);

            _stageBranch = Substitute.For<IBranch>();
            _stageBranch.FullName.Returns(Stage);

            _masterBranch = Substitute.For<IBranch>();
            _masterBranch.FullName.Returns(Master);

            _featureBranch = Substitute.For<IBranch>();
            _featureBranch.FullName.Returns(Feature);

            _bugBranch = Substitute.For<IBranch>();
            _bugBranch.FullName.Returns(Bug);

            _longNameFeatureABranch = Substitute.For<IBranch>();
            _longNameFeatureABranch.FullName.Returns(LongNameFeatureA);

            _longNameFeatureBBranch = Substitute.For<IBranch>();
            _longNameFeatureBBranch.FullName.Returns(LongNameFeatureB);

            _longNameFeatureCBranch = Substitute.For<IBranch>();
            _longNameFeatureCBranch.FullName.Returns(LongNameFeatureC);

            _me = Substitute.For<IUser>();
            _me.UniqueName.Returns("Me");
            _me.Id.Returns("Me");

            _someoneElse = Substitute.For<IUser>();
            _someoneElse.UniqueName.Returns("SomeoneElse");
            _someoneElse.Id.Returns("SomeoneElse");

            _allowAllConfiguration = Substitute.For<IConfiguration>();
            _allowAllConfiguration.CanceledBuildNotifyConfig.Returns(BuildNotificationModes.Always);
            _allowAllConfiguration.FailedBuildNotifyConfig.Returns(BuildNotificationModes.Always);
            _allowAllConfiguration.SucceededBuildNotifyConfig.Returns(BuildNotificationModes.Always);

            _onlyRequestedByMeConfiguration = Substitute.For<IConfiguration>();
            _onlyRequestedByMeConfiguration.CanceledBuildNotifyConfig.Returns(BuildNotificationModes.RequestedByMe);
            _onlyRequestedByMeConfiguration.FailedBuildNotifyConfig.Returns(BuildNotificationModes.RequestedByMe);
            _onlyRequestedByMeConfiguration.SucceededBuildNotifyConfig.Returns(BuildNotificationModes.RequestedByMe);

            _onlyRequestedForMeConfiguration = Substitute.For<IConfiguration>();
            _onlyRequestedForMeConfiguration.CanceledBuildNotifyConfig.Returns(BuildNotificationModes.RequestedByOrForMe);
            _onlyRequestedForMeConfiguration.FailedBuildNotifyConfig.Returns(BuildNotificationModes.RequestedByOrForMe);
            _onlyRequestedForMeConfiguration.SucceededBuildNotifyConfig.Returns(BuildNotificationModes.RequestedByOrForMe);

            _dontNotifyConfiguration = Substitute.For<IConfiguration>();
            _dontNotifyConfiguration.CanceledBuildNotifyConfig.Returns(BuildNotificationModes.None);
            _dontNotifyConfiguration.FailedBuildNotifyConfig.Returns(BuildNotificationModes.None);
            _dontNotifyConfiguration.SucceededBuildNotifyConfig.Returns(BuildNotificationModes.None);

            _treatPartialsAsSucceededConfiguration = Substitute.For<IConfiguration>();
            _treatPartialsAsSucceededConfiguration.CanceledBuildNotifyConfig.Returns(BuildNotificationModes.Always);
            _treatPartialsAsSucceededConfiguration.FailedBuildNotifyConfig.Returns(BuildNotificationModes.Always);
            _treatPartialsAsSucceededConfiguration.SucceededBuildNotifyConfig.Returns(BuildNotificationModes.Always);
            _treatPartialsAsSucceededConfiguration.PartialSucceededTreatmentMode.Returns(PartialSucceededTreatmentMode.TreatAsSucceeded);

            _treatPartialsAsFailedConfiguration = Substitute.For<IConfiguration>();
            _treatPartialsAsFailedConfiguration.CanceledBuildNotifyConfig.Returns(BuildNotificationModes.Always);
            _treatPartialsAsFailedConfiguration.FailedBuildNotifyConfig.Returns(BuildNotificationModes.Always);
            _treatPartialsAsFailedConfiguration.SucceededBuildNotifyConfig.Returns(BuildNotificationModes.Always);
            _treatPartialsAsFailedConfiguration.PartialSucceededTreatmentMode.Returns(PartialSucceededTreatmentMode.TreatAsFailed);

            _userIdentityList = Substitute.For<IUserIdentityList>();
            _userIdentityList.IdentitiesOfCurrentUser.Returns(new List<IUser> {_me});
        }

        private readonly IBuildDefinition _ciDefinition;
        private const string Ci = nameof(Ci);

        private readonly IBuildDefinition _nightlyDefinition;
        private const string Nightly = nameof(Nightly);

        private readonly IBuildDefinition _cloudDefinition;
        private const string Cloud = nameof(Cloud);

        private readonly IBuildDefinition _mobileDefinition;
        private const string Mobile = nameof(Mobile);

        private readonly IBranch _stageBranch;
        private const string Stage = nameof(Stage);

        private readonly IBranch _masterBranch;
        private const string Master = nameof(Master);

        private readonly IBranch _featureBranch;
        private const string Feature = nameof(Feature);

        private readonly IBranch _longNameFeatureABranch;
        private const string LongNameFeatureA = LongFeatureNameBase + "FeatureA";

        private readonly IBranch _longNameFeatureBBranch;
        private const string LongNameFeatureB = LongFeatureNameBase + "FeatureB";

        private readonly IBranch _longNameFeatureCBranch;
        private const string LongNameFeatureC = LongFeatureNameBase + "Something";

        private const string LongFeatureNameBase = "Feature/Team1";

        private readonly IBranch _bugBranch;
        private const string Bug = nameof(Bug);

        private readonly IUser _me;

        private readonly IUser _someoneElse;

        private readonly IUserIdentityList _userIdentityList;

        private readonly IConfiguration _allowAllConfiguration;
        private readonly IConfiguration _onlyRequestedByMeConfiguration;
        private readonly IConfiguration _onlyRequestedForMeConfiguration;
        private readonly IConfiguration _dontNotifyConfiguration;
        private readonly IConfiguration _treatPartialsAsSucceededConfiguration;
        private readonly IConfiguration _treatPartialsAsFailedConfiguration;

        private const string ProjectId = nameof(ProjectId);

        private void RequestedByMe(IBuildNode build)
        {
            build.Build.RequestedBy.Returns(_me);
        }

        private void RequestedForMe(IBuildNode build)
        {
            build.Build.RequestedFor.Returns(_me);
        }

        private IBuildNode CreateBuildNode(IBuildDefinition definition, IBranch branch, string id, BuildStatus status)
        {
            var build = Substitute.For<IBuild>();
            build.Definition.Returns(definition);
            var branchName = branch.FullName;
            build.BranchName.Returns(branchName);
            build.Id.Returns(id);
            build.Status.Returns(status);
            build.RequestedBy.Returns(_someoneElse);
            build.RequestedFor.Returns(_someoneElse);
            build.ProjectName.Returns(ProjectId);

            var node = new BuildNode(build);

            return node;
        }

        [Fact]
        public void AllBuildsFromFourDefinitionsButSameBranchShouldResultInMessageTellingAboutBranch()
        {
            // arrange
            var build1 = CreateBuildNode(_ciDefinition, _masterBranch, "1", BuildStatus.Failed);
            var build2 = CreateBuildNode(_mobileDefinition, _masterBranch, "2", BuildStatus.Failed);
            var build3 = CreateBuildNode(_nightlyDefinition, _masterBranch, "3", BuildStatus.Failed);
            var build4 = CreateBuildNode(_cloudDefinition, _masterBranch, "4", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build1);
            delta.FailedBuilds.Add(build2);
            delta.FailedBuilds.Add(build3);
            delta.FailedBuilds.Add(build4);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, BranchNotification.BranchChangedTextId);
            Assert.True(message.DisplayContent.Contains(_masterBranch.FullName, StringComparison.Ordinal));
        }

        [Fact]
        public void AllBuildsFromSameDefinitionsAndBranchesShouldResultInMessageTellingAboutDefinitionAndBranch()
        {
            // arrange
            var build1 = CreateBuildNode(_ciDefinition, _stageBranch, "1", BuildStatus.Failed);
            var build2 = CreateBuildNode(_ciDefinition, _stageBranch, "2", BuildStatus.Failed);
            var build3 = CreateBuildNode(_ciDefinition, _stageBranch, "3", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build1);
            delta.FailedBuilds.Add(build2);
            delta.FailedBuilds.Add(build3);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, DefinitionAndBranchNotification.BranchAndDefinitionFailedTextId);
            Assert.True(message.DisplayContent.Contains(_ciDefinition.Name, StringComparison.Ordinal));
            Assert.True(message.DisplayContent.Contains(_stageBranch.FullName, StringComparison.Ordinal));
        }

        [Fact]
        public void AllBuildsFromSameDefinitionShouldResultInMessageTellingAboutDefinition()
        {
            // arrange
            var build1 = CreateBuildNode(_ciDefinition, _stageBranch, "1", BuildStatus.Failed);
            var build2 = CreateBuildNode(_ciDefinition, _masterBranch, "2", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build1);
            delta.FailedBuilds.Add(build2);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, DefinitionNotification.DefinitionChangedTextId);
            Assert.True(message.DisplayContent.Contains(_ciDefinition.Name, StringComparison.Ordinal));
        }

        [Fact]
        public void AllBuildsFromThreeBranchesShouldResultInMessageTellingAboutBranch()
        {
            // arrange
            var build1 = CreateBuildNode(_ciDefinition, _masterBranch, "1", BuildStatus.Failed);
            var build2 = CreateBuildNode(_mobileDefinition, _masterBranch, "2", BuildStatus.Failed);
            var build3 = CreateBuildNode(_nightlyDefinition, _stageBranch, "3", BuildStatus.Failed);
            var build4 = CreateBuildNode(_cloudDefinition, _featureBranch, "4", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build1);
            delta.FailedBuilds.Add(build2);
            delta.FailedBuilds.Add(build3);
            delta.FailedBuilds.Add(build4);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, BranchNotification.ThreeBranchesChangedTextId);
            Assert.True(message.DisplayContent.Contains(_masterBranch.FullName, StringComparison.Ordinal));
            Assert.True(message.DisplayContent.Contains(_stageBranch.FullName, StringComparison.Ordinal));
            Assert.True(message.DisplayContent.Contains(_featureBranch.FullName, StringComparison.Ordinal));
        }

        [Fact]
        public void AllBuildsFromThreeDefinitionsShouldResultInMessageTellingAboutDefinition()
        {
            // arrange
            var build1 = CreateBuildNode(_ciDefinition, _stageBranch, "1", BuildStatus.Failed);
            var build2 = CreateBuildNode(_ciDefinition, _masterBranch, "2", BuildStatus.Failed);
            var build3 = CreateBuildNode(_nightlyDefinition, _stageBranch, "3", BuildStatus.Failed);
            var build4 = CreateBuildNode(_cloudDefinition, _featureBranch, "4", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build1);
            delta.FailedBuilds.Add(build2);
            delta.FailedBuilds.Add(build3);
            delta.FailedBuilds.Add(build4);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, DefinitionNotification.ThreeDefinitionsChangedTextId);
            Assert.True(message.DisplayContent.Contains(_ciDefinition.Name, StringComparison.Ordinal));
            Assert.True(message.DisplayContent.Contains(_nightlyDefinition.Name, StringComparison.Ordinal));
            Assert.True(message.DisplayContent.Contains(_cloudDefinition.Name, StringComparison.Ordinal));
        }

        [Fact]
        public void AllBuildsFromTwoBranchesShouldResultInMessageTellingAboutBranch()
        {
            // arrange
            var build1 = CreateBuildNode(_ciDefinition, _masterBranch, "1", BuildStatus.Failed);
            var build2 = CreateBuildNode(_mobileDefinition, _masterBranch, "2", BuildStatus.Failed);
            var build3 = CreateBuildNode(_nightlyDefinition, _stageBranch, "3", BuildStatus.Failed);
            var build4 = CreateBuildNode(_cloudDefinition, _stageBranch, "4", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build1);
            delta.FailedBuilds.Add(build2);
            delta.FailedBuilds.Add(build3);
            delta.FailedBuilds.Add(build4);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, BranchNotification.TwoBranchesChangedTextId);
            Assert.True(message.DisplayContent.Contains(_masterBranch.FullName, StringComparison.Ordinal));
            Assert.True(message.DisplayContent.Contains(_stageBranch.FullName, StringComparison.Ordinal));
        }

        [Fact]
        public void AllBuildsFromTwoDefinitionsButSameBranchShouldResultInMessageTellingAboutBranch()
        {
            // arrange
            var build1 = CreateBuildNode(_ciDefinition, _stageBranch, "1", BuildStatus.Failed);
            var build2 = CreateBuildNode(_nightlyDefinition, _stageBranch, "2", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build1);
            delta.FailedBuilds.Add(build2);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, BranchNotification.BranchChangedTextId);
            Assert.True(message.DisplayContent.Contains(_stageBranch.FullName, StringComparison.Ordinal));
        }

        [Fact]
        public void AllBuildsFromTwoDefinitionsShouldResultInMessageTellingAboutDefinition()
        {
            // arrange
            var build1 = CreateBuildNode(_ciDefinition, _stageBranch, "1", BuildStatus.Failed);
            var build2 = CreateBuildNode(_ciDefinition, _masterBranch, "2", BuildStatus.Failed);
            var build3 = CreateBuildNode(_nightlyDefinition, _stageBranch, "3", BuildStatus.Failed);
            var build4 = CreateBuildNode(_nightlyDefinition, _stageBranch, "4", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build1);
            delta.FailedBuilds.Add(build2);
            delta.FailedBuilds.Add(build3);
            delta.FailedBuilds.Add(build4);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, DefinitionNotification.TwoDefinitionsChangedTextId, StringComparer.Ordinal);
            Assert.True(message.DisplayContent.Contains(_ciDefinition.Name, StringComparison.Ordinal));
            Assert.True(message.DisplayContent.Contains(_nightlyDefinition.Name, StringComparison.Ordinal));
        }

        [Fact]
        public void BranchNamesShouldGetTruncated()
        {
            // arrange
            var build1 = CreateBuildNode(_ciDefinition, _longNameFeatureABranch, "1", BuildStatus.Failed);
            var build2 = CreateBuildNode(_mobileDefinition, _longNameFeatureBBranch, "2", BuildStatus.Failed);
            var build3 = CreateBuildNode(_nightlyDefinition, _longNameFeatureCBranch, "3", BuildStatus.Failed);
            var build4 = CreateBuildNode(_cloudDefinition, _longNameFeatureABranch, "4", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build1);
            delta.FailedBuilds.Add(build2);
            delta.FailedBuilds.Add(build3);
            delta.FailedBuilds.Add(build4);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, BranchNotification.ThreeBranchesChangedTextId);
            Assert.False(message.DisplayContent.Contains(LongFeatureNameBase, StringComparison.Ordinal));
        }

        [Fact]
        public void BuildsFromFourDefinitionsAndFourBranchesShouldResultInMessageTellingAboutBuilds()
        {
            // arrange
            var build1 = CreateBuildNode(_ciDefinition, _stageBranch, "1", BuildStatus.Failed);
            var build2 = CreateBuildNode(_mobileDefinition, _masterBranch, "2", BuildStatus.Failed);
            var build3 = CreateBuildNode(_nightlyDefinition, _bugBranch, "3", BuildStatus.Failed);
            var build4 = CreateBuildNode(_cloudDefinition, _featureBranch, "4", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build1);
            delta.FailedBuilds.Add(build2);
            delta.FailedBuilds.Add(build3);
            delta.FailedBuilds.Add(build4);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, BuildNotification.BuildsChangedTextId);
        }

        [Fact]
        public void BuildWithManualNotificationShouldProduceNotification()
        {
            // Arrange
            var build = CreateBuildNode(_ciDefinition, _stageBranch, "1", BuildStatus.Succeeded);
            build.IsManualNotificationEnabled = true;
            var delta = new BuildTreeBuildsDelta();
            delta.SucceededBuilds.Add(build);

            // Act
            var messages = new NotificationFactory(_dontNotifyConfiguration, _userIdentityList).ProduceNotifications(delta);

            // Assert
            Assert.Single(messages);
        }

        [Theory]
        [InlineData(BuildStatus.Failed)]
        [InlineData(BuildStatus.Succeeded)]
        [InlineData(BuildStatus.Cancelled)]
        public void MessageIsNotProducedForBuildRequestedBySomeoneElseWhenSettingIsOn(BuildStatus status)
        {
            // arrange
            var build = CreateBuildNode(_ciDefinition, _stageBranch, "1", status);
            var delta = new BuildTreeBuildsDelta();

            switch (status)
            {
                case BuildStatus.Failed:
                    delta.FailedBuilds.Add(build);
                    break;
                case BuildStatus.Succeeded:
                    delta.SucceededBuilds.Add(build);
                    break;
                default:
                    delta.CancelledBuilds.Add(build);
                    break;
            }

            // act
            var messages = new NotificationFactory(_onlyRequestedByMeConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            Assert.False(messages.Any());
        }

        [Theory]
        [InlineData(BuildStatus.Failed)]
        [InlineData(BuildStatus.Succeeded)]
        [InlineData(BuildStatus.Cancelled)]
        public void MessageIsNotProducedForBuildRequestedForSomeoneElseWhenSettingIsOn(BuildStatus status)
        {
            // arrange
            var build = CreateBuildNode(_ciDefinition, _stageBranch, "1", status);
            var delta = new BuildTreeBuildsDelta();

            switch (status)
            {
                case BuildStatus.Failed:
                    delta.FailedBuilds.Add(build);
                    break;
                case BuildStatus.Succeeded:
                    delta.SucceededBuilds.Add(build);
                    break;
                default:
                    delta.CancelledBuilds.Add(build);
                    break;
            }

            // act
            var messages = new NotificationFactory(_onlyRequestedForMeConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            Assert.False(messages.Any());
        }

        [Theory]
        [InlineData(BuildStatus.Failed)]
        [InlineData(BuildStatus.Succeeded)]
        [InlineData(BuildStatus.Cancelled)]
        public void MessageIsNotProducedForBuildWhenSettingDictatesNone(BuildStatus status)
        {
            // arrange
            var build = CreateBuildNode(_ciDefinition, _stageBranch, "1", status);
            RequestedByMe(build);
            RequestedForMe(build);
            var delta = new BuildTreeBuildsDelta();

            switch (status)
            {
                case BuildStatus.Failed:
                    delta.FailedBuilds.Add(build);
                    break;
                case BuildStatus.Succeeded:
                    delta.SucceededBuilds.Add(build);
                    break;
                default:
                    delta.CancelledBuilds.Add(build);
                    break;
            }

            // act
            var messages = new NotificationFactory(_dontNotifyConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            Assert.False(messages.Any());
        }

        [Theory]
        [InlineData(BuildStatus.Failed)]
        [InlineData(BuildStatus.Succeeded)]
        [InlineData(BuildStatus.Cancelled)]
        public void MessageIsProducedForBuildRequestedByMeWhenSettingIsOn(BuildStatus status)
        {
            // arrange
            var build = CreateBuildNode(_ciDefinition, _stageBranch, "1", status);
            RequestedByMe(build);
            var delta = new BuildTreeBuildsDelta();

            switch (status)
            {
                case BuildStatus.Failed:
                    delta.FailedBuilds.Add(build);
                    break;
                case BuildStatus.Succeeded:
                    delta.SucceededBuilds.Add(build);
                    break;
                default:
                    delta.CancelledBuilds.Add(build);
                    break;
            }

            // act
            var messages = new NotificationFactory(_onlyRequestedByMeConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            Assert.True(messages.Any());
        }

        [Theory]
        [InlineData(BuildStatus.Failed)]
        [InlineData(BuildStatus.Succeeded)]
        [InlineData(BuildStatus.Cancelled)]
        public void MessageIsProducedForBuildRequestedForMeWhenSettingIsOn(BuildStatus status)
        {
            // arrange
            var build = CreateBuildNode(_ciDefinition, _stageBranch, "1", status);
            RequestedByMe(build);
            RequestedForMe(build);
            var delta = new BuildTreeBuildsDelta();

            switch (status)
            {
                case BuildStatus.Failed:
                    delta.FailedBuilds.Add(build);
                    break;
                case BuildStatus.Succeeded:
                    delta.SucceededBuilds.Add(build);
                    break;
                default:
                    delta.CancelledBuilds.Add(build);
                    break;
            }

            // act
            var messages = new NotificationFactory(_onlyRequestedForMeConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            Assert.True(messages.Any());
        }

        [Theory]
        [InlineData(BuildStatus.Failed)]
        [InlineData(BuildStatus.Succeeded)]
        [InlineData(BuildStatus.Cancelled)]
        public void MessageIsProducedForBuildWhenSettingDictatesAlways(BuildStatus status)
        {
            // arrange
            var build = CreateBuildNode(_ciDefinition, _stageBranch, "1", status);
            var delta = new BuildTreeBuildsDelta();

            switch (status)
            {
                case BuildStatus.Failed:
                    delta.FailedBuilds.Add(build);
                    break;
                case BuildStatus.Succeeded:
                    delta.SucceededBuilds.Add(build);
                    break;
                default:
                    delta.CancelledBuilds.Add(build);
                    break;
            }

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            Assert.True(messages.Any());
        }

        [Fact]
        public void NoBuildChangingShouldResultInNoMessage()
        {
            // arrange
            var delta = new BuildTreeBuildsDelta();

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            Assert.Empty(messages);
        }

        [Fact]
        public void PartiallySucceededBuildWithSettingOnTreatAsFailedShouldResultInFailMessage()
        {
            // arrange
            var build = CreateBuildNode(_ciDefinition, _stageBranch, "1", BuildStatus.PartiallySucceeded);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build);

            // act
            var messages = new NotificationFactory(_treatPartialsAsFailedConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, BuildNotification.BuildChangedTextId);
            Assert.True(message.DisplayContent.Contains(_ciDefinition.Name, StringComparison.Ordinal));
            Assert.True(message.DisplayContent.Contains(_stageBranch.FullName, StringComparison.Ordinal));
            Assert.Equal(BuildStatus.Failed, message.Status);
        }

        [Fact]
        public void PartiallySucceededBuildWithSettingOnTreatAsSucceededShouldResultInSuccessMessage()
        {
            // arrange
            var build = CreateBuildNode(_ciDefinition, _stageBranch, "1", BuildStatus.PartiallySucceeded);
            var delta = new BuildTreeBuildsDelta();
            delta.SucceededBuilds.Add(build);

            // act
            var messages = new NotificationFactory(_treatPartialsAsSucceededConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, BuildNotification.BuildChangedTextId);
            Assert.True(message.DisplayContent.Contains(_ciDefinition.Name, StringComparison.Ordinal));
            Assert.True(message.DisplayContent.Contains(_stageBranch.FullName, StringComparison.Ordinal));
            Assert.Equal(BuildStatus.Succeeded, message.Status);
        }

        [Fact]
        public void SingleBuildFailingShouldResultInMessageTellingAboutBuild()
        {
            // arrange
            var build = CreateBuildNode(_ciDefinition, _stageBranch, "1", BuildStatus.Failed);
            var delta = new BuildTreeBuildsDelta();
            delta.FailedBuilds.Add(build);

            // act
            var messages = new NotificationFactory(_allowAllConfiguration, _userIdentityList).ProduceNotifications(delta);

            // assert
            var message = messages.First();
            Assert.Equal(message.ContentTextId, BuildNotification.BuildChangedTextId);
            Assert.True(message.DisplayContent.Contains(_ciDefinition.Name, StringComparison.Ordinal));
            Assert.True(message.DisplayContent.Contains(_stageBranch.FullName, StringComparison.Ordinal));
        }
    }
}