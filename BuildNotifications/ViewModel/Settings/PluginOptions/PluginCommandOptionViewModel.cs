using System.Windows.Input;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginCommandOptionViewModel : PluginOptionViewModel
    {
        public PluginCommandOptionViewModel(ICommandOption model)
            : base(model)
        {
            Command = new DelegateCommand(x => model.Execute(), x => model.CanExecute());
        }

        public ICommand Command { get; }
    }
}