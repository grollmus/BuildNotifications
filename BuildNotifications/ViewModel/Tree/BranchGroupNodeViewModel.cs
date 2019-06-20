using System.Linq;
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

        public BranchGroupNodeViewModel(IBranchGroupNode node)
        {
            _node = node;
            AddOneBuildCommand = new DelegateCommand(AddOneBuild);
            RemoveOneChildCommand = new DelegateCommand(RemoveOneChild);
            AddAndRemoveCommand = new DelegateCommand(o =>
            {
                AddOneBuild(o);
                RemoveOneChild(o);
            });
        }

        private void AddOneBuild(object parameter)
        {
            var otherBuild = Children.FirstOrDefault() as BuildNodeViewModel;

            if (otherBuild == null)
                return;

            var newBuild = new BuildNodeViewModel(null)
            {
                MaxTreeDepth = otherBuild.MaxTreeDepth,
                CurrentTreeLevelDepth = otherBuild.CurrentTreeLevelDepth
            };

            Children.Add(newBuild);
        }

        private void RemoveOneChild(object parameter)
        {
            var someBuild = Children.FirstOrDefault();
            if (someBuild != null)
                Children.Remove(someBuild);
        }
    }
}
