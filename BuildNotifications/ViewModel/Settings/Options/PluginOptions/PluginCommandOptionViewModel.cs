using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
    internal class PluginCommandOptionViewModel : OptionViewModelBase, IPluginOptionViewModel
    {
        public PluginCommandOptionViewModel(ICommandOption model, ILocalizationProvider localizationProvider)
            : base(model.NameTextId, model.DescriptionTextId)
        {
            _pluginOptionViewModelImplementation = new PluginOptionViewModelImplementation(model, localizationProvider, this);
            Command = AsyncCommand.Create(model.Execute, model.CanExecute);
        }

        public AsyncCommand<object?> Command { get; }

        public override string Description => _pluginOptionViewModelImplementation.Description;
        public override string DisplayName => _pluginOptionViewModelImplementation.DisplayName;
        public override bool IsEnabled => _pluginOptionViewModelImplementation.IsEnabled;
        public override bool IsLoading => _pluginOptionViewModelImplementation.IsLoading;
        public override bool IsVisible => _pluginOptionViewModelImplementation.IsVisible;

        public void Clear()
        {
            _pluginOptionViewModelImplementation.Clear();
        }

        private readonly PluginOptionViewModelImplementation _pluginOptionViewModelImplementation;
    }
}