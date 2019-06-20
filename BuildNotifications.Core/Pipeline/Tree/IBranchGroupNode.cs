namespace BuildNotifications.Core.Pipeline.Tree
{
    internal interface IBranchGroupNode : IGroupNode
    {
        string BranchName { get; }
    }
}