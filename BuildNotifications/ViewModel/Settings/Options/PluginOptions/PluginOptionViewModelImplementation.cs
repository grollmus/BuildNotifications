using System;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
    internal class PluginOptionViewModelImplementation : IPluginOptionViewModel
    {
        public PluginOptionViewModelImplementation(IOption option,
            ILocalizationProvider localizationProvider,
            IViewModel viewModel)
        {
            Option = option;
            _localizationProvider = localizationProvider;
            ViewModel = viewModel;

            Option.IsEnabledChanged += Option_IsEnabledChanged;
            Option.IsVisibleChanged += Option_IsVisibleChanged;
            Option.IsLoadingChanged += Option_IsLoadingChanged;
        }

        public string Description => _localizationProvider.Localize(Option.DescriptionTextId);
        public string DisplayName => _localizationProvider.Localize(Option.NameTextId);

        private IOption Option { get; }

        private void Option_IsEnabledChanged(object? sender, EventArgs e)
        {
            ViewModel.OnPropertyChanged(nameof(OptionViewModelBase.IsEnabled));
        }

        private void Option_IsLoadingChanged(object? sender, EventArgs e)
        {
            ViewModel.OnPropertyChanged(nameof(OptionViewModelBase.IsLoading));
        }

        private void Option_IsVisibleChanged(object? sender, EventArgs e)
        {
            ViewModel.OnPropertyChanged(nameof(OptionViewModelBase.IsVisible));
        }

        protected readonly IViewModel ViewModel;

        private readonly ILocalizationProvider _localizationProvider;
    }

    internal class PluginOptionViewModelImplementation<TValue> : PluginOptionViewModelImplementation
    {
        public PluginOptionViewModelImplementation(ValueOption<TValue> option,
            ILocalizationProvider localizationProvider,
            IViewModel viewModel)
            : base(option, localizationProvider, viewModel)
        {
            Option = option;

            Option.ValueChanged += OptionOnValueChanged;
        }

        public TValue Value
        {
            get => Option.Value;
            set => Option.Value = value;
        }

        private ValueOption<TValue> Option { get; }

        private void OptionOnValueChanged(object? sender, ValueChangedEventArgs<TValue> e)
        {
            ViewModel.OnPropertyChanged(nameof(OptionViewModelBase<TValue>.Value));
        }
    }
}