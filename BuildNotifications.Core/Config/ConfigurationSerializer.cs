using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildNotifications.Core.Utilities;
using JetBrains.Annotations;
using NLog.Fluent;

namespace BuildNotifications.Core.Config
{
    public class ConfigurationSerializer : IConfigurationSerializer
    {
        public ConfigurationSerializer(ISerializer serializer)
        {
            _serializer = serializer;
        }

        private IEnumerable<ConnectionData> LegacyLoadPredefinedConnections(string fileName)
        {
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

        private PredefinedConfigurationContainer LoadPredefinedConfigurationContainer(string fileName)
        {
            try
            {
                var json = File.ReadAllText(fileName);
                var container = _serializer.Deserialize<PredefinedConfigurationContainer>(json);

                return container;
            }
            catch (Exception e)
            {
                Log.Error().Message("Failed to load predefined configuration container.").Exception(e).Write();
                return new PredefinedConfigurationContainer();
            }
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

            return configuration;
        }

        public IEnumerable<ConnectionData> LoadPredefinedConnections(string fileName)
        {
            if (!File.Exists(fileName))
                return Enumerable.Empty<ConnectionData>();

            var container = LoadPredefinedConfigurationContainer(fileName);
            if (!container.Connections.Any())
                return LegacyLoadPredefinedConnections(fileName);

            return container.Connections;
        }

        public IEnumerable<IProjectConfiguration> LoadPredefinedProjects(string fileName)
        {
            if (!File.Exists(fileName))
                return Enumerable.Empty<IProjectConfiguration>();

            var container = LoadPredefinedConfigurationContainer(fileName);
            return container.Projects;
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

        private class PredefinedConfigurationContainer
        {
            [UsedImplicitly]
            public List<ConnectionData> Connections { get; set; } = new List<ConnectionData>();

            [UsedImplicitly]
            public List<ProjectConfiguration> Projects { get; set; } = new List<ProjectConfiguration>();
        }
    }
}