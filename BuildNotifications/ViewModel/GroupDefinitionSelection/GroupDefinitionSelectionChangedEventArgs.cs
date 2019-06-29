using System;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionSelectionChangedEventArgs : EventArgs
    {
        public GroupDefinitionViewModel OldValue { get; }
        public GroupDefinitionViewModel NewValue { get; }

        public GroupDefinitionSelectionChangedEventArgs(GroupDefinitionViewModel oldValue, GroupDefinitionViewModel newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}