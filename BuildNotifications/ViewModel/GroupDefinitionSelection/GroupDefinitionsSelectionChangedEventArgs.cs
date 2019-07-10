using System;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionsSelectionChangedEventArgs : EventArgs
    {
        public GroupDefinitionsSelectionChangedEventArgs(GroupDefinitionViewModel oldValue, GroupDefinitionViewModel newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public GroupDefinitionViewModel NewValue { get; }
        public GroupDefinitionViewModel OldValue { get; }
    }
}