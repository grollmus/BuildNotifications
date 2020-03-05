using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.Setup.PluginOptions
{
    internal class PluginCommandOptionViewModel : PluginOptionViewModel
    {
        public PluginCommandOptionViewModel(ICommandOption model, ILocalizationProvider localizationProvider)
            : base(model, localizationProvider)
        {
            Command = AsyncCommand.Create(model.Execute, model.CanExecute);
        }

        public AsyncCommand<object?> Command { get; }
    }
}