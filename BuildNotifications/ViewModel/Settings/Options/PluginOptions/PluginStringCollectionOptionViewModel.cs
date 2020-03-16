using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
    internal class PluginStringCollectionOptionViewModel : StringCollectionOptionViewModel, IPluginOptionViewModel
    {
        public PluginStringCollectionOptionViewModel(StringCollectionOption option, ILocalizationProvider localizationProvider)
            : base(option.NameTextId, option.Value)
        {
            _pluginOptionViewModelImplementation = new PluginOptionViewModelImplementation<List<string>>(option, localizationProvider, this);
        }

        public override string Description => _pluginOptionViewModelImplementation.Description;
        public override string DisplayName => _pluginOptionViewModelImplementation.DisplayName;
        public override bool IsEnabled => _pluginOptionViewModelImplementation.IsEnabled;
        public override bool IsLoading => _pluginOptionViewModelImplementation.IsLoading;
        public override bool IsVisible => _pluginOptionViewModelImplementation.IsVisible;

        protected override void OnCollectionChanged()
        {
            base.OnCollectionChanged();
            _pluginOptionViewModelImplementation.Value = Values.Select(v => v.Value ?? string.Empty).ToList();
        }

        protected override void OnItemChanged()
        {
            base.OnItemChanged();
            _pluginOptionViewModelImplementation.Value = Values.Select(v => v.Value ?? string.Empty).ToList();
        }

        private readonly PluginOptionViewModelImplementation<List<string>> _pluginOptionViewModelImplementation;
    }
}