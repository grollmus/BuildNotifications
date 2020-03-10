using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.TestMocks;
using BuildNotifications.ViewModel.Settings;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings
{
    public class SettingsViewModelTests
    {
        public static TheoryData OptionsSetters
        {
            get
            {
                return new TheoryData<Action<SettingsViewModel>>
                {
                    vm => vm.AnimationsMode.Value = AnimationMode.DoubleSpeed,
                    vm => vm.AutoStartMode.Value = AutostartMode.StartWithWindows,
                    vm => vm.BuildsPerGroup.Value = 12,
                    vm => vm.CanceledBuildNotify.Value = BuildNotificationModes.Always,
                    vm => vm.FailedBuildNotify.Value = BuildNotificationModes.Always,
                    vm => vm.Language.Value = vm.Language.AvailableValues.Last().Value,
                    vm => vm.PartialSucceededTreatmentMode.Value = PartialSucceededTreatmentMode.TreatAsFailed,
                    vm => vm.ShowBusyIndicatorDuringUpdate.Value = true,
                    vm => vm.SucceededBuildNotify.Value = BuildNotificationModes.Always,
                    vm => vm.UpdateInterval.Value = 12,
                    vm => vm.UpdateToPreReleases.Value = true
                };
            }
        }

        [Theory]
        [MemberData(nameof(OptionsSetters))]
        public void ChangingAnyOptionShouldCallSaveMethod(Action<SettingsViewModel> optionSetter)
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var userIdentityList = Substitute.For<IUserIdentityList>();

            var saveCalled = false;
            Action saveMethod = () => saveCalled = true;
            var sut = new SettingsViewModel(configuration, saveMethod, userIdentityList);

            // Act
            optionSetter(sut);

            // Assert
            Assert.True(saveCalled);
        }

        [Fact]
        public void EditConnectionCommandShouldRaiseEvent()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var userIdentityList = Substitute.For<IUserIdentityList>();
            Action saveMethod = () => { };
            var sut = new SettingsViewModel(configuration, saveMethod, userIdentityList);

            // Act
            var evt = Assert.RaisesAny<EventArgs>(
                e => sut.EditConnectionsRequested += e,
                e => sut.EditConnectionsRequested -= e,
                () => sut.EditConnectionsCommand.Execute(null));

            // Assert
            Assert.NotNull(evt);
            Assert.Same(sut, evt.Sender);
            Assert.Equal(EventArgs.Empty, evt.Arguments);
        }

        [Fact]
        public void UpdateUsersShouldSynchronizeUserIdentities()
        {
            // Arrange
            var configuration = Substitute.For<IConfiguration>();
            var userIdentityList = Substitute.For<IUserIdentityList>();
            Action saveMethod = () => { };
            var sut = new SettingsViewModel(configuration, saveMethod, userIdentityList);

            ICollection<IUser> expectedUsers = new List<IUser>();
            expectedUsers.Add(new MockUser("123", "User123", "U123"));
            expectedUsers.Add(new MockUser("234", "User234", "U234"));
            userIdentityList.IdentitiesOfCurrentUser.Returns(expectedUsers);

            // Act
            sut.UpdateUser();

            // Assert
            Assert.Equal(expectedUsers, sut.CurrentUserIdentities.Select(u => u.User));
        }
    }
}