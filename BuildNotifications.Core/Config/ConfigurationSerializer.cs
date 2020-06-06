using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Utilities;
using NLog.Fluent;

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
                try
                {
                    var json = File.ReadAllText(fileName);
                    configuration = _serializer.Deserialize<Configuration>(json);
                }
                catch (Exception e)
                {
                    Log.Info().Message("Failed to load existing config").Exception(e).Write();
                    configuration = new Configuration();
                }
            }
            else
            {
                Log.Info().Message($"File {fileName} does not exist. Using default configuration").Write();
                configuration = new Configuration();
            }

            configuration.PossibleBuildPluginsFunction = () => _pluginRepository.Build.Select(x => x.GetType().FullName);
            configuration.PossibleSourceControlPluginsFunction = () => _pluginRepository.SourceControl.Select(x => x.GetType().FullName);
            configuration.PluginRepository = _pluginRepository;

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
                Log.Error().Message("Failed to load predefined connections.").Exception(e).Write();
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
                Log.Info().Message($"Creating directory for config \"{directory}\" as it does not exist.").Write();
            }

            Log.Info().Message("Saving current configuration.").Write();
            try
            {
                Log.Debug().Message($"Writing to path \"{fileName}\".").Write();
                File.WriteAllText(fileName, json);
            }
            catch (Exception e)
            {
                Log.Fatal().Message("Failed to persist configuration.").Exception(e).Write();
            }
        }

        private readonly ISerializer _serializer;
        private readonly IPluginRepository _pluginRepository;
    }
}