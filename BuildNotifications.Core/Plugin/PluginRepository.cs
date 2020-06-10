using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using BuildNotifications.PluginInterfacesLegacy.Notification;
using NLog.Fluent;

namespace BuildNotifications.Core.Plugin
{
    internal class PluginRepository : IPluginRepository
    {
        public PluginRepository(IReadOnlyList<IBuildPlugin> build, IReadOnlyList<ISourceControlPlugin> sourceControl,
            IReadOnlyList<INotificationProcessor> notificationProcessors, ITypeMatcher typeMatcher)
        {
            Build = build;
            SourceControl = sourceControl;
            NotificationProcessors = notificationProcessors;
            _typeMatcher = typeMatcher;
        }

        private IPlugin? FindPlugin(string? typeName)
        {
            var plugin = SourceControl.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName))
                         ?? (IPlugin) Build.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));

            if (plugin == null)
                Log.Warn().Message($"Failed to find source control plugin for type \"{typeName}\"").Write();

            return plugin;
        }

        public IReadOnlyList<IBuildPlugin> Build { get; }
        public IReadOnlyList<INotificationProcessor> NotificationProcessors { get; }
        public IReadOnlyList<ISourceControlPlugin> SourceControl { get; }

        public IBuildPlugin? FindBuildPlugin(string? typeName)
        {
            var plugin = Build.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));
            if (plugin == null)
                Log.Warn().Message($"Failed to find build plugin for type \"{typeName}\"").Write();

            return plugin;
        }

        public ISourceControlPlugin? FindSourceControlPlugin(string? typeName)
        {
            var plugin = SourceControl.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));
            if (plugin == null)
                Log.Warn().Message($"Failed to find source control plugin for type \"{typeName}\"").Write();

            return plugin;
        }

        public string? FindPluginName(string typeName)
        {
            var plugin = FindPlugin(typeName);
            return plugin?.DisplayName;
        }

        public string? FindIconGeometry(string typeName)
        {
            var plugin = FindPlugin(typeName);
            return plugin?.IconSvgPath;
        }

        private readonly ITypeMatcher _typeMatcher;
    }
}