using System;
using System.Collections.Generic;
using System.Linq;
using Anotar.NLog;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Plugin
{
    internal class PluginRepository : IPluginRepository
    {
        /// <inheritdoc />
        public PluginRepository(IReadOnlyList<IBuildPlugin> build, IReadOnlyList<ISourceControlPlugin> sourceControl, ITypeMatcher typeMatcher)
        {
            Build = build;
            SourceControl = sourceControl;
            _typeMatcher = typeMatcher;
        }

        /// <inheritdoc />
        public IReadOnlyList<IBuildPlugin> Build { get; }

        /// <inheritdoc />
        public IReadOnlyList<ISourceControlPlugin> SourceControl { get; }

        /// <inheritdoc />
        public IBuildPlugin? FindBuildPlugin(string? typeName)
        {
            var plugin = Build.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));
            if (plugin == null)
                LogTo.Warn($"Failed to find build plugin for type {typeName}");

            return plugin;
        }

        /// <inheritdoc />
        public ISourceControlPlugin? FindSourceControlPlugin(string? typeName)
        {
            var plugin = SourceControl.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));
            if (plugin == null)
                LogTo.Warn($"Failed to find source control plugin for type {typeName}");

            return plugin;
        }

        public string? FindPluginName(string typeName)
        {
            IPlugin plugin = SourceControl.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));

            if (plugin == null)
                plugin = Build.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));

            if (plugin == null)
                LogTo.Warn($"Failed to find source control plugin for type {typeName}");

            return plugin?.DisplayName;
        }

        public string? FindIconGeometry(string typeName)
        {
            IPlugin plugin = SourceControl.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));

            if (plugin == null)
                plugin = Build.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));

            if (plugin == null)
                LogTo.Warn($"Failed to find source control plugin for type {typeName}");

            return plugin?.IconSvgPath;
        }

        public Type? FindConfigurationType(string typeName)
        {
            IPlugin plugin = SourceControl.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));

            if(plugin == null)
                plugin = Build.FirstOrDefault(t => _typeMatcher.MatchesType(t.GetType(), typeName));

            if (plugin == null)
                LogTo.Warn($"Failed to find source control plugin for type {typeName}");

            return plugin?.GetConfigurationType();
        }

        public event EventHandler<ErrorNotificationEventArgs> ErrorOccured;

        private readonly ITypeMatcher _typeMatcher;
    }
}