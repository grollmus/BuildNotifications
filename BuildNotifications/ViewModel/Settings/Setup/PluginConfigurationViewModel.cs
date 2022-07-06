using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.ViewModel.Settings.Options.PluginOptions;

namespace BuildNotifications.ViewModel.Settings.Setup;

internal class PluginConfigurationViewModel : BaseViewModel
{
    public PluginConfigurationViewModel(IPluginConfiguration configuration)
    {
        Configuration = configuration;
        Options = new ObservableCollection<IPluginOptionViewModel>(ConstructOptionViewModels(configuration));

        foreach (var option in Options)
        {
            option.PropertyChanged += OptionOnPropertyChanged;
        }
    }

    public IPluginConfiguration Configuration { get; }

    public ObservableCollection<IPluginOptionViewModel> Options { get; }

    public event EventHandler<EventArgs>? ValueChanged;

    public void Clear()
    {
        foreach (var option in Options)
        {
            option.PropertyChanged -= OptionOnPropertyChanged;
            option.Clear();
        }
    }

    private IEnumerable<IPluginOptionViewModel> ConstructOptionViewModels(IPluginConfiguration configuration)
    {
        var factory = new PluginOptionViewModelFactory(configuration.Localizer);
        return configuration.ListAvailableOptions().Select(o => factory.Construct(o));
    }

    private void OptionOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IValueOption.Value))
            ValueChanged?.Invoke(this, EventArgs.Empty);
    }
}