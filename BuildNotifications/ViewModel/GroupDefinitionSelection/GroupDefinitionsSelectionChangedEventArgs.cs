using System;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionsSelectionChangedEventArgs : EventArgs
    {
        public GroupDefinitionViewModel OldValue { get; }
        public GroupDefinitionViewModel NewValue { get; }

        public GroupDefinitionsSelectionChangedEventArgs(GroupDefinitionViewModel oldValue, GroupDefinitionViewModel newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}