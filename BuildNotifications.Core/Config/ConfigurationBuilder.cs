using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NLog.Fluent;

namespace BuildNotifications.Core.Config
{
    public class ConfigurationBuilder
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

            var predefinedFilePath = _pathResolver.PredefinedConfigurationFilePath;
            var predefinedConnections = _configurationSerializer.LoadPredefinedConnections(predefinedFilePath).ToList();
            foreach (var connection in predefinedConnections)
            {
                if (config.Connections.All(c => c.Name != connection.Name))
                    config.Connections.Add(connection);
            }

            var predefinedConnectionNames = predefinedConnections.Select(p => p.Name).ToList();
            if (!config.Projects.Any(p => AllConnectionsUsedInProject(p, predefinedConnectionNames)) && predefinedConnections.Any())
            {
                var defaultProject = CreateDefaultProject(predefinedConnections.First());
                config.Projects.Add(defaultProject);
            }

            Log.Info().Message($"Setting language to \"{config.Culture}\"").Write();
            CultureInfo.CurrentUICulture = config.Culture;

            return config;
        }

        private bool AllConnectionsUsedInProject(IProjectConfiguration project, List<string> connectionNames)
        {
            return connectionNames.All(n => project.BuildConnectionNames.Contains(n) || project.SourceControlConnectionNames.Contains(n));
        }

        private IProjectConfiguration CreateDefaultProject(ConnectionData withConnection)
        {
            var project = new ProjectConfiguration();
            var connectionToUse = withConnection;

            project.BuildConnectionNames.Add(withConnection.Name);
            project.SourceControlConnectionNames.Add(withConnection.Name);
            project.ProjectName = connectionToUse.Name;

            return project;
        }

        private readonly IPathResolver _pathResolver;
        private readonly IConfigurationSerializer _configurationSerializer;
    }
}