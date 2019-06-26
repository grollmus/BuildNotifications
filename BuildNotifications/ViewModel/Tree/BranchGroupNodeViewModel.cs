﻿using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Tree
{
    public class BranchGroupNodeViewModel : BuildTreeNodeViewModel
    {
        private readonly IBranchGroupNode _node;

        public string BranchName => _node.BranchName;

        public ICommand AddOneBuildCommand { get; set; }

        public ICommand RemoveOneChildCommand { get; set; }

        public ICommand AddAndRemoveCommand { get; set; }

        public ICommand HighlightCommand { get; set; }

        public BranchGroupNodeViewModel(IBranchGroupNode node) : base(node)
        {
            _node = node;
            AddOneBuildCommand = new DelegateCommand(AddOneBuild);
            RemoveOneChildCommand = new DelegateCommand(RemoveOneChild);
            AddAndRemoveCommand = new DelegateCommand(o =>
            {
                AddOneBuild(o);
                RemoveOneChild(o);
            });
            HighlightCommand = new DelegateCommand(Highlight);
        }

        private void Highlight(object obj)
        {
            bool? targetValue = null;
            foreach (var build in Children.OfType<BuildNodeViewModel>())
            {
                if (targetValue == null)
                    targetValue = !build.IsHighlighted;

                build.IsHighlighted = targetValue.Value;
            }
        }

        private void AddOneBuild(object parameter)
        {
            var otherBuild = Children.FirstOrDefault() as BuildNodeViewModel;

            if (otherBuild == null)
                return;

            var newBuild = new BuildNodeViewModel(null)
            {
                MaxTreeDepth = otherBuild.MaxTreeDepth,
                CurrentTreeLevelDepth = otherBuild.CurrentTreeLevelDepth,
                IsLargeSize = true
            };

            foreach (var build in Children.OfType<BuildNodeViewModel>().Where(x => x.IsLargeSize))
            {
                build.IsLargeSize = false;
            }

            Children.Add(newBuild);
        }

        private void RemoveOneChild(object parameter)
        {
            var someBuild = Children.FirstOrDefault(x => !x.IsRemoving);
            if (someBuild != null)
                Children.Remove(someBuild);
        }
    }
}
