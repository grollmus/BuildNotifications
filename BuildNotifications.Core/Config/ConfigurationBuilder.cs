using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Anotar.NLog;
using BuildNotifications.Core.Text;

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
            LogTo.Info("Loading configuration");
            var configFilePath = _pathResolver.UserConfigurationFilePath;
            var config = _configurationSerializer.Load(configFilePath);

            var predefinedFilePath = _pathResolver.PredefinedConfigurationFilePath;
            var predefinedConnections = _configurationSerializer.LoadPredefinedConnections(predefinedFilePath).ToList();
            foreach (var connection in predefinedConnections)
            {
                if (config.Connections.All(c => c.Name != connection.Name))
                    config.Connections.Add(connection);
            }

            var predefinedConnectionNames = predefinedConnections.Select(p => p.Name).ToList();
            if (!config.Projects.Any(p => AllConnectionsUsedInProject(p, predefinedConnectionNames)) && predefinedConnectionNames.Any())
            {
                var defaultProject = CreateDefaultProject(predefinedConnectionNames);
                config.Projects.Add(defaultProject);
            }

            LogTo.Info($"Setting language to \"{config.Culture}\"");
            CultureInfo.CurrentUICulture = config.Culture;

            return config;
        }

        private bool AllConnectionsUsedInProject(IProjectConfiguration project, List<string> connectionNames)
        {
            return connectionNames.All(n => project.BuildConnectionNames.Contains(n) || project.SourceControlConnectionNames.Contains(n));
        }

        private IProjectConfiguration CreateDefaultProject(List<string> predefinedConnectionNames)
        {
            var project = new ProjectConfiguration();
            project.BuildConnectionNames.Add(predefinedConnectionNames.First());
            project.SourceControlConnectionNames.Add(predefinedConnectionNames.Last());
            project.ProjectName = StringLocalizer.NewProject;

            return project;
        }

        private readonly IPathResolver _pathResolver;
        private readonly IConfigurationSerializer _configurationSerializer;
    }
}