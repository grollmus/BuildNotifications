using System;
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
                return null;
            }

            var pluginType = connectionData.PluginType;
            var sourceControlPlugin = _pluginRepository.FindSourceControlPlugin(pluginType);
            if (sourceControlPlugin == null)
            {
                return null;
            }

            IBranchProvider? branchProvider;
            try
            {
                var options = connectionData.Options;
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
                return null;
            }

            var pluginType = connectionData.PluginType;
            var buildPlugin = _pluginRepository.FindBuildPlugin(pluginType);
            if (buildPlugin == null)
            {
                return null;
            }

            IBuildProvider? buildProvider;
            try
            {
                var options = connectionData.Options;
                buildProvider = buildPlugin.ConstructProvider(options);
            }
            catch (Exception e)
            {
                LogTo.ErrorException($"Failed to construct build provider from plugin {buildPlugin.GetType()}", e);
                return null;
            }

            return buildProvider;
        }

        private IConnectionData? FindConnection(string connectionName)
        {
            return _configuration.Connections.FirstOrDefault(c => c.Name == connectionName);
        }

        /// <inheritdoc />
        public IProject? Construct(IProjectConfiguration config)
        {
            LogTo.Debug($"Trying to construct project from {config.BuildConnectionName} and {config.SourceControlConnectionName}");

            var buildProvider = BuildProvider(config.BuildConnectionName);
            if (buildProvider == null)
            {
                LogTo.Error("Failed to construct build provider");
                return null;
            }

            var branchProvider = BranchProvider(config.SourceControlConnectionName);
            if (branchProvider == null)
            {
                LogTo.Error("Failed to construct branch provider");
                return null;
            }

            return new Project(buildProvider, branchProvider, config);
        }

        private readonly IPluginRepository _pluginRepository;
        private readonly IConfiguration _configuration;
    }
}