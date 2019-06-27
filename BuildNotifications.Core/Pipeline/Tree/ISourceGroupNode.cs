namespace BuildNotifications.Core.Pipeline.Tree
{
    public interface ISourceGroupNode : IGroupNode
    {
        string SourceName { get; set; }
    }
}
