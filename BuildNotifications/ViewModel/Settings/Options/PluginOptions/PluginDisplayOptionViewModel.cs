using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions;

internal class PluginDisplayOptionViewModel : OptionViewModelBase, IPluginOptionViewModel
{
    public PluginDisplayOptionViewModel(IOption option, ILocalizationProvider localizationProvider)
        : base(option.NameTextId, option.DescriptionTextId)
    {
        _pluginOptionViewModelImplementation = new PluginOptionViewModelImplementation(option, localizationProvider, this);
        _value = option.ToString() ?? string.Empty;
    }

    public PluginDisplayOptionViewModel(DisplayOption displayOption, ILocalizationProvider localizationProvider)
        : base(displayOption.NameTextId, displayOption.DescriptionTextId)
    {
        _pluginOptionViewModelImplementation = new PluginOptionViewModelImplementation(displayOption, localizationProvider, this);
        _value = displayOption.Value;

        displayOption.ValueChanged += (_, _) => Value = displayOption.Value;
    }

    public override string Description => _pluginOptionViewModelImplementation.Description;
    public override string DisplayName => _pluginOptionViewModelImplementation.DisplayName;
    public override bool IsEnabled => _pluginOptionViewModelImplementation.IsEnabled;
    public override bool IsLoading => _pluginOptionViewModelImplementation.IsLoading;
    public override bool IsVisible => _pluginOptionViewModelImplementation.IsVisible;

    public string Value
    {
        get => _value;
        private set
        {
            if (_value == value)
                return;

            _value = value;
            OnPropertyChanged();
        }
    }

    public void Clear()
    {
        _pluginOptionViewModelImplementation.Clear();
    }

    private readonly PluginOptionViewModelImplementation _pluginOptionViewModelImplementation;
    private string _value;
}