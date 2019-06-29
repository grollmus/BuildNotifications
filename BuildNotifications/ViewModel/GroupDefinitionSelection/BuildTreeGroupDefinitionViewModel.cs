using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class BuildTreeGroupDefinitionViewModel : BaseViewModel
    {
        public RemoveTrackingObservableCollection<SingleGroupDefinitionSelectionViewModel> Definitions { get; set; }

        public IBuildTreeGroupDefinition GroupDefinition
        {
            get => new BuildTreeGroupDefinition(ToGroupDefinitions());
            set => FromGroupDefinitions(value);
        }

        private IEnumerable<GroupDefinition> ToGroupDefinitions()
        {
            foreach (var def in Definitions.Where(x => x.SelectedDefinition.GroupDefinition != Core.Pipeline.Tree.GroupDefinition.None))
            {
                yield return def.SelectedDefinition.GroupDefinition;
            }
        }

        private bool _suppressEvents;

        public BuildTreeGroupDefinitionViewModel()
        {
            Definitions = new RemoveTrackingObservableCollection<SingleGroupDefinitionSelectionViewModel>(TimeSpan.FromSeconds(0.4));
            Definitions.CollectionChanged += DefinitionsOnCollectionChanged;
            Definitions.Add(new SingleGroupDefinitionSelectionViewModel());
        }

        private void FromGroupDefinitions(IEnumerable<GroupDefinition> definitions)
        {
            var definitionsList = definitions.ToList();
            var neededAmountOfVms = definitionsList.Count + 1;

            for (var i = Definitions.Count; i < neededAmountOfVms; i++)
            {
                Definitions.Add(new SingleGroupDefinitionSelectionViewModel());
            }

            var index = 0;
            foreach (var groupDefinition in definitionsList)
            {
                var definition = Definitions[index];
                definition.SelectedDefinition = definition.Definitions.First(x => x.GroupDefinition.Equals(groupDefinition));
                index++;
            }

            Definitions.Last().SelectedDefinition = Definitions.Last().Definitions.First(x => x.GroupDefinition == Core.Pipeline.Tree.GroupDefinition.None);
        }

        private void DefinitionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (SingleGroupDefinitionSelectionViewModel item in e.NewItems)
                    {
                        item.SelectedDefinitionChanged += SingleGroupDefinitionChanged;
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (SingleGroupDefinitionSelectionViewModel item in e.OldItems)
                    {
                        item.SelectedDefinitionChanged -= SingleGroupDefinitionChanged;
                    }

                    break;
                default:
                    break;
            }
        }

        private void SingleGroupDefinitionChanged(object sender, GroupDefinitionSelectionChangedEventArgs e)
        {
            if (_suppressEvents)
                return;

            _suppressEvents = true;
            CutOffAfterNone();
            SwapDuplicates((SingleGroupDefinitionSelectionViewModel) sender, e);
            AddNoneAtEnd();
            SetTexts();
            _suppressEvents = false;
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

        private void CutOffAfterNone()
        {
            var firstNoneItem = Definitions.FirstOrDefault(x => x.SelectedDefinition.GroupDefinition == Core.Pipeline.Tree.GroupDefinition.None);
            if (firstNoneItem == null)
                return;
            var indexOfNone = Definitions.IndexOf(firstNoneItem);

            var itemsToRemove = Definitions.Skip(indexOfNone + 1).ToList();
            foreach (var definition in itemsToRemove)
            {
                Definitions.Remove(definition);
            }
        }

        private void SwapDuplicates(SingleGroupDefinitionSelectionViewModel sender, GroupDefinitionSelectionChangedEventArgs e)
        {
            var newSelectedValue = e.NewValue.GroupDefinition;
            var otherElementThatHasSameValue = Definitions.FirstOrDefault(x => !x.IsRemoving && x != sender && x.SelectedDefinition.GroupDefinition == newSelectedValue);

            if (otherElementThatHasSameValue == null)
                return;

            if (e.OldValue.GroupDefinition == Core.Pipeline.Tree.GroupDefinition.None)
                Definitions.Remove(otherElementThatHasSameValue);
            else
                otherElementThatHasSameValue.SelectedDefinition = otherElementThatHasSameValue.Definitions.First(x => x.GroupDefinition == e.OldValue.GroupDefinition);
        }

        private void AddNoneAtEnd()
        {
            if (Definitions.LastOrDefault()?.SelectedDefinition.GroupDefinition == Core.Pipeline.Tree.GroupDefinition.None)
                return;

            Definitions.Add(new SingleGroupDefinitionSelectionViewModel());
        }
    }
}