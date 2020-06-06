using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Anotar.NLog;

namespace BuildNotifications.Core.Config
{
    public interface IConfigurationBuilder
    {
        IProjectConfiguration EmptyConfiguration(string name);
    }

    public class ConfigurationBuilder : IConfigurationBuilder
    {
        public ConfigurationBuilder(IPathResolver pathResolver, IConfigurationSerializer configurationSerializer)
        {
            _pathResolver = pathResolver;
            _configurationSerializer = configurationSerializer;
        }

        public IConfiguration LoadConfiguration()
        {
            var configFilePath = _pathResolver.UserConfigurationFilePath;
            LogTo.Info($"Loading configuration. Looking in path: \"{_pathResolver.UserConfigurationFilePath}\"");
            var config = _configurationSerializer.Load(configFilePath);
            var configDirty = false;

            var predefinedFilePath = _pathResolver.PredefinedConfigurationFilePath;
            var predefinedConnections = _configurationSerializer.LoadPredefinedConnections(predefinedFilePath).ToList();
            foreach (var connection in predefinedConnections)
            {
                if (config.Connections.All(c => c.Name != connection.Name))
                {
                    LogTo.Debug($"Adding predefined connection: {connection.Name}");
                    config.Connections.Add(connection);
                    configDirty = true;
                }
            }

            var predefinedConnectionNames = predefinedConnections.Select(p => p.Name).ToList();
            if (!config.Projects.Any(p => AllConnectionsUsedInProject(p, predefinedConnectionNames)) && predefinedConnections.Any())
            {
                var buildConnection = predefinedConnections.First(x => x.ConnectionType == ConnectionPluginType.Build);
                var sourceConnection = predefinedConnections.First(x => x.ConnectionType == ConnectionPluginType.SourceControl);

                var defaultProject = CreateDefaultProject(buildConnection, sourceConnection);
                LogTo.Debug($"Adding project with predefined connections: {buildConnection.Name} and {sourceConnection.Name}");
                config.Projects.Add(defaultProject);
                configDirty = true;
            }

            if (configDirty)
            {
                LogTo.Info("Edited configuration because of predefined connections. Saving new configuration.");
                _configurationSerializer.Save(config, configFilePath);
            }

            LogTo.Info($"Setting language to \"{config.Culture}\"");
            CultureInfo.CurrentUICulture = config.Culture;

            return config;
        }

        private bool AllConnectionsUsedInProject(IProjectConfiguration project, List<string> connectionNames)
        {
            return connectionNames.All(n => project.BuildConnectionNames.Contains(n) || project.SourceControlConnectionName == n);
        }

        private IProjectConfiguration CreateDefaultProject(ConnectionData buildConnection, ConnectionData sourceConnection)
        {
            var project = EmptyConfiguration(buildConnection.Name);

            project.BuildConnectionNames = new List<string> {buildConnection.Name};
            project.SourceControlConnectionName = sourceConnection.Name;

            return project;
        }

        public IProjectConfiguration EmptyConfiguration(string name) => new ProjectConfiguration
        {
            ProjectName = name
        };

        private readonly IPathResolver _pathResolver;
        private readonly IConfigurationSerializer _configurationSerializer;
    }
}