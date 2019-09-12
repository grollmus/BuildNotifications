using System;
using System.IO;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using BuildNotifications.PluginInterfacesLegacy.Notification;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ToastNotificationsPlugin
{
    internal class ToastNotificationFactory
    {
        public event EventHandler<ToastEventArgs> ToastActivated;

        public void Process(IDistributedNotification notification)
        {
            var toast = CreateToast(notification);
            PublishToast(toast);
        }

        private void PublishToast(INotificationContent toast)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(toast.GetContent());
            var toastNotification = new ToastNotification(xmlDocument);

            toastNotification.Activated += (sender, args) => ToastActivated?.Invoke(this, new ToastEventArgs(args.ToString()));
            ToastNotificationManager.CreateToastNotifier(ToastNotificationProcessor.ApplicationId).Show(toastNotification);
        }

        private ToastContent CreateToast(IDistributedNotification notification)
        {
            var content = SimpleToastContent(notification);

            AddAppLogo(notification, content);
            AddAttribution(notification, content);
            AddImage(notification, content);

            return content;
        }

        private static ToastContent SimpleToastContent(IDistributedNotification notification)
        {
            var isError = notification.NotificationErrorType == DistributedNotificationErrorType.Error;
            return new ToastContent()
            {
                Launch = notification.FeedbackArguments,
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

        private static void AddAppLogo(IDistributedNotification notification, ToastContent content)
        {
            if (string.IsNullOrEmpty(notification.AppIconUrl) || !File.Exists(notification.AppIconUrl))
                return;

            content.Visual.BindingGeneric.AppLogoOverride = new ToastGenericAppLogo()
            {
                Source = notification.AppIconUrl,
                HintCrop = ToastGenericAppLogoCrop.None
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
                    new ToastGenericHeroImage()
                    {
                        Source = notification.ContentImageUrl
                    };
            }
            else
            {
                content.Visual.BindingGeneric.Children.Add(new AdaptiveImage()
                {
                    Source = notification.ContentImageUrl
                });
            }
        }

        private static void AddAttribution(IDistributedNotification notification, ToastContent content)
        {
            if (string.IsNullOrEmpty(notification.Source))
                return;

            content.Visual.BindingGeneric.Attribution = new ToastGenericAttributionText()
            {
                Text = notification.Source
            };
        }
    }
}