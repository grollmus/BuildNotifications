using System;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class SortingDefinitionsSelectionChangedEventArgs : EventArgs
    {
        public SortingDefinitionViewModel OldValue { get; }
        public SortingDefinitionViewModel NewValue { get; }

        public SortingDefinitionsSelectionChangedEventArgs(SortingDefinitionViewModel oldValue, SortingDefinitionViewModel newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}