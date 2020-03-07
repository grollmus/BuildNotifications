using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
    internal class PluginListOptionViewModel<TValue> : ListOptionBaseViewModel<TValue>, IPluginOptionViewModel
    {
        public PluginListOptionViewModel(ListOption<TValue> valueOption, ILocalizationProvider localizationProvider)
            : base(valueOption.NameTextId, valueOption.Value)
        {
            _pluginOptionViewModelImplementation = new PluginOptionViewModelImplementation<TValue>(valueOption, localizationProvider, this);

            _listOption = valueOption;
            _listOption.AvailableValuesChanged += ListOption_AvailableValuesChanged;
        }

        public override string Description => _pluginOptionViewModelImplementation.Description;
        public override string DisplayName => _pluginOptionViewModelImplementation.DisplayName;

        public override TValue Value
        {
            get => _pluginOptionViewModelImplementation.Value;
            set => _pluginOptionViewModelImplementation.Value = value;
        }

        protected override IEnumerable<TValue> ModelValues
        {
            get { return _listOption.AvailableValues.Select(x => x.Value); }
        }

        protected override string? DisplayNameFor(TValue item)
        {
            var optionValue = _listOption.AvailableValues.FirstOrDefault(v => Equals(v.Value, item));
            if (optionValue != null)
                return optionValue.DisplayName;

            return base.DisplayNameFor(item);
        }

        private void ListOption_AvailableValuesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(AvailableValues));
        }

        private readonly ListOption<TValue> _listOption;
        private readonly PluginOptionViewModelImplementation<TValue> _pluginOptionViewModelImplementation;
    }
}