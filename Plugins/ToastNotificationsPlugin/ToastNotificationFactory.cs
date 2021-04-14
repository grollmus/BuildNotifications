using System.Diagnostics.CodeAnalysis;
using System.IO;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using BuildNotifications.PluginInterfaces.Notification;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ToastNotificationsPlugin
{
    [ExcludeFromCodeCoverage]
    internal class ToastNotificationFactory
    {
        public void Process(IDistributedNotification notification)
        {
            var toast = CreateToast(notification);
            PublishToast(toast, NotificationTag(notification));
        }

        private static void AddAppLogo(IDistributedNotification notification, ToastContent content)
        {
            if (string.IsNullOrEmpty(notification.AppIconUrl) || !File.Exists(notification.AppIconUrl))
                return;

            content.Visual.BindingGeneric.AppLogoOverride = new ToastGenericAppLogo
            {
                Source = notification.AppIconUrl,
                HintCrop = ToastGenericAppLogoCrop.None
            };
        }

        private static void AddAttribution(IDistributedNotification notification, ToastContent content)
        {
            if (string.IsNullOrEmpty(notification.Source))
                return;

            content.Visual.BindingGeneric.Attribution = new ToastGenericAttributionText
            {
                Text = notification.Source
            };
        }

        private static void AddImage(IDistributedNotification notification, ToastContent content)
        {
            if (string.IsNullOrEmpty(notification.ContentImageUrl) || !File.Exists(notification.ContentImageUrl))
                return;

            var isError = notification.NotificationErrorType == DistributedNotificationErrorType.Error;

            if (isError)
            {
                content.Visual.BindingGeneric.HeroImage =
                    new ToastGenericHeroImage
                    {
                        Source = notification.ContentImageUrl
                    };
            }
            else
            {
                content.Visual.BindingGeneric.Children.Add(new AdaptiveImage
                {
                    Source = notification.ContentImageUrl
                });
            }
        }

        private ToastContent CreateToast(IDistributedNotification notification)
        {
            var content = SimpleToastContent(notification);

            AddAppLogo(notification, content);
            AddAttribution(notification, content);
            AddImage(notification, content);

            return content;
        }

        private void PublishToast(INotificationContent toast, string tag)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(toast.GetContent());
            var toastNotification = new ToastNotification(xmlDocument) {Tag = tag, Group = ToastNotificationProcessor.Group};

            ToastNotificationManager.CreateToastNotifier(ToastNotificationProcessor.ApplicationId).Show(toastNotification);
        }

        private static ToastContent SimpleToastContent(IDistributedNotification notification)
        {
            var isError = notification.NotificationErrorType == DistributedNotificationErrorType.Error;
            return new ToastContent
            {
                Launch = notification.FeedbackArguments,
                ActivationType = ToastActivationType.Protocol,
                Duration = isError ? ToastDuration.Long : ToastDuration.Short,
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText
                            {
                                Text = notification.Title,
                                HintMaxLines = 1
                            },

                            new AdaptiveText
                            {
                                Text = notification.Content
                            }
                        }
                    }
                }
            };
        }

        public string NotificationTag(IDistributedNotification notification) => notification.BasedOnNotification?.ToString() ?? "";
    }
}