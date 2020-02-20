using System;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginOptionViewModelFactory
    {
        public PluginOptionViewModel Construct(IOption option)
        {
            return option switch
            {
                BooleanOption bOption => ConstructOption(bOption),
                NumberOption nOption => ConstructOption(nOption),
                TextOption tOption => ConstructOption(tOption),
                CommandOption cOption => ConstructOption(cOption),
                
                _ => throw new NotImplementedException()
            };
        }

        private PluginNumberOptionViewModel ConstructOption(NumberOption option) => new PluginNumberOptionViewModel(option);
        private PluginTextOptionViewModel ConstructOption(TextOption option) => new PluginTextOptionViewModel(option);
        private PluginCommandOptionViewModel ConstructOption(CommandOption option) => new PluginCommandOptionViewModel(option);
        private PluginBooleanOptionViewModel ConstructOption(BooleanOption option) => new PluginBooleanOptionViewModel(option);
    }
}