using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;
using JetBrains.Annotations;

namespace BuildNotifications.Resources.Settings;

internal class PluginChooser : Control
{
    public ConnectionPluginType ConnectionPluginType
    {
        get => (ConnectionPluginType)GetValue(PluginTypeProperty);
        set => SetValue(PluginTypeProperty, value);
    }

    public IPluginRepository? PluginRepository
    {
        get => (IPluginRepository)GetValue(PluginRepositoryProperty);
        set => SetValue(PluginRepositoryProperty, value);
    }

    public ObservableCollection<IPlugin> Plugins
    {
        get => (ObservableCollection<IPlugin>)GetValue(PluginsProperty);
        [UsedImplicitly] private set => SetValue(PluginsKey, value);
    }

    public IPlugin? SelectedPlugin
    {
        get => (IPlugin?)GetValue(SelectedPluginProperty);
        set => SetValue(SelectedPluginProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
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
        var oldSelected = SelectedPlugin;
        Plugins.Clear();

        if (PluginRepository != null)
        {
            var plugins = ConnectionPluginType switch
            {
                ConnectionPluginType.Build => PluginRepository.Build,
                ConnectionPluginType.SourceControl => PluginRepository.SourceControl,
                _ => Enumerable.Empty<IPlugin>()
            };

            foreach (var plugin in plugins)
            {
                Plugins.Add(plugin);
            }
        }

        SelectedPlugin = Plugins.FirstOrDefault(p => p.GetType() == oldSelected?.GetType());
    }

    public static readonly DependencyProperty PluginRepositoryProperty = DependencyProperty.Register(
        "PluginRepository", typeof(IPluginRepository), typeof(PluginChooser), new PropertyMetadata(default(IPluginRepository), OnPluginRepositoryChanged));

    public static readonly DependencyProperty PluginTypeProperty = DependencyProperty.Register(
        "ConnectionPluginType", typeof(ConnectionPluginType), typeof(PluginChooser), new PropertyMetadata(default(ConnectionPluginType), OnPluginTypeChanged));

    private static readonly DependencyPropertyKey PluginsKey
        = DependencyProperty.RegisterReadOnly("Plugins", typeof(ObservableCollection<IPlugin>), typeof(PluginChooser),
            new FrameworkPropertyMetadata(new ObservableCollection<IPlugin>(), FrameworkPropertyMetadataOptions.None));

    public static readonly DependencyProperty PluginsProperty
        = PluginsKey.DependencyProperty;

    public static readonly DependencyProperty SelectedPluginProperty = DependencyProperty.Register(
        "SelectedPlugin", typeof(IPlugin), typeof(PluginChooser), new FrameworkPropertyMetadata(default(IPlugin), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        "Title", typeof(string), typeof(PluginChooser), new PropertyMetadata(default(string)));
}