using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree.Dummy
{
    internal class BuildTreeNodeDummy : IBuildTreeNode
    {
        public BuildTreeNodeDummy()
        {
            _children = new List<IBuildTreeNode>();
        }

        public IEnumerable<IBuildTreeNode> Children => _children;

        private readonly List<IBuildTreeNode> _children;

        public void AddChild(IBuildTreeNode node)
        {
            _children.Add(node);
        }

        public void RemoveChild(IBuildTreeNode node)
        {
            _children.Remove(node);
        }

        public void AddRange(IEnumerable<IBuildTreeNode> nodes)
        {
            foreach (var node in nodes)
            {
                AddChild(node);
            }
        }
    }
}