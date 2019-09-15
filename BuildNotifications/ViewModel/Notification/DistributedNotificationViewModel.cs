using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfacesLegacy.Notification;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.Notification
{
    public class DistributedNotificationViewModel : BaseViewModel
    {
        public IconType IconType { get; }

        public BuildStatus BuildStatus { get; }

        public List<string> Messages { get; }

        public int Rows => Math.Min(Messages.Count, 3);

        public double Width { get; }

        public double Height { get; }

        public Visibility BigViewVisibility { get; }

        public Visibility SmallViewVisibility { get; }

        public DistributedNotificationViewModel(IDistributedNotification notification)
        {
            IconType = SetIconType(notification);
            BuildStatus = SetBuildStatus(notification);
            Messages = SetMessages(notification);
            var (width, height) = SetWidthAndHeight(notification);
            Width = width;
            Height = height;
            BigViewVisibility = GetBigViewVisibility(notification);
            SmallViewVisibility = BigViewVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private Visibility GetBigViewVisibility(IDistributedNotification notification)
        {
            return notification.NotificationErrorType == DistributedNotificationErrorType.Error ? Visibility.Visible : Visibility.Collapsed;
        }

        private (double Width, double Height) SetWidthAndHeight(IDistributedNotification notification)
        {
            // these are the resolutions that Win 10 will display in the action center
            var width = notification.NotificationErrorType == DistributedNotificationErrorType.Error ? 364 : 334;
            var height = notification.NotificationErrorType == DistributedNotificationErrorType.Error ? 180 : 43;

            return (width, height);
        }

        private List<string> SetMessages(IDistributedNotification notification) => notification.IssueSource.Split('\n').Take(3).ToList();

        private BuildStatus SetBuildStatus(IDistributedNotification notification)
        {
            switch (notification.NotificationErrorType)
            {
                case DistributedNotificationErrorType.Error:
                    return BuildStatus.Failed;
                case DistributedNotificationErrorType.Success:
                    return BuildStatus.Succeeded;
                case DistributedNotificationErrorType.Cancel:
                    return BuildStatus.Cancelled;
                default:
                    return BuildStatus.None;
            }
        }

        private IconType SetIconType(IDistributedNotification notification)
        {
            return notification.NotificationType switch
            {
                DistributedNotificationType.Branch => IconType.Branch,
                DistributedNotificationType.Definition => IconType.Definition,
                DistributedNotificationType.Build => IconType.GroupingSolo,
                DistributedNotificationType.Builds => IconType.BuildNotification,
                DistributedNotificationType.GeneralError => IconType.Lightning,
                DistributedNotificationType.DefinitionAndBranch => IconType.GroupingSolo,
                _ => IconType.Info
            };
        }
    }
}