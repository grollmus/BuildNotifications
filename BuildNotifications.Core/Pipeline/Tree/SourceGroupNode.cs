namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class SourceGroupNode : TreeNode, ISourceGroupNode
    {
        public SourceGroupNode(string sourceName)
        {
            SourceName = sourceName;
        }

        public string SourceName { get; set; }

        public override bool Equals(IBuildTreeNode other)
        {
            return SourceName.Equals((other as SourceGroupNode)?.SourceName);
        }
    }
}