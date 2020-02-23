using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginListOptionViewModel<TValue> : PluginValueOptionViewModel<TValue>
    {
        public PluginListOptionViewModel(ListOption<TValue> valueOption, ILocalizationProvider localizationProvider)
            : base(valueOption, localizationProvider)
        {
            ListOption = valueOption;
            _localizationProvider = localizationProvider;

            ListOption.AvailableValuesChanged += ListOption_AvailableValuesChanged;
        }

        public IEnumerable<PluginListOptionItemViewModel<TValue>> AvailableValues => ListOption.AvailableValues.Select(x => new PluginListOptionItemViewModel<TValue>(x, _localizationProvider));

        private void ListOption_AvailableValuesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(AvailableValues));
        }

        private readonly ListOption<TValue> ListOption;
        private readonly ILocalizationProvider _localizationProvider;
    }
}