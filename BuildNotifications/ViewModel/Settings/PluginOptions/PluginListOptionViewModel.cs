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
        }

        public IEnumerable<PluginListOptionItemViewModel<TValue>> AvailableValues => ListOption.AvailableValues.Select(x => new PluginListOptionItemViewModel<TValue>(x, _localizationProvider));

        private readonly ListOption<TValue> ListOption;
        private readonly ILocalizationProvider _localizationProvider;
    }
}