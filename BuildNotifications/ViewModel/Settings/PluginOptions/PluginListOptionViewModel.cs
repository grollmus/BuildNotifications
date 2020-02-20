using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginListOptionViewModel<TValue> : PluginValueOptionViewModel<TValue>
    {
        public PluginListOptionViewModel(ListOption<TValue> valueOption)
            : base(valueOption)
        {
            ListOption = valueOption;
        }

        public IEnumerable<PluginListOptionItemViewModel<TValue>> AvailableValues => ListOption.AvailableValues.Select(x => new PluginListOptionItemViewModel<TValue>(x));

        private readonly ListOption<TValue> ListOption;
    }
}