using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification
{
    public abstract class BaseNotification : INotification
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

        public string DisplayTitle => string.Format(StringLocalizer.Instance.GetText(TitleTextId), Parameters.ToArray());
        
        public string TitleTextId => GetMessageTextId();

        public string DisplayContent => string.Format(StringLocalizer.Instance.GetText(ContentTextId), new[] {BuildNodes.Count});

        public string ContentTextId => BuildNodes.Count == 1 ? BuildNotificationContentSingularTextId : BuildNotificationContentPluralTextId;

        public NotificationType Type { get; }

        public IList<IBuildNode> BuildNodes { get; }

        public BuildStatus Status { get; }

        protected List<string> Parameters { get; } = new List<string>();

        protected BaseNotification(NotificationType type, IList<IBuildNode> buildNodes, BuildStatus status)
        {
            Type = type;
            BuildNodes = buildNodes;
            Status = status;
        }

        protected abstract string GetMessageTextId();

        protected string StatusTextId(bool isSingular) =>
            Status switch
            {
                BuildStatus.Cancelled => (isSingular ? CancelledSingular : CancelledPlural),
                BuildStatus.Succeeded => (isSingular ? SucceededSingular : SucceededPlural),
                BuildStatus.PartiallySucceeded => (isSingular ? SucceededSingular : SucceededPlural),
                _ => (isSingular ? FailedSingular : FailedPlural)
            };
    }
}