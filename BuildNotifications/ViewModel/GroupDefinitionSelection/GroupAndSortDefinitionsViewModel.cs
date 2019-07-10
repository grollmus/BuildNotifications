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
        public RemoveTrackingObservableCollection<GroupDefinitionsViewModel> Definitions { get; set; }

        public GroupAndSortDefinitionsViewModel()
        {
            Definitions = new RemoveTrackingObservableCollection<GroupDefinitionsViewModel>(TimeSpan.FromSeconds(0.4));
            Definitions.CollectionChanged += DefinitionsOnCollectionChanged;
            Definitions.Add(new GroupDefinitionsViewModel());
        }

        public IBuildTreeSortingDefinition BuildTreeSortingDefinition
        {
            get => new BuildTreeSortingDefinition(ToSortDefinitions());
            set => FromSortDefinitions(value);
        }

        private IEnumerable<SortingDefinition> ToSortDefinitions()
        {
            foreach (var def in Definitions.Where(x => x.SelectedDefinition.GroupDefinition != GroupDefinition.None))
            {
                yield return def.SelectedDefinition.SortingDefinitionsViewModel.SelectedViewModel.SortingDefinition;
            }
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

        public IBuildTreeGroupDefinition BuildTreeGroupDefinition
        {
            get => new BuildTreeGroupDefinition(ToGroupDefinitions());
            set => FromGroupDefinitions(value);
        }

        private IEnumerable<GroupDefinition> ToGroupDefinitions()
        {
            foreach (var def in Definitions.Where(x => x.SelectedDefinition.GroupDefinition != GroupDefinition.None))
            {
                yield return def.SelectedDefinition.GroupDefinition;
            }
        }

        private void FromGroupDefinitions(IEnumerable<GroupDefinition> definitions)
        {
            var definitionsList = definitions.ToList();
            var neededAmountOfVms = definitionsList.Count + 1;

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
        }

        private void DefinitionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (GroupDefinitionsViewModel item in e.NewItems)
                    {
                        item.SelectedDefinitionChanged += SingleGroupDefinitionChanged;
                        item.SelectedSortingDefinitionChanged += SingleSortingDefinitionChanged;
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (GroupDefinitionsViewModel item in e.OldItems)
                    {
                        item.SelectedDefinitionChanged -= SingleGroupDefinitionChanged;
                        item.SelectedSortingDefinitionChanged -= SingleSortingDefinitionChanged;
                    }

                    break;
                default:
                    break;
            }
        }

        private bool _suppressEvents;

        private void SingleGroupDefinitionChanged(object sender, GroupDefinitionsSelectionChangedEventArgs e)
        {
            if (_suppressEvents)
                return;

            _suppressEvents = true;
            RemoveNoneElements();
            SwapDuplicates((GroupDefinitionsViewModel) sender, e);
            AddNoneAtEnd();
            SetTexts();
            _suppressEvents = false;
            OnPropertyChanged(nameof(BuildTreeGroupDefinition));
            OnPropertyChanged(nameof(BuildTreeSortingDefinition));
        }

        private void SingleSortingDefinitionChanged(object sender, SortingDefinitionsSelectionChangedEventArgs e)
        {
            if (_suppressEvents)
                return;

            OnPropertyChanged(nameof(BuildTreeSortingDefinition));
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

        private void RemoveNoneElements()
        {
            var allNoneItems = Definitions.Where(x => x.SelectedDefinition.GroupDefinition == GroupDefinition.None).ToList();
            var lastItem = Definitions.Last();

            if (allNoneItems.Contains(lastItem))
                allNoneItems.Remove(lastItem);

            foreach (var definition in allNoneItems)
            {
                Definitions.Remove(definition);
            }
        }

        private void SwapDuplicates(GroupDefinitionsViewModel sender, GroupDefinitionsSelectionChangedEventArgs e)
        {
            if (e.NewValue.GroupDefinition == GroupDefinition.None)
                return;

            var newSelectedValue = e.NewValue.GroupDefinition;
            var otherElementThatHasSameValue = Definitions.FirstOrDefault(x => !x.IsRemoving && x != sender && x.SelectedDefinition.GroupDefinition == newSelectedValue);
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

        private void AddNoneAtEnd()
        {
            if (Definitions.LastOrDefault()?.SelectedDefinition.GroupDefinition == GroupDefinition.None)
                return;

            Definitions.Add(new GroupDefinitionsViewModel());
        }
    }
}