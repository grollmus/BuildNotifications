using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Setup.PluginOptions
{
    internal class PluginValueOptionViewModel<TValue> : PluginOptionViewModel
    {
        public PluginValueOptionViewModel(ValueOption<TValue> valueOption, ILocalizationProvider localizationProvider)
            : base(valueOption, localizationProvider)
        {
            ValueOption = valueOption;
            ValueOption.ValueChanged += ValueOption_ValueChanged;
        }

        public TValue Value
        {
            get => ValueOption.Value;
            set => ValueOption.Value = value;
        }

        protected ValueOption<TValue> ValueOption { get; }

        private void ValueOption_ValueChanged(object? sender, ValueChangedEventArgs<TValue> e)
        {
            OnPropertyChanged(nameof(Value));
        }
    }
}