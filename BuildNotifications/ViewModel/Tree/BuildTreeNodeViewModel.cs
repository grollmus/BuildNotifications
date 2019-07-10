﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Tree.Dummy;
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

        public string DisplayName => CalculateDisplayName();

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
                RemoveOneChild(o);
                AddOneBuild(o);
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

            OnPropertyChanged(nameof(BuildStatus));
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

        protected virtual BuildStatus CalculateBuildStatus()
        {
            return !Children.Any() ? BuildStatus.None : Children.Max(x => x.BuildStatus);
        }

        protected virtual string CalculateDisplayName() => ToString();

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

        private async void AddOneBuild(object parameter)
        {
            var otherBuild = Children.FirstOrDefault() as BuildNodeViewModel;

            if (otherBuild == null)
                return;

            var newBuild = new BuildNodeViewModel(new BuildNodeDummy((BuildStatus) new Random().Next((int) BuildStatus.Cancelled, (int) BuildStatus.Failed + 1)))
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

            await Task.Delay(500);

            if (newBuild.BuildStatus == BuildStatus.Failed)
                newBuild.IsHighlighted = true;
        }

        private void RemoveOneChild(object parameter)
        {
            var someBuild = Children.FirstOrDefault(x => !x.IsRemoving);
            if (someBuild != null)
                Children.Remove(someBuild);
        }

        private SortingDefinition _currentSortingDefinition;

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
    }
}