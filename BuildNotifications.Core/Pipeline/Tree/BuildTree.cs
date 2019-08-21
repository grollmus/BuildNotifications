using System;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;

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

        public override bool Equals(IBuildTreeNode other)
        {
            throw new NotImplementedException();
        }
    }
}