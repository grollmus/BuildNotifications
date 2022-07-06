namespace BuildNotifications.Core.Pipeline.Notification;

public enum NotificationType
{
    Branch,
    Definition,
    DefinitionAndBranch,
    Build,
    None,
    Error,
    Info,
    Success,
    Progress
}