using System;
using System.Collections.Generic;
using System.Linq;
using Anotar.NLog;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Text;
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
                ReportError("ConnectionNotFound", connectionName);
                return null;
            }

            var pluginType = connectionData.PluginType ?? string.Empty;
            var sourceControlPlugin = _pluginRepository.FindSourceControlPlugin(pluginType);
            if (sourceControlPlugin == null)
            {
                ReportError("SourceControlPluginNotFound", pluginType);
                return null;
            }

            IBranchProvider? branchProvider;
            try
            {
                var options = connectionData.PluginConfiguration;
                branchProvider = sourceControlPlugin.ConstructProvider(options ?? string.Empty);
            }
            catch (Exception e)
            {
                ReportError("FailedToConstructBranchProviderFromPlugin", sourceControlPlugin.GetType(), e);
                return null;
            }

            return branchProvider;
        }

        private IBuildProvider? BuildProvider(string connectionName)
        {
            var connectionData = FindConnection(connectionName);
            if (connectionData == null)
            {
                ReportError("ConnectionNotFound", connectionName);
                return null;
            }

            var pluginType = connectionData.PluginType ?? string.Empty;
            var buildPlugin = _pluginRepository.FindBuildPlugin(pluginType);
            if (buildPlugin == null)
            {
                ReportError("BuildPluginNotFound", pluginType);
                return null;
            }

            IBuildProvider? buildProvider;
            try
            {
                var options = connectionData.PluginConfiguration;
                buildProvider = buildPlugin.ConstructProvider(options ?? string.Empty);
            }
            catch (Exception e)
            {
                ReportError("FailedToConstructBuildProviderFromPlugin", buildPlugin.GetType(), e);
                return null;
            }

            return buildProvider;
        }

        private ConnectionData? FindConnection(string connectionName)
        {
            return _configuration.Connections.FirstOrDefault(c => c.Name == connectionName);
        }

        private void ReportError(string messageTextId, params object[] parameter)
        {
            var localizedMessage = StringLocalizer.Instance.GetText(messageTextId);
            var fullMessage = string.Format(StringLocalizer.CurrentCulture, localizedMessage, parameter);
            if (parameter.FirstOrDefault(x => x is Exception) is Exception exception)
                LogTo.ErrorException(fullMessage, exception);
            else
                LogTo.Error(fullMessage);

            ErrorOccured?.Invoke(this, new ErrorNotificationEventArgs());
        }

        public IProject? Construct(IProjectConfiguration config)
        {
            var buildConnectionNames = string.Join(", ", config.BuildConnectionNames);
            LogTo.Debug($"Trying to construct project from {buildConnectionNames} and {config.SourceControlConnectionName}");

            var buildProviders = new List<IBuildProvider>();
            foreach (var connectionName in config.BuildConnectionNames)
            {
                var buildProvider = BuildProvider(connectionName);
                if (buildProvider == null)
                {
                    ReportError("FailedToConstructBuildProviderForConnection", connectionName);
                    return null;
                }

                buildProviders.Add(buildProvider);
            }

            IBranchProvider? branchProvider = null;
            if (!string.IsNullOrEmpty(config.SourceControlConnectionName))
                branchProvider = BranchProvider(config.SourceControlConnectionName);

            if (branchProvider != null)
                return new Project(buildProviders, branchProvider, config);

            ReportError("FailedToConstructBranchProviderForConnection", config.SourceControlConnectionName);
            return null;
        }

        public event EventHandler<ErrorNotificationEventArgs>? ErrorOccured;

        private readonly IPluginRepository _pluginRepository;
        private readonly IConfiguration _configuration;
    }
}