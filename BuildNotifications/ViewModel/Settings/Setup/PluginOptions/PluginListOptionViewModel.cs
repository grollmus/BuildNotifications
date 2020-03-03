using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Setup.PluginOptions
{
    internal class PluginListOptionViewModel<TValue> : PluginValueOptionViewModel<TValue>
    {
        public PluginListOptionViewModel(ListOption<TValue> valueOption, ILocalizationProvider localizationProvider)
            : base(valueOption, localizationProvider)
        {
            _listOption = valueOption;
            _localizationProvider = localizationProvider;

            _listOption.AvailableValuesChanged += ListOption_AvailableValuesChanged;
        }

        public IEnumerable<PluginListOptionItemViewModel<TValue>> AvailableValues => _listOption.AvailableValues.Select(x => new PluginListOptionItemViewModel<TValue>(x));

        private void ListOption_AvailableValuesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(AvailableValues));
        }

        private readonly ListOption<TValue> _listOption;
        private readonly ILocalizationProvider _localizationProvider;
    }
}