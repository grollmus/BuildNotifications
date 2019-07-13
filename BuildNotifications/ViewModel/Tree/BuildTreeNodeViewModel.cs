using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Tree
{
    public abstract class BuildTreeNodeViewModel : BaseViewModel
    {
        protected BuildTreeNodeViewModel(IBuildTreeNode nodeSource)
        {
            NodeSource = nodeSource;
            Children = new RemoveTrackingObservableCollection<BuildTreeNodeViewModel>(TimeSpan.FromSeconds(0.8));
            Children.CollectionChanged += ChildrenOnCollectionChanged;
            SetChildrenSorting(_currentSortingDefinition);

            RemoveOneChildCommand = new DelegateCommand(RemoveOneChild);
            AddAndRemoveCommand = new DelegateCommand(o =>
            {
                RemoveOneChild(o);
            });
            HighlightCommand = new DelegateCommand(Highlight);
        }

        public virtual void BackendPropertiesChanged()
        {
        }

        public ICommand AddAndRemoveCommand { get; set; }

        public ICommand AddOneBuildCommand { get; set; }

        public BuildStatus BuildStatus => CalculateBuildStatus();
        public RemoveTrackingObservableCollection<BuildTreeNodeViewModel> Children { get; }

        public int CurrentTreeLevelDepth { get; set; }

        public string DisplayName => CalculateDisplayName();

        public ICommand HighlightCommand { get; set; }

        public int MaxTreeDepth { get; set; }

        // object this ViewModel originates from
        public IBuildTreeNode NodeSource { get; }

        public ICommand RemoveOneChildCommand { get; set; }

        // only display status for the lowest and third lowest levels
        public bool ShouldColorByStatus
        {
            get
            {
                var thisLevelToDeepest = MaxTreeDepth - CurrentTreeLevelDepth;

                return thisLevelToDeepest == 0 || thisLevelToDeepest == 2;
            }
        }

        protected virtual BuildStatus CalculateBuildStatus()
        {
            return !Children.Any() ? BuildStatus.None : Children.Max(x => x.BuildStatus);
        }

        protected virtual string CalculateDisplayName()
        {
            return ToString();
        }

        protected void SetSortings(List<SortingDefinition> sortingDefinitions, int index = 0)
        {
            if (index >= sortingDefinitions.Count)
            {
                // the last group are expected to be builds anyway, these are only implicitly sorted by the order they are added to the list
                _currentSortingDefinition = SortingDefinition.Undefined;
                Children.DontSort();
                return;
            }

            foreach (var child in Children)
            {
                child.SetSortings(sortingDefinitions, index + 1);
            }

            var newSorting = sortingDefinitions[index];
            if (newSorting == _currentSortingDefinition)
                return;

            _currentSortingDefinition = newSorting;
            SetChildrenSorting(_currentSortingDefinition);
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

            if (Children.Any(x => x is BuildNodeViewModel))
                SetBuildLargeStatus();

            OnPropertyChanged(nameof(BuildStatus));
        }

        private void SetBuildLargeStatus()
        {
            var buildChildren = Children.OfType<BuildNodeViewModel>().ToList();

            foreach (var child in buildChildren)
            {
                child.IsLargeSize = false;
            }

            buildChildren.Last().IsLargeSize = true;
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

        private void OnChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(BuildStatus)))
            {
                OnPropertyChanged(nameof(BuildStatus));
                if (_currentSortingDefinition == SortingDefinition.StatusAscending || _currentSortingDefinition == SortingDefinition.StatusDescending)
                    Children.InvokeSort();
            }
        }

        private void RemoveOneChild(object parameter)
        {
            var someBuild = Children.FirstOrDefault(x => !x.IsRemoving);
            if (someBuild != null)
                Children.Remove(someBuild);
        }

        private void SetChildrenSorting(SortingDefinition sortingDefinition)
        {
            switch (sortingDefinition)
            {
                case SortingDefinition.AlphabeticalDescending:
                    Children.Sort(x => x.DisplayName);
                    break;
                case SortingDefinition.AlphabeticalAscending:
                    Children.SortDescending(x => x.DisplayName);
                    break;
                case SortingDefinition.StatusAscending:
                    Children.Sort(x => x.BuildStatus);
                    break;
                case SortingDefinition.StatusDescending:
                    Children.SortDescending(x => x.BuildStatus);
                    break;
            }
        }

        private SortingDefinition _currentSortingDefinition;
    }
}