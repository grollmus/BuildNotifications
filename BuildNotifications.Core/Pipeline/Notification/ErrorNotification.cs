using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification;

public class ErrorNotification : INotification
{
    public ErrorNotification(string messageTextId, params object[] parameter)
    {
        _parameter = parameter;
        ContentTextId = messageTextId;
    }

    public string DisplayContent => string.Format(StringLocalizer.CurrentCulture, StringLocalizer.Instance.GetText(ContentTextId), _parameter);

    public string ContentTextId { get; }

    public string DisplayTitle => StringLocalizer.Instance.GetText(TitleTextId);

    public string TitleTextId { get; set; } = "AnErrorOccured";

    public string IssueSource { get; } = "BuildNotifications";

    public string Source { get; set; } = "";

    public NotificationType Type => NotificationType.Error;

    public IList<IBuildNode> BuildNodes => new List<IBuildNode>();

    public BuildStatus Status => BuildStatus.Failed;

    public Guid Guid { get; } = Guid.NewGuid();
    private readonly object[] _parameter;
}