using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Services
{
    internal class UpdateNotification : INotification
    {
        public Guid Guid { get; } = Guid.NewGuid();

        public string DisplayContent => StringLocalizer.Instance.GetText(ContentTextId);

        public string DisplayTitle => StringLocalizer.Instance.GetText(TitleTextId);

        public string ContentTextId { get; } = "RestartApplicationToApplyUpdate";

        public string TitleTextId { get; set; } = "AnUpdateHasBeenInstalled";

        public string IssueSource { get; } = "BuildNotifications";

        public string Source { get; set; } = "";

        public NotificationType Type => NotificationType.Info;

        public IList<IBuildNode> BuildNodes => new List<IBuildNode>();

        public BuildStatus Status => BuildStatus.None;
    }
}