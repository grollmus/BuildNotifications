using System.Globalization;
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

            Log.Info().Message($"Setting language to \"{config.Culture}\"").Write();
            CultureInfo.CurrentUICulture = config.Culture;

            return config;
        }

        public IProjectConfiguration EmptyConfiguration(string name)
        {
            return new ProjectConfiguration
            {
                ProjectName = name
            };
        }

        private readonly IPathResolver _pathResolver;
        private readonly IConfigurationSerializer _configurationSerializer;
    }
}