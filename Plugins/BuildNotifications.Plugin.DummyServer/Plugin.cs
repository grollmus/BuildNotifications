using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Host;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.DummyServer
{
    public class Plugin : ISourceControlPlugin, IBuildPlugin
    {
        private Connection GetConnection(ConfigurationRawData configuration)
        {
            var url = configuration?.Url ?? string.Empty;

            if (Connections.TryGetValue(url, out var connection))
                return connection;

            connection = new Connection(url);
            Connections.Add(url, connection);
            return connection;
        }

        private ConfigurationRawData ParseConfig(string serialized)
        {
            var config = new Configuration();
            if (!config.Deserialize(serialized))
                return null;

            return config.AsRawData();
        }

        private async Task<ConnectionTestResult> TestConnection(ConfigurationRawData data)
        {
            try
            {
                var connection = GetConnection(data);
                var buildProvider = new BuildProvider(connection);
                var definitionsEnumerable = buildProvider.FetchExistingBuildDefinitions();
                await foreach (var _ in definitionsEnumerable)
                {
                    return ConnectionTestResult.Success;
                }
            }
            catch (Exception ex)
            {
                return ConnectionTestResult.Failure(ex.Message);
            }

            return ConnectionTestResult.Success;
        }

        IBuildProvider IBuildPlugin.ConstructProvider(string data)
        {
            var connection = GetConnection(ParseConfig(data));
            return new BuildProvider(connection);
        }

        Task<ConnectionTestResult> IBuildPlugin.TestConnection(string data) => TestConnection(ParseConfig(data));

        public IPluginConfiguration ConstructNewConfiguration() => new Configuration();

        public string DisplayName => "Dummy Server";

        public string IconSvgPath => "F1 M64,64z M0,0z M32.05,32L0,32Q0,45.3 9.35,54.6 18.75,64 32,64L32.05,64 32.05,32 M64,32Q64,18.75 54.6,9.35 45.3173828125,0.017578125 32.05,0L32.05,32 64,32z";

        public void OnPluginLoaded(IPluginHost host)
        {
        }

        Task<ConnectionTestResult> ISourceControlPlugin.TestConnection(string data) => TestConnection(ParseConfig(data));

        public IBranchProvider ConstructProvider(string data)
        {
            var connection = GetConnection(ParseConfig(data));
            return new SourceControlProvider(connection);
        }

        private static readonly Dictionary<string, Connection> Connections = new Dictionary<string, Connection>();
    }
}