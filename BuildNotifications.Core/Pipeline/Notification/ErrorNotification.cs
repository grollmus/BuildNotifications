using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification
{
    internal class ErrorNotification : INotification
    {
        private readonly object[] _parameter;

        public ErrorNotification(string messageTextId, params object[] parameter)
        {
            _parameter = parameter;
            ContentTextId = messageTextId;
        }
        
        public string DisplayContent => string.Format(StringLocalizer.Instance.GetText(ContentTextId), _parameter);

        public string ContentTextId { get; }

        public string DisplayTitle => StringLocalizer.Instance.GetText(TitleTextId);

        public string TitleTextId => "AnErrorOccured";

        public NotificationType Type => NotificationType.Error;

        public IList<IBuildNode> BuildNodes => new List<IBuildNode>();

        public BuildStatus Status => BuildStatus.Failed;
    }
}
