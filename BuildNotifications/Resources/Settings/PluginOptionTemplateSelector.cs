using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Core;
using BuildNotifications.ViewModel.Settings.Setup.PluginOptions;

namespace BuildNotifications.Resources.Settings
{
    internal class PluginOptionTemplateSelector : DataTemplateSelector
    {
        public static PluginOptionTemplateSelector Instance { get; } = new PluginOptionTemplateSelector();

        public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            var simpleType = SelectTemplateSimple(item, element);
            if (simpleType != null)
                return simpleType;

            if (item != null)
            {
                var itemType = item.GetType();

                var listOptionBaseType = typeof(PluginListOptionViewModel<>);

                if (itemType.IsAssignableToGenericType(listOptionBaseType))
                    return element?.TryFindResource("PluginListOptionTemplate") as DataTemplate;
            }

            return base.SelectTemplate(item, container);
        }

        private DataTemplate? SelectTemplateSimple(object? item, FrameworkElement? container)
        {
            return item switch
            {
                PluginCommandOptionViewModel _ => (container?.TryFindResource("PluginCommandOptionTemplate") as DataTemplate),
                PluginEncryptedTextOptionViewModel _ => (container?.TryFindResource("PluginEncryptedTextOptionTemplate") as DataTemplate),
                PluginNumberOptionViewModel _ => (container?.TryFindResource("PluginNumberOptionTemplate") as DataTemplate),
                PluginTextOptionViewModel _ => (container?.TryFindResource("PluginTextOptionTemplate") as DataTemplate),
                PluginDisplayOptionViewModel _ => (container?.TryFindResource("PluginDisplayOptionTemplate") as DataTemplate),
                _ => null
            };
        }
    }
}