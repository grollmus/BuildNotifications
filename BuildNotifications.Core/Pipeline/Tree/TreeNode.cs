using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal abstract class TreeNode : IBuildTreeNode
    {
        protected TreeNode()
        {
            _childList = new List<IBuildTreeNode>();
        }

        public void AddChild(IBuildTreeNode node)
        {
            _childList.Add(node);
        }

        public void RemoveChild(IBuildTreeNode node)
        {
            _childList.RemoveAll(x => ReferenceEquals(x, node));
        }

        public int Depth { get; set; }

        public virtual void UpdateWithValuesFrom(IBuildTreeNode nodeToInsert)
        {
        }

        public IEnumerable<IBuildTreeNode> AllChildren()
        {
            return Children.Concat(Children.SelectMany(c => c.AllChildren()));
        }

        public IEnumerable<IBuildTreeNode> Children => _childList;

        public virtual bool Equals(IBuildTreeNode other)
        {
            if (other == null)
                return false;

            return Depth == other.Depth;
        }

        private readonly List<IBuildTreeNode> _childList;
    }
}