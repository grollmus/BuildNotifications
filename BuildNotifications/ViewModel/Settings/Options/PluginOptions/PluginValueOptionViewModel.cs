using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
    internal class PluginValueOptionViewModel<TValue> : OptionViewModelBase<TValue>, IPluginOptionViewModel
    {
        public PluginValueOptionViewModel(ValueOption<TValue> valueOption, ILocalizationProvider localizationProvider)
            : base(valueOption.Value, valueOption.NameTextId)
        {
            _pluginOptionViewModelImplementation = new PluginOptionViewModelImplementation<TValue>(valueOption, localizationProvider, this);

            ValueOption = valueOption;
            ValueOption.ValueChanged += ValueOption_ValueChanged;
        }

        protected ValueOption<TValue> ValueOption { get; }

        private void ValueOption_ValueChanged(object? sender, ValueChangedEventArgs<TValue> e)
        {
            OnPropertyChanged(nameof(Value));
        }

        public override string Description => _pluginOptionViewModelImplementation.Description;
        public override string DisplayName => _pluginOptionViewModelImplementation.DisplayName;

        private readonly PluginOptionViewModelImplementation<TValue> _pluginOptionViewModelImplementation;
    }
}