using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfacesLegacy.Notification;
using BuildNotifications.Resources.BuildTree.Converter;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Notification
{
    public class NotificationDistributor : BaseNotificationDistributor
    {
        private readonly string _assemblyPath;

        public NotificationDistributor()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase!);
            var unescapedDataString = Uri.UnescapeDataString(uri.Path!);
            _assemblyPath = Path.GetDirectoryName(unescapedDataString) ?? "";
        }

        protected override IDistributedNotification ToDistributedNotification(INotification notification)
        {
            var distributedNotification = new DistributedNotification
            {
                Title = notification.DisplayTitle,
                Content = notification.DisplayContent,
                ContentImageUrl = CreateNotificationImage(notification),
                AppIconUrl = AppIconPath(notification.Status),
                NotificationType = ToDistributedNotificationType(notification.Type),
                NotificationErrorType = ToDistributedErrorType(notification.Status),
                Source = notification.BuildNodes.Any() ? notification.BuildNodes.First().Build.ProjectName : null,
                FeedbackArguments = notification.GetType().Name
            };

            var statusToColorConverter = BuildStatusToBrushConverter.Instance;
            var brushFromStatus = statusToColorConverter.Convert(notification.Status) as SolidColorBrush ?? statusToColorConverter.DefaultBrush;

            distributedNotification.ColorCode = brushFromStatus.Color.ToUintColor();

            return distributedNotification;
        }

        private DistributedNotificationErrorType ToDistributedErrorType(BuildStatus notificationStatus)
        {
            return notificationStatus switch
            {
                BuildStatus.Failed => DistributedNotificationErrorType.Error,
                BuildStatus.Succeeded => DistributedNotificationErrorType.Success,
                BuildStatus.PartiallySucceeded => DistributedNotificationErrorType.Success,
                BuildStatus.Cancelled => DistributedNotificationErrorType.Cancel,
                _ => DistributedNotificationErrorType.None,
            };
        }

        private DistributedNotificationType ToDistributedNotificationType(NotificationType notificationType)
        {
            return notificationType switch
            {
                NotificationType.Branch => DistributedNotificationType.Branch,
                NotificationType.Definition => DistributedNotificationType.Definition,
                NotificationType.DefinitionAndBranch => DistributedNotificationType.DefinitionAndBranch,
                NotificationType.Build => DistributedNotificationType.Build,
                NotificationType.Error => DistributedNotificationType.GeneralError,
                _ => DistributedNotificationType.General
            };
        }

        private string? AppIconPath(BuildStatus forBuildStatus)
        {
            return forBuildStatus switch
            {
                BuildStatus.Succeeded => ToAbsolute("/Resources/Icons/Green.ico"),
                BuildStatus.PartiallySucceeded => ToAbsolute("/Resources/Icons/Green.ico"),
                BuildStatus.Failed => ToAbsolute("/Resources/Icons/Red.ico"),
                _ => ToAbsolute("/Resources/Icons/Gray.ico")
            };
        }

        private string? CreateNotificationImage(INotification notification)
        {
            const string heroPlaceholderPath = "/Resources/Icons/ToastHeroPlaceholder.png";
            const string badgePlaceholderPath = "/Resources/Icons/ToastImagePlaceholder.png";
            return notification.Status == BuildStatus.Failed ? ToAbsolute(heroPlaceholderPath) : ToAbsolute(badgePlaceholderPath);
        }

        private string? ToAbsolute(string relativePath)
        {
            relativePath = relativePath.Replace('/', '\\');
            var absolutePath = $"{_assemblyPath}{relativePath}";
            return File.Exists(absolutePath) ? absolutePath : null;
        }
    }
}