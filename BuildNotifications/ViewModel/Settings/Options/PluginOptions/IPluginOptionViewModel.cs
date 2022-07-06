using System.ComponentModel;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions;

internal interface IPluginOptionViewModel : INotifyPropertyChanged
{
    void Clear();
}