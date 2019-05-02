namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class BuildTree : TreeNode, IBuildTree
    {
        public BuildTree(IBuildTreeGroupDefinition groupDefinition)
        {
            GroupDefinition = groupDefinition;
        }

        /// <inheritdoc />
        public IBuildTreeGroupDefinition GroupDefinition { get; }
    }
}