using System.Windows;
using ReflectSettings.EditableConfigs;

namespace BuildNotifications.Resources.Settings
{
    internal class ConnectionsAndProjectsTemplateSelector : EditableConfigTemplateSelector
    {
        public new static ConnectionsAndProjectsTemplateSelector Instance { get; } = new ConnectionsAndProjectsTemplateSelector();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            var template = ResolveTemplate(item, element);
            return template ?? base.SelectTemplate(item, container);
        }

        private static DataTemplate? ResolveTemplate(object item, FrameworkElement? element)
        {
            switch (item)
            {
                case IEditableCollection editableCollection:
                    if (!editableCollection.SubItemType.IsPrimitive && editableCollection.SubItemType != typeof(string))
                        return element?.TryFindResource("ConnectionsAndProjectsEditableCollectionTemplate") as DataTemplate;
                    break;
            }

            return null;
        }
    }
}