using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Core;
using BuildNotifications.ViewModel.Settings.Options;
using BuildNotifications.ViewModel.Settings.Options.PluginOptions;

namespace BuildNotifications.Resources.Settings
{
    internal class OptionTemplateSelector : DataTemplateSelector
    {
        public static OptionTemplateSelector Instance { get; } = new OptionTemplateSelector();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            return (TryFindTemplateSimple(item, element)
                    ?? TryFindTemplateGeneric(item, element)
                    ?? base.SelectTemplate(item, container))!;
        }

        private DataTemplate? BooleanOptionTemplate(FrameworkElement? element) => element?.TryFindResource(nameof(BooleanOptionTemplate)) as DataTemplate;
        private DataTemplate? NumberOptionTemplate(FrameworkElement? element) => element?.TryFindResource(nameof(NumberOptionTemplate)) as DataTemplate;
        private DataTemplate? PluginCommandOptionTemplate(FrameworkElement? element) => element?.TryFindResource(nameof(PluginCommandOptionTemplate)) as DataTemplate;
        private DataTemplate? PluginDisplayOptionTemplate(FrameworkElement? element) => element?.TryFindResource(nameof(PluginDisplayOptionTemplate)) as DataTemplate;
        private DataTemplate? PluginEncryptedTextOptionTemplate(FrameworkElement? element) => element?.TryFindResource(nameof(PluginEncryptedTextOptionTemplate)) as DataTemplate;
        private DataTemplate? StringCollectionOptionTemplate(FrameworkElement? element) => element?.TryFindResource(nameof(StringCollectionOptionTemplate)) as DataTemplate;
        private DataTemplate? TextOptionTemplate(FrameworkElement? element) => element?.TryFindResource(nameof(TextOptionTemplate)) as DataTemplate;

        private DataTemplate? TryFindTemplateGeneric(object item, FrameworkElement? element)
        {
            var itemType = item.GetType();

            var listOptionBaseType = typeof(ListOptionBaseViewModel<>);

            if (itemType.IsAssignableToGenericType(listOptionBaseType))
                return element?.TryFindResource("ListOptionTemplate") as DataTemplate;

            return null;
        }

        private DataTemplate? TryFindTemplateSimple(object item, FrameworkElement? element)
        {
            return item switch
            {
                BooleanOptionViewModel _ => BooleanOptionTemplate(element),
                NumberOptionViewModel _ => NumberOptionTemplate(element),
                TextOptionViewModel _ => TextOptionTemplate(element),
                PluginEncryptedTextOptionViewModel _ => PluginEncryptedTextOptionTemplate(element),
                StringCollectionOptionViewModel _ => StringCollectionOptionTemplate(element),
                PluginCommandOptionViewModel _ => PluginCommandOptionTemplate(element),
                PluginDisplayOptionViewModel _ => PluginDisplayOptionTemplate(element),
                _ => null
            };
        }
    }
}