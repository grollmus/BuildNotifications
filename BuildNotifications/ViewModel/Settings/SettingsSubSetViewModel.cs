using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using BuildNotifications.Resources.Settings;
using ReflectSettings.EditableConfigs;

namespace BuildNotifications.ViewModel.Settings
{
    public class SettingsSubSetViewModel
    {
        public SettingsSubSetViewModel(IEnumerable<IEditableConfig> editableConfigs)
        {
            Configs = new ObservableCollection<IEditableConfig>(editableConfigs);
        }

        public ObservableCollection<IEditableConfig> Configs { get; }

        public DataTemplateSelector DataTemplateSelector { get; set; } = EditableConfigTemplateSelector.Instance;
    }
}