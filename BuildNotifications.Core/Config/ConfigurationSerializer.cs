using System.IO;
using System.Linq;
using Anotar.NLog;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Utilities;

namespace BuildNotifications.Core.Config
{
    public class ConfigurationSerializer : IConfigurationSerializer
    {
        public ConfigurationSerializer(ISerializer serializer, IPluginRepository pluginRepository)
        {
            _serializer = serializer;
            _pluginRepository = pluginRepository;
        }

        public IConfiguration Load(string fileName)
        {
            Configuration configuration;
            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);
                configuration = _serializer.Deserialize<Configuration>(json);
            }else
            {
                LogTo.Warn($"File {fileName} does not exist. Using default configuration");
                configuration = new Configuration();
            }

            configuration.PossibleBuildPluginsFunction = () => _pluginRepository.Build.Select(x => x.GetType().FullName);
            configuration.PossibleSourceControlPluginsFunction = () => _pluginRepository.SourceControl.Select(x => x.GetType().FullName);

            return configuration;
        }

        public void Save(IConfiguration configuration, string fileName)
        {
            var json = _serializer.Serialize(configuration);
            File.WriteAllText(fileName, json);
        }

        private readonly ISerializer _serializer;
        private readonly IPluginRepository _pluginRepository;
    }
}