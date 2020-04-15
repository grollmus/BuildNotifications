using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification
{
    public class StatusNotification : INotification
    {
        public StatusNotification(string messageTextId, string titleTextId, NotificationType notificationType, params object[] parameter)
        {
            _parameter = parameter;
            ContentTextId = messageTextId;
            TitleTextId = titleTextId;
            Type = notificationType;
        }

        public string DisplayContent => string.Format(StringLocalizer.CurrentCulture, StringLocalizer.Instance.GetText(ContentTextId), _parameter);

        public string ContentTextId { get; }

        public string DisplayTitle => StringLocalizer.Instance.GetText(TitleTextId);

        public string TitleTextId { get; }

        public NotificationType Type { get; }

        public IList<IBuildNode> BuildNodes => new List<IBuildNode>();

        public string Source { get; } = "";

        public BuildStatus Status => Type == NotificationType.Success ? BuildStatus.Succeeded : BuildStatus.Running;

        public Guid Guid { get; } = Guid.NewGuid();

        public string IssueSource { get; } = "BuildNotifications";

        private readonly object[] _parameter;
    }
}