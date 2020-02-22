using System;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal abstract class PluginOptionViewModel : BaseViewModel
    {
        protected PluginOptionViewModel(IOption option, ILocalizationProvider localizationProvider)
        {
            _localizationProvider = localizationProvider;
            Option = option;

            Option.IsEnabledChanged += Option_IsEnabledChanged;
            Option.IsVisibleChanged += Option_IsVisibleChanged;
        }

        public string Description => Option.DescriptionTextId;
        public string DisplayName => _localizationProvider.Localize(Option.NameTextId);
        public bool IsEnabled => Option.IsEnabled;
        public bool IsVisible => Option.IsVisible;

        protected IOption Option { get; }

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