﻿using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
    internal class PluginTextOptionViewModel : TextOptionViewModel, IPluginOptionViewModel
    {
        public PluginTextOptionViewModel(TextOption valueOption, ILocalizationProvider localizationProvider)
            : base(valueOption.Value, valueOption.NameTextId)
        {
            _pluginOptionViewModelImplementation = new PluginOptionViewModelImplementation<string?>(valueOption, localizationProvider, this);
        }

        public override string Description => _pluginOptionViewModelImplementation.Description;
        public override string DisplayName => _pluginOptionViewModelImplementation.DisplayName;

        public override string? Value
        {
            get => _pluginOptionViewModelImplementation.Value;
            set => _pluginOptionViewModelImplementation.Value = value;
        }

        private readonly PluginOptionViewModelImplementation<string?> _pluginOptionViewModelImplementation;
    }
}