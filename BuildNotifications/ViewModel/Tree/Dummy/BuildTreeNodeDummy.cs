using System;
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

        public void AddRange(IEnumerable<IBuildTreeNode> nodes)
        {
            foreach (var node in nodes)
            {
                AddChild(node);
            }
        }

        public void Clear()
        {
            _children.Clear();
        }

        public IEnumerable<IBuildTreeNode> Children => _children;

        public void AddChild(IBuildTreeNode node)
        {
            _children.Add(node);
        }

        public void RemoveChild(IBuildTreeNode node)
        {
            _children.Remove(node);
        }

        public bool Equals(IBuildTreeNode other)
        {
            throw new NotImplementedException();
        }

        private readonly List<IBuildTreeNode> _children;
    }
}