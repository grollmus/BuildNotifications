using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Resources.Settings
{
    internal class PluginChooser : Control
    {
        public ConnectionPluginType ConnectionPluginType
        {
            get => (ConnectionPluginType) GetValue(PluginTypeProperty);
            set => SetValue(PluginTypeProperty, value);
        }

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

        public IPlugin? SelectedPlugin
        {
            get => (IPlugin?) GetValue(SelectedPluginProperty);
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
                ctrl.OnPluginTypeChanged();
        }

        private static void OnPluginTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PluginChooser ctrl)
                ctrl.OnPluginTypeChanged();
        }

        private void OnPluginTypeChanged()
        {
            if (PluginRepository != null)
            {
                var plugins = ConnectionPluginType switch
                {
                    ConnectionPluginType.Build => PluginRepository.Build,
                    ConnectionPluginType.SourceControl => PluginRepository.SourceControl,
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
            "ConnectionPluginType", typeof(ConnectionPluginType), typeof(PluginChooser), new PropertyMetadata(default(ConnectionPluginType), OnPluginTypeChanged));

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