using System;
using System.Linq;
using System.Reflection;
using BuildNotifications.Core;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions;

internal class PluginOptionViewModelFactory
{
    public PluginOptionViewModelFactory(ILocalizer localizer)
    {
        _constructListOptionMethodInfo = GetType().GetMethod(nameof(ConstructListOption), BindingFlags.NonPublic | BindingFlags.Instance)
                                         ?? throw new Exception("ConstructListOption not found!");

        _localizationProvider = new LocalizationProvider(localizer);
    }

    public IPluginOptionViewModel Construct(IOption option)
    {
        if (option is IListOption)
        {
            var optionType = option.GetType().FindBaseType(typeof(ListOption<>));
            if (optionType != null)
            {
                var tValue = optionType.GetGenericArguments().First();
                var generic = _constructListOptionMethodInfo.MakeGenericMethod(tValue);

                var opt = generic.Invoke(this, new object?[] { option });
                if (opt != null)
                    return (IPluginOptionViewModel)opt;
            }
        }

        return option switch
        {
            BooleanOption bOption => new PluginBooleanOptionViewModel(bOption, _localizationProvider),
            NumberOption nOption => new PluginNumberOptionViewModel(nOption, _localizationProvider),
            TextOption tOption => new PluginTextOptionViewModel(tOption, _localizationProvider),
            CommandOption cOption => new PluginCommandOptionViewModel(cOption, _localizationProvider),
            EncryptedTextOption eOption => new PluginEncryptedTextOptionViewModel(eOption, _localizationProvider),
            StringCollectionOption sOption => new PluginStringCollectionOptionViewModel(sOption, _localizationProvider),
            DisplayOption dOption => new PluginDisplayOptionViewModel(dOption, _localizationProvider),

            _ => ConstructDisplayOption(option)
        };
    }

    private IPluginOptionViewModel ConstructDisplayOption(IOption option) => new PluginDisplayOptionViewModel(option, _localizationProvider);
    private PluginListOptionViewModel<TValue> ConstructListOption<TValue>(ListOption<TValue> option) => new(option, _localizationProvider);
    private readonly MethodInfo _constructListOptionMethodInfo;
    private readonly ILocalizationProvider _localizationProvider;
}