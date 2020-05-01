using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using BuildNotifications.Core;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
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

                    var opt = generic.Invoke(this, new object?[] {option});
                    if (opt != null)
                        return (IPluginOptionViewModel) opt;
                }
            }

            Binder? binder = null;
            var args = new object?[] {option, _localizationProvider};
            const BindingFlags bindingAttributes = BindingFlags.CreateInstance;
            CultureInfo? culture = null;
            return Activator.CreateInstance(option.GetType(), bindingAttributes, binder, args, culture) as IPluginOptionViewModel
                   ?? ConstructDisplayOption(option);
        }

        private IPluginOptionViewModel ConstructDisplayOption(IOption option) => new PluginDisplayOptionViewModel(option, _localizationProvider);
        private PluginListOptionViewModel<TValue> ConstructListOption<TValue>(ListOption<TValue> option) => new PluginListOptionViewModel<TValue>(option, _localizationProvider);
        private readonly MethodInfo _constructListOptionMethodInfo;
        private readonly ILocalizationProvider _localizationProvider;
    }
}