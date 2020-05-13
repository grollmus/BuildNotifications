using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Host;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    public class Plugin : ISourceControlPlugin, IBuildPlugin
    {
        private Connection GetConnection(ConfigurationRawData configuration)
        {
            var port = configuration?.Port ?? 0;

            if (Connections.TryGetValue(port, out var connection))
                return connection;

            var socket = new NamedPipeClientStream(".", $"BuildNotifications.DummyBuildServer.{port}", PipeDirection.InOut);

            connection = new Connection(socket);
            Connections.Add(port, connection);
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
                using var connection = GetConnection(data);
                await connection.Connect();
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

        Task<ConnectionTestResult> IBuildPlugin.TestConnection(string data)
        {
            return TestConnection(ParseConfig(data));
        }

        public IPluginConfiguration ConstructNewConfiguration() => new Configuration();

        public string DisplayName => "Dummy Build Server";

        public string IconSvgPath => "F1 M64,64z M0,0z M32.05,32L0,32Q0,45.3 9.35,54.6 18.75,64 32,64L32.05,64 32.05,32 M64,32Q64,18.75 54.6,9.35 45.3173828125,0.017578125 32.05,0L32.05,32 64,32z";

        public void OnPluginLoaded(IPluginHost host)
        {
        }

        Task<ConnectionTestResult> ISourceControlPlugin.TestConnection(string data)
        {
            return TestConnection(ParseConfig(data));
        }

        public IBranchProvider ConstructProvider(string data)
        {
            var connection = GetConnection(ParseConfig(data));
            return new SourceControlProvider(connection);
        }

        private static readonly Dictionary<int, Connection> Connections = new Dictionary<int, Connection>();
    }
}