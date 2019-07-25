using System.IO;
using Anotar.NLog;
using BuildNotifications.Core.Utilities;

namespace BuildNotifications.Core.Config
{
    public class ConfigurationSerializer : IConfigurationSerializer
    {
        public ConfigurationSerializer(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public IConfiguration Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);
                return _serializer.Deserialize<Configuration>(json);
            }

            LogTo.Warn($"File {fileName} does not exist. Using default configuration");
            return new Configuration();
        }

        public void Save(IConfiguration configuration, string fileName)
        {
            var json = _serializer.Serialize(configuration);
            File.WriteAllText(fileName, json);
        }

        private readonly ISerializer _serializer;
    }
}