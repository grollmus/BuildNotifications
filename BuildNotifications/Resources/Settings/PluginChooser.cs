using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Resources.Settings
{
    internal enum PluginType
    {
        Build,
        SourceControl
    }

    internal class PluginChooser : Control
    {
        public IPluginRepository PluginRepository
        {
            get => (IPluginRepository) GetValue(PluginRepositoryProperty);
            set => SetValue(PluginRepositoryProperty, value);
        }

        public IEnumerable<IPlugin> Plugins
        {
            get => (IEnumerable<IPlugin>) GetValue(PluginsProperty);
            private set => SetValue(PluginsKey, value);
        }

        public PluginType PluginType
        {
            get => (PluginType) GetValue(PluginTypeProperty);
            set => SetValue(PluginTypeProperty, value);
        }

        public IPlugin SelectedPlugin
        {
            get => (IPlugin) GetValue(SelectedPluginProperty);
            set => SetValue(SelectedPluginProperty, value);
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private static void OnPluginRepositoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PluginChooser ctrl)
                ctrl.RefreshItems();
        }

        private static void OnPluginTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PluginChooser ctrl)
                ctrl.RefreshItems();
        }

        private void RefreshItems()
        {
            if (PluginRepository != null)
            {
                var plugins = PluginType switch
                {
                    PluginType.Build => PluginRepository.Build,
                    PluginType.SourceControl => PluginRepository.SourceControl,
                    _ => Enumerable.Empty<IPlugin>()
                };

                Plugins = new ObservableCollection<IPlugin>(plugins);
            }
            else
                Plugins = new ObservableCollection<IPlugin>();
        }

        public static readonly DependencyProperty PluginRepositoryProperty = DependencyProperty.Register(
            "PluginRepository", typeof(IPluginRepository), typeof(PluginChooser), new PropertyMetadata(default(IPluginRepository), OnPluginRepositoryChanged));

        public static readonly DependencyProperty PluginTypeProperty = DependencyProperty.Register(
            "PluginType", typeof(PluginType), typeof(PluginChooser), new PropertyMetadata(default(PluginType), OnPluginTypeChanged));

        private static readonly DependencyPropertyKey PluginsKey
            = DependencyProperty.RegisterReadOnly("Plugins", typeof(IEnumerable<IPlugin>), typeof(PluginChooser),
                new FrameworkPropertyMetadata(default(IEnumerable<IPlugin>), FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty PluginsProperty
            = PluginsKey.DependencyProperty;

        public static readonly DependencyProperty SelectedPluginProperty = DependencyProperty.Register(
            "SelectedPlugin", typeof(IPlugin), typeof(PluginChooser), new FrameworkPropertyMetadata(default(IPlugin), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(PluginChooser), new PropertyMetadata(default(string)));
    }
}