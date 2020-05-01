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

            LogTo.Info($"Setting language to \"{config.Culture}\"");
            CultureInfo.CurrentUICulture = config.Culture;

            return config;
        }

        private bool AllConnectionsUsedInProject(IProjectConfiguration project, List<string> connectionNames)
        {
            return connectionNames.All(n => project.BuildConnectionName.Contains(n) || project.SourceControlConnectionName == n);
        }

        private IProjectConfiguration CreateDefaultProject(ConnectionData withConnection)
        {
            var project = EmptyConfiguration(withConnection.Name);

            project.BuildConnectionName.Add(withConnection.Name);
            project.SourceControlConnectionName = withConnection.Name;

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
