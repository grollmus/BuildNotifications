using System;
using System.Linq;
using System.Reflection;
using BuildNotifications.Core;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginOptionViewModelFactory
    {
        public PluginOptionViewModelFactory(ILocalizer localizer)
        {
            _constructListOptionMethodInfo = GetType().GetMethod(nameof(ConstructListOption), BindingFlags.NonPublic | BindingFlags.Instance)
                                             ?? throw new Exception("ConstructListOption not found!");

            _localizationProvider = new LocalizationProvider(localizer);
        }

        public PluginOptionViewModel Construct(IOption option)
        {
            if (option is IListOption)
            {
                var optionType = option.GetType().FindBaseType(typeof(ListOption<>));
                if (optionType != null)
                {
                    var tValue = optionType.GetGenericArguments().First();
                    var generic = _constructListOptionMethodInfo.MakeGenericMethod(tValue);

                    var opt = generic.Invoke(this, new object?[] {option});
                    if (opt != null)
                        return (PluginOptionViewModel) opt;
                }
            }

            return option switch
            {
                BooleanOption bOption => ConstructOption(bOption),
                NumberOption nOption => ConstructOption(nOption),
                TextOption tOption => ConstructOption(tOption),
                CommandOption cOption => ConstructOption(cOption),
                EncryptedTextOption eOption => ConstructOption(eOption),

                _ => ConstructDisplayOption(option)
            };
        }

        private PluginEncryptedTextOptionViewModel ConstructOption(EncryptedTextOption option) => new PluginEncryptedTextOptionViewModel(option, _localizationProvider);
        private PluginOptionViewModel ConstructDisplayOption(IOption option) => new PluginDisplayOptionViewModel(option, _localizationProvider);
        private PluginListOptionViewModel<TValue> ConstructListOption<TValue>(ListOption<TValue> option) => new PluginListOptionViewModel<TValue>(option, _localizationProvider);
        private PluginNumberOptionViewModel ConstructOption(NumberOption option) => new PluginNumberOptionViewModel(option, _localizationProvider);
        private PluginTextOptionViewModel ConstructOption(TextOption option) => new PluginTextOptionViewModel(option, _localizationProvider);
        private PluginCommandOptionViewModel ConstructOption(CommandOption option) => new PluginCommandOptionViewModel(option, _localizationProvider);
        private PluginBooleanOptionViewModel ConstructOption(BooleanOption option) => new PluginBooleanOptionViewModel(option, _localizationProvider);
        private readonly MethodInfo _constructListOptionMethodInfo;
        private readonly ILocalizationProvider _localizationProvider;
    }
}