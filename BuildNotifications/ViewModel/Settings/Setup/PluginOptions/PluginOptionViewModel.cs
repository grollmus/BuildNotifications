using System;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Setup.PluginOptions
{
    internal abstract class PluginOptionViewModel : BaseViewModel
    {
        protected PluginOptionViewModel(IOption option, ILocalizationProvider localizationProvider)
        {
            _localizationProvider = localizationProvider;
            Option = option;

            Option.IsEnabledChanged += Option_IsEnabledChanged;
            Option.IsVisibleChanged += Option_IsVisibleChanged;
            Option.IsLoadingChanged += Option_IsLoadingChanged;
        }

        public string Description => _localizationProvider.Localize(Option.DescriptionTextId);
        public string DisplayName => _localizationProvider.Localize(Option.NameTextId);
        public bool IsEnabled => Option.IsEnabled;
        public bool IsVisible => Option.IsVisible;
        public bool IsLoading => Option.IsLoading;

        protected IOption Option { get; }

        private void Option_IsLoadingChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsLoading));
        }

        private void Option_IsEnabledChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsEnabled));
        }

        private void Option_IsVisibleChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsVisible));
        }

        private readonly ILocalizationProvider _localizationProvider;
    }
}