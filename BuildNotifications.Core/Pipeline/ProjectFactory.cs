using System;
using System.Collections.Generic;
using System.Linq;
using Anotar.NLog;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class ProjectFactory : IProjectFactory
    {
        public ProjectFactory(IPluginRepository pluginRepository, IConfiguration configuration)
        {
            _pluginRepository = pluginRepository;
            _configuration = configuration;
        }

        private IBranchProvider? BranchProvider(string connectionName)
        {
            var connectionData = FindConnection(connectionName);
            if (connectionData == null)
            {
                LogTo.Error($"No connection with name '{connectionName}' found");
                return null;
            }

            var pluginType = connectionData.SourceControlPluginType ?? string.Empty;
            var sourceControlPlugin = _pluginRepository.FindSourceControlPlugin(pluginType);
            if (sourceControlPlugin == null)
            {
                LogTo.Error($"No source control plugin '{pluginType}' found");
                return null;
            }

            IBranchProvider? branchProvider;
            try
            {
                var options = connectionData.SourceControlPluginConfiguration;
                branchProvider = sourceControlPlugin.ConstructProvider(options);
            }
            catch (Exception e)
            {
                LogTo.ErrorException($"Failed to construct branch provider from plugin {sourceControlPlugin.GetType()}", e);
                return null;
            }

            return branchProvider;
        }

        private IBuildProvider? BuildProvider(string connectionName)
        {
            var connectionData = FindConnection(connectionName);
            if (connectionData == null)
            {
                LogTo.Error($"No connection with name '{connectionName}' found");
                return null;
            }

            var pluginType = connectionData.BuildPluginType ?? string.Empty;
            var buildPlugin = _pluginRepository.FindBuildPlugin(pluginType);
            if (buildPlugin == null)
            {
                LogTo.Error($"No build plugin '{pluginType}' found");
                return null;
            }

            IBuildProvider? buildProvider;
            try
            {
                var options = connectionData.BuildPluginConfiguration;
                buildProvider = buildPlugin.ConstructProvider(options);
            }
            catch (Exception e)
            {
                LogTo.ErrorException($"Failed to construct build provider from plugin {buildPlugin.GetType()}", e);
                return null;
            }

            return buildProvider;
        }

        private ConnectionData? FindConnection(string connectionName)
        {
            return _configuration.Connections.FirstOrDefault(c => c.Name == connectionName);
        }

        /// <inheritdoc />
        public IProject? Construct(IProjectConfiguration config)
        {
            LogTo.Debug($"Trying to construct project from {config.BuildConnectionNames} and {config.SourceControlConnectionNames}");

            var buildProviders = new List<IBuildProvider>();
            foreach (var connectionName in config.BuildConnectionNames)
            {
                var buildProvider = BuildProvider(connectionName);
                if (buildProvider == null)
                {
                    LogTo.Error($"Failed to construct build provider for connection {connectionName}");
                    return null;
                }

                buildProviders.Add(buildProvider);
            }

            var branchProviders = new List<IBranchProvider>();
            foreach (var connectionName in config.SourceControlConnectionNames)
            {
                var branchProvider = BranchProvider(connectionName);
                if (branchProvider == null)
                {
                    LogTo.Error($"Failed to construct branch provider for connection {connectionName}");
                    return null;
                }

                branchProviders.Add(branchProvider);
            }

            return new Project(buildProviders, branchProviders, config);
        }

        private readonly IPluginRepository _pluginRepository;
        private readonly IConfiguration _configuration;
    }
}