using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Configuration configuration;
            if (File.Exists(fileName))
            {
                try
                {
                    var json = File.ReadAllText(fileName);
                    configuration = _serializer.Deserialize<Configuration>(json);
                }
                catch (Exception e)
                {
                    LogTo.InfoException("Failed to load existing config.", e);
                    configuration = new Configuration();
                }
            }
            else
            {
                LogTo.Info($"File {fileName} does not exist. Using default configuration");
                configuration = new Configuration();
            }

            return configuration;
        }

        public IEnumerable<ConnectionData> LoadPredefinedConnections(string fileName)
        {
            if (!File.Exists(fileName))
                return Enumerable.Empty<ConnectionData>();

            try
            {
                var json = File.ReadAllText(fileName);
                var list = _serializer.Deserialize<List<ConnectionData>>(json);

                return list;
            }
            catch (Exception e)
            {
                LogTo.ErrorException("Failed to load predefined connections.", e);
                return Enumerable.Empty<ConnectionData>();
            }
        }

        public void Save(IConfiguration configuration, string fileName)
        {
            var json = _serializer.Serialize(configuration);
            var directory = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                LogTo.Info($"Creating directory for config \"{directory}\" as it does not exist.");
            }

            LogTo.Info("Saving current configuration.");
            try
            {
                LogTo.Debug($"Writing to path \"{fileName}\".");
                File.WriteAllText(fileName, json);
            }
            catch (Exception e)
            {
                LogTo.FatalException("Failed to persist configuration.", e);
            }
        }

        private readonly ISerializer _serializer;
    }
}