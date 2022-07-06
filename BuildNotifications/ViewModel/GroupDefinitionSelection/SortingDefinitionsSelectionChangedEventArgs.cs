using System;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection;

public class SortingDefinitionsSelectionChangedEventArgs : EventArgs
{
    public SortingDefinitionsSelectionChangedEventArgs(SortingDefinitionViewModel oldValue, SortingDefinitionViewModel newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    public SortingDefinitionViewModel NewValue { get; }
    public SortingDefinitionViewModel OldValue { get; }
}