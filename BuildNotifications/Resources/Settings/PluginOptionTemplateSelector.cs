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
            if (item != null)
            {
                var element = container as FrameworkElement;
                var itemType = item.GetType();

                var listOptionBaseType = typeof(PluginListOptionViewModel<>);

                if (itemType.IsAssignableToGenericType(listOptionBaseType))
                    return element?.TryFindResource("PluginListOptionTemplate") as DataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}