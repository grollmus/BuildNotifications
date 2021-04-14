using System;
using System.Text;
using BuildNotifications.Core.Protocol;
using BuildNotifications.PluginInterfaces.Notification;
using Newtonsoft.Json;
using NLog.Fluent;

namespace BuildNotifications.Core.Pipeline.Notification.Distribution
{
    public class DistributedNotification : IDistributedNotification
    {
        public static IDistributedNotification? FromSerialized(string base64)
        {
            try
            {
                Log.Debug().Message("Reading content and deserializing.").Write();
                var content = FromBase64(base64);
                Log.Debug().Message("Successfully converted back from base64.").Write();
                return JsonConvert.DeserializeObject<DistributedNotification>(content);
            }
            catch (Exception e)
            {
                Log.Error().Message("Failed to deserialize DistributedNotification.").Exception(e).Write();
                return null;
            }
        }

        public string Serialize()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return ToBase64(serialized);
        }

        public string ToUriProtocol() => $"{UriSchemeRegistration.UriScheme}:{Serialize()}";

        private static string FromBase64(string base64) => Encoding.UTF8.GetString(Convert.FromBase64String(base64));

        private static string ToBase64(string source) => Convert.ToBase64String(Encoding.UTF8.GetBytes(source));

        public uint ColorCode { get; set; } = 0xffffffff;

        public string Content { get; set; } = "";

        public string Title { get; set; } = "";

        public string? ContentImageUrl { get; set; }

        public string? AppIconUrl { get; set; }

        public string? Source { get; set; }

        public string FeedbackArguments { get; set; } = "";

        public string IssueSource { get; set; } = "";

        public Guid? BasedOnNotification { get; set; }

        public DistributedNotificationErrorType NotificationErrorType { get; set; } = DistributedNotificationErrorType.None;

        public DistributedNotificationType NotificationType { get; set; } = DistributedNotificationType.General;
    }
}