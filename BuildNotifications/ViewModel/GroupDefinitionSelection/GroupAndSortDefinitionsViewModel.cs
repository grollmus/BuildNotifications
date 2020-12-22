using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Core.Text;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupAndSortDefinitionsViewModel : BaseViewModel
    {
        public GroupAndSortDefinitionsViewModel()
        {
            Definitions = new RemoveTrackingObservableCollection<GroupDefinitionsViewModel>(TimeSpan.FromSeconds(0.4));
            Definitions.CollectionChanged += DefinitionsOnCollectionChanged;
            Definitions.Add(new GroupDefinitionsViewModel());
        }

        public IBuildTreeGroupDefinition BuildTreeGroupDefinition
        {
            get => new BuildTreeGroupDefinition(ToGroupDefinitions());
            set => FromGroupDefinitions(value);
        }

        public IBuildTreeSortingDefinition BuildTreeSortingDefinition
        {
            get => new BuildTreeSortingDefinition(ToSortDefinitions());
            set => FromSortDefinitions(value);
        }

        public RemoveTrackingObservableCollection<GroupDefinitionsViewModel> Definitions { get; }

        private void AddNoneAtEnd()
        {
            var selectedGroupDefinition = Definitions.LastOrDefault()?.SelectedDefinition?.GroupDefinition;
            if (selectedGroupDefinition == null || selectedGroupDefinition == GroupDefinition.None)
                return;

            Definitions.Add(new GroupDefinitionsViewModel());
        }

        private void DefinitionsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems?.OfType<GroupDefinitionsViewModel>() ?? Enumerable.Empty<GroupDefinitionsViewModel>())
                    {
                        item.SelectedDefinitionChanged += SingleGroupDefinitionChanged;
                        item.SelectedSortingDefinitionChanged += SingleSortingDefinitionChanged;
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems?.OfType<GroupDefinitionsViewModel>() ?? Enumerable.Empty<GroupDefinitionsViewModel>())
                    {
                        item.SelectedDefinitionChanged -= SingleGroupDefinitionChanged;
                        item.SelectedSortingDefinitionChanged -= SingleSortingDefinitionChanged;
                    }

                    break;
            }
        }

        private void FromGroupDefinitions(IEnumerable<GroupDefinition> definitions)
        {
            var definitionsList = definitions.ToList();
            var neededAmountOfVms = definitionsList.Count + 1;
            _suppressEvents = true;
            for (var i = Definitions.Count; i < neededAmountOfVms; i++)
            {
                Definitions.Add(new GroupDefinitionsViewModel());
            }

            var index = 0;
            foreach (var groupDefinition in definitionsList)
            {
                var definition = Definitions[index];
                definition.SelectedDefinition = definition.Definitions.First(x => x.GroupDefinition.Equals(groupDefinition));
                index++;
            }

            Definitions.Last().SelectedDefinition = Definitions.Last().Definitions.First(x => x.GroupDefinition == GroupDefinition.None);
            _suppressEvents = false;
        }

        private void FromSortDefinitions(IEnumerable<SortingDefinition> definitions)
        {
            var index = 0;
            foreach (var sortingDefinition in definitions)
            {
                if (Definitions.Count <= index)
                    break;

                var viewModelAtIndex = Definitions[index];
                viewModelAtIndex.SelectedSortingDefinition = sortingDefinition;

                index++;
            }
        }

        private void RemoveNoneElements()
        {
            var allNoneItems = Definitions.Where(x => x.SelectedDefinition?.GroupDefinition == GroupDefinition.None).ToList();
            var lastItem = Definitions.Last();

            if (allNoneItems.Contains(lastItem))
                allNoneItems.Remove(lastItem);

            foreach (var definition in allNoneItems)
            {
                Definitions.Remove(definition);
            }
        }

        private void SetTexts()
        {
            var setFirst = false;
            foreach (var definition in Definitions.Where(x => !x.IsRemoving))
            {
                var key = setFirst ? "ThenBy" : "GroupBy";
                setFirst = true;
                definition.GroupByText = StringLocalizer.Instance[key];
            }
        }

        private void SingleGroupDefinitionChanged(object? sender, GroupDefinitionsSelectionChangedEventArgs e)
        {
            if (_suppressEvents)
                return;

            _suppressEvents = true;
            RemoveNoneElements();
            if (sender is GroupDefinitionsViewModel groupDefinitionsViewModel)
                SwapDuplicates(groupDefinitionsViewModel, e);
            AddNoneAtEnd();
            SetTexts();
            _suppressEvents = false;
            OnPropertyChanged(nameof(BuildTreeGroupDefinition));
            OnPropertyChanged(nameof(BuildTreeSortingDefinition));
        }

        private void SingleSortingDefinitionChanged(object? sender, SortingDefinitionsSelectionChangedEventArgs e)
        {
            if (_suppressEvents)
                return;

            OnPropertyChanged(nameof(BuildTreeSortingDefinition));
        }

        private void SwapDuplicates(GroupDefinitionsViewModel sender, GroupDefinitionsSelectionChangedEventArgs e)
        {
            if (e.NewValue.GroupDefinition == GroupDefinition.None)
                return;

            var newSelectedValue = e.NewValue.GroupDefinition;
            var otherElementThatHasSameValue = Definitions.FirstOrDefault(x => !x.IsRemoving && x != sender && x.SelectedDefinition?.GroupDefinition == newSelectedValue);
            var previousSelectedValue = sender.Definitions.First(x => x.GroupDefinition == e.OldValue.GroupDefinition);

            if (otherElementThatHasSameValue == null)
                return;

            if (e.OldValue.GroupDefinition == GroupDefinition.None)
                Definitions.Remove(otherElementThatHasSameValue);
            else
            {
                otherElementThatHasSameValue.SelectedDefinition = otherElementThatHasSameValue.Definitions.First(x => x.GroupDefinition == e.OldValue.GroupDefinition);
                otherElementThatHasSameValue.SelectedSortingDefinition = previousSelectedValue.SortingDefinitionsViewModel.SelectedSortingDefinition;
            }
        }

        private IEnumerable<GroupDefinition> ToGroupDefinitions()
        {
            foreach (var def in Definitions.Where(x => x.SelectedDefinition?.GroupDefinition != GroupDefinition.None))
            {
                if (def.SelectedDefinition?.GroupDefinition != null)
                    yield return def.SelectedDefinition.GroupDefinition;
            }
        }

        private IEnumerable<SortingDefinition> ToSortDefinitions()
        {
            foreach (var def in Definitions.Where(x => x.SelectedDefinition?.GroupDefinition != GroupDefinition.None))
            {
                if (def.SelectedDefinition?.SortingDefinitionsViewModel.SelectedViewModel != null)
                    yield return def.SelectedDefinition.SortingDefinitionsViewModel.SelectedViewModel.SortingDefinition;
            }
        }

        private bool _suppressEvents;
    }
}