using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Tree
{
    public abstract class BuildTreeNodeViewModel : BaseViewModel
    {
        public RemoveTrackingObservableCollection<BuildTreeNodeViewModel> Children { get; }

        public int CurrentTreeLevelDepth { get; set; }

        public int MaxTreeDepth { get; set; }

        public ICommand AddOneBuildCommand { get; set; }

        public ICommand RemoveOneChildCommand { get; set; }

        public ICommand AddAndRemoveCommand { get; set; }

        public ICommand HighlightCommand { get; set; }

        // only display status for the lowest and third lowest levels
        public bool ShouldColorByStatus
        {
            get
            {
                var thisLevelToDeepest = MaxTreeDepth - CurrentTreeLevelDepth;

                return thisLevelToDeepest == 0 || thisLevelToDeepest == 2;
            }
        }

        public BuildStatus BuildStatus => CalculateBuildStatus();

        // object this ViewModel originates from
        public IBuildTreeNode NodeSource { get; }

        protected BuildTreeNodeViewModel(IBuildTreeNode nodeSource)
        {
            NodeSource = nodeSource;
            Children = new RemoveTrackingObservableCollection<BuildTreeNodeViewModel>(TimeSpan.FromSeconds(0.8));
            Children.CollectionChanged += ChildrenOnCollectionChanged;
            AddOneBuildCommand = new DelegateCommand(AddOneBuild);
            RemoveOneChildCommand = new DelegateCommand(RemoveOneChild);
            AddAndRemoveCommand = new DelegateCommand(o =>
            {
                AddOneBuild(o);
                RemoveOneChild(o);
            });
            HighlightCommand = new DelegateCommand(Highlight);
        }

        private void ChildrenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (BuildTreeNodeViewModel child in e.NewItems)
                    {
                        child.PropertyChanged += OnChildPropertyChanged;
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (BuildTreeNodeViewModel child in e.OldItems)
                    {
                        child.PropertyChanged -= OnChildPropertyChanged;
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var child in Children)
                    {
                        child.PropertyChanged -= OnChildPropertyChanged;
                    }

                    break;
            }
        }

        private void OnChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(BuildStatus)))
                OnPropertyChanged(nameof(BuildStatus));
        }

        protected virtual BuildStatus CalculateBuildStatus()
        {
            return !Children.Any() ? BuildStatus.None : Children.Max(x => x.BuildStatus);
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

            var newBuild = new BuildNodeViewModel(otherBuild.Node)
            {
                MaxTreeDepth = otherBuild.MaxTreeDepth,
                CurrentTreeLevelDepth = otherBuild.CurrentTreeLevelDepth,
                IsLargeSize = true,
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