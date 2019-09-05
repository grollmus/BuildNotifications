using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification
{
    public abstract class BaseBuildNotification : INotification
    {
        protected BaseBuildNotification(NotificationType type, IList<IBuildNode> buildNodes, BuildStatus status)
        {
            Type = type;
            BuildNodes = buildNodes;
            Status = status;
        }

        protected List<string> Parameters { get; } = new List<string>();

        protected abstract string GetMessageTextId();

        protected string StatusTextId(bool isSingular)
        {
            return Status switch
            {
                BuildStatus.Cancelled => StringLocalizer.Instance.GetText(isSingular ? CancelledSingular : CancelledPlural),
                BuildStatus.Succeeded => StringLocalizer.Instance.GetText(isSingular ? SucceededSingular : SucceededPlural),
                BuildStatus.PartiallySucceeded => StringLocalizer.Instance.GetText(isSingular ? SucceededSingular : SucceededPlural),
                _ => StringLocalizer.Instance.GetText(isSingular ? FailedSingular : FailedPlural)
            };
        }

        public string DisplayContent => string.Format(StringLocalizer.Instance.GetText(ContentTextId), Parameters.ToArray());

        public string ContentTextId => GetMessageTextId();

        public string DisplayTitle => string.Format(StringLocalizer.Instance.GetText(TitleTextId), new object[] {StatusTextId(BuildNodes.Count == 1), BuildNodes.Count.ToString()});

        public string TitleTextId => BuildNodes.Count == 1 ? BuildNotificationContentSingularTextId : BuildNotificationContentPluralTextId;

        public NotificationType Type { get; }

        public IList<IBuildNode> BuildNodes { get; }

        public BuildStatus Status { get; }

        // {0} builds. E.g. 25 builds
        private const string BuildNotificationContentPluralTextId = nameof(BuildNotificationContentPluralTextId);

        // One build.
        private const string BuildNotificationContentSingularTextId = nameof(BuildNotificationContentSingularTextId);
        public const string CancelledPlural = nameof(CancelledPlural);

        public const string CancelledSingular = nameof(CancelledSingular);
        public const string FailedPlural = nameof(FailedPlural);
        public const string FailedSingular = nameof(FailedSingular);
        public const string SucceededPlural = nameof(SucceededPlural);

        public const string SucceededSingular = nameof(SucceededSingular);
    }
}