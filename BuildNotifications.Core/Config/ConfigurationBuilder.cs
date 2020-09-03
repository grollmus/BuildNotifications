using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NLog.Fluent;

namespace BuildNotifications.Core.Config
{
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
            Log.Info().Message($"Loading configuration. Looking in path: \"{_pathResolver.UserConfigurationFilePath}\"").Write();
            var config = _configurationSerializer.Load(configFilePath);
            var configDirty = false;

            var predefinedFilePath = _pathResolver.PredefinedConfigurationFilePath;
            var predefinedConnections = _configurationSerializer.LoadPredefinedConnections(predefinedFilePath).ToList();
            foreach (var connection in predefinedConnections)
            {
                if (connection.PluginType == null || connection.PluginConfiguration == null)
                {
                    Log.Warn().Message($"Connection '{connection.Name}' contains no data.").Write();
                    continue;
                }

                if (config.Connections.All(c => c.Name != connection.Name))
                {
                    Log.Debug().Message($"Adding predefined connection: {connection.Name}").Write();
                    config.Connections.Add(connection);
                    configDirty = true;
                }
            }

            var predefinedProjects = _configurationSerializer.LoadPredefinedProjects(predefinedFilePath).ToList();
            if (predefinedProjects.Any())
            {
                foreach (var predefinedProject in predefinedProjects)
                {
                    if (config.Projects.All(p => p.ProjectName != predefinedProject.ProjectName))
                    {
                        Log.Debug().Message($"Adding predefined project: {predefinedProject.ProjectName}").Write();
                        config.Projects.Add(predefinedProject);
                        configDirty = true;
                    }
                }
            }
            else
            {
                var predefinedConnectionNames = predefinedConnections.Select(p => p.Name).ToList();
                if (!config.Projects.Any(p => AllConnectionsUsedInProject(p, predefinedConnectionNames)) && predefinedConnections.Any())
                {
                    var buildConnection = predefinedConnections.FirstOrDefault(x => x.ConnectionType == ConnectionPluginType.Build);
                    var sourceConnection = predefinedConnections.FirstOrDefault(x => x.ConnectionType == ConnectionPluginType.SourceControl);

                    if (buildConnection != null && sourceConnection != null)
                    {
                        var defaultProject = CreateDefaultProject(buildConnection, sourceConnection);
                        Log.Debug().Message($"Adding project with predefined connections: {buildConnection.Name} and {sourceConnection.Name}").Write();
                        config.Projects.Add(defaultProject);
                        configDirty = true;
                    }
                }
            }

            if (configDirty)
            {
                Log.Info().Message("Edited configuration because of predefined connections. Saving new configuration.").Write();
                _configurationSerializer.Save(config, configFilePath);
            }

            Log.Info().Message($"Setting language to \"{config.Culture}\"").Write();
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