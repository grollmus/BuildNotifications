﻿using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification
{
    public abstract class BaseBuildNotification : INotification
    {
        public const string FailedSingular = nameof(FailedSingular);
        public const string FailedPlural = nameof(FailedPlural);

        public const string SucceededSingular = nameof(SucceededSingular);
        public const string SucceededPlural = nameof(SucceededPlural);

        public const string CancelledSingular = nameof(CancelledSingular);
        public const string CancelledPlural = nameof(CancelledPlural);

        // One build.
        private const string BuildNotificationContentSingularTextId = nameof(BuildNotificationContentSingularTextId);

        // {0} builds. E.g. 25 builds
        private const string BuildNotificationContentPluralTextId = nameof(BuildNotificationContentPluralTextId);

        public string DisplayContent => string.Format(StringLocalizer.Instance.GetText(ContentTextId), Parameters.ToArray());

        public string ContentTextId => GetMessageTextId();

        public string DisplayTitle => string.Format(StringLocalizer.Instance.GetText(TitleTextId), new object[] {StatusTextId(BuildNodes.Count == 1), BuildNodes.Count.ToString()});

        public string TitleTextId => BuildNodes.Count == 1 ? BuildNotificationContentSingularTextId : BuildNotificationContentPluralTextId;

        public NotificationType Type { get; }

        public IList<IBuildNode> BuildNodes { get; }

        public BuildStatus Status { get; }

        protected List<string> Parameters { get; } = new List<string>();

        protected BaseBuildNotification(NotificationType type, IList<IBuildNode> buildNodes, BuildStatus status)
        {
            Type = type;
            BuildNodes = buildNodes;
            Status = status;
        }

        protected abstract string GetMessageTextId();

        protected string StatusTextId(bool isSingular) =>
            Status switch
            {
                BuildStatus.Cancelled => StringLocalizer.Instance.GetText(isSingular ? CancelledSingular : CancelledPlural),
                BuildStatus.Succeeded => StringLocalizer.Instance.GetText(isSingular ? SucceededSingular : SucceededPlural),
                BuildStatus.PartiallySucceeded => StringLocalizer.Instance.GetText(isSingular ? SucceededSingular : SucceededPlural),
                _ => StringLocalizer.Instance.GetText(isSingular ? FailedSingular : FailedPlural)
            };
    }
}