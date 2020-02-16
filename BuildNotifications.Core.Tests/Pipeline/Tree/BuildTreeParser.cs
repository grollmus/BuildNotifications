using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.Core.Tests.Pipeline.Tree
{
    internal class BuildTreeParser
    {
        public BuildTreeParser(IBuildTreeNode tree)
        {
            _tree = tree;
        }

        public IEnumerable<IBuildTreeNode> ChildrenAtLevel(int level)
        {
            if (level == 0)
                return _tree.Yield();

            var currentLevel = 1;

            var currentChildren = _tree.Children.ToList();

            while (currentLevel < level && currentChildren.Any())
            {
                currentChildren = currentChildren.SelectMany(x => x.Children).ToList();
                ++currentLevel;
            }

            return currentChildren;
        }

        private readonly IBuildTreeNode _tree;
    }
}