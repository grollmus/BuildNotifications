using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Core;
using BuildNotifications.ViewModel.Settings.Options;

namespace BuildNotifications.Resources.Settings
{
    internal class OptionTemplateSelector : DataTemplateSelector
    {
        public static OptionTemplateSelector Instance { get; } = new OptionTemplateSelector();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            return TryFindTemplateSimple(item, element)
                   ?? TryFindTemplateGeneric(item, element)
                   ?? base.SelectTemplate(item, container);
        }

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
                BooleanOptionViewModel _ => (element?.TryFindResource("BooleanOptionTemplate") as DataTemplate),
                NumberOptionViewModel _ => (element?.TryFindResource("NumberOptionTemplate") as DataTemplate),
                TextOptionViewModel _ => (element?.TryFindResource("TextOptionTemplate") as DataTemplate),
                StringCollectionOptionViewModel _ => (element?.TryFindResource("StringCollectionOptionTemplate") as DataTemplate),
                _ => null
            };
        }
    }
}