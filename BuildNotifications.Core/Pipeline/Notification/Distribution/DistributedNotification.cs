using System;
using System.Text;
using Anotar.NLog;
using BuildNotifications.Core.Protocol;
using BuildNotifications.PluginInterfacesLegacy.Notification;
using Newtonsoft.Json;

namespace BuildNotifications.Core.Pipeline.Notification.Distribution
{
    public class DistributedNotification : IDistributedNotification
    {
        public string Content { get; set; } = "";

        public string Title { get; set; } = "";

        public string? ContentImageUrl { get; set; }

        public string? AppIconUrl { get; set; }

        public string? Source { get; set; }

        public string FeedbackArguments { get; set; } = "";

        public uint ColorCode { get; set; } = 0xffffffff;

        public Guid? BasedOnNotification { get; set; }

        public DistributedNotificationErrorType NotificationErrorType { get; set; } = DistributedNotificationErrorType.None;

        public DistributedNotificationType NotificationType { get; set; } = DistributedNotificationType.General;

        public string Serialize()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return ToBase64(serialized);
        }

        public string ToUriProtocol()
        {
            return $"{UriSchemeRegistration.UriScheme}:{Serialize()}";
        }

        public static IDistributedNotification? FromSerialized(string base64)
        {
            try
            {
                LogTo.Debug($"Reading content and deserializing.");
                var content = FromBase64(base64);
                LogTo.Debug($"Successfully converted back from base64.");
                return JsonConvert.DeserializeObject<DistributedNotification>(content);
            }
            catch (Exception e)
            {
                LogTo.ErrorException("Failed to deserialize DistributedNotification.", e);
                return null;
            }
        }

        private static string ToBase64(string source) => Convert.ToBase64String(Encoding.UTF8.GetBytes(source));

        private static string FromBase64(string base64) => Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }
}