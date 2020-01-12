namespace BuildNotifications.Core.Pipeline.Tree
{
    public interface IBranchGroupNode : IGroupNode
    {
        string BranchName { get; }
        bool IsPullRequest { get; }
    }
}