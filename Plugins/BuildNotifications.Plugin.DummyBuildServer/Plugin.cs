using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    public class Plugin : ISourceControlPlugin, IBuildPlugin
    {
        private Connection GetConnection(Configuration configuration)
        {
            var port = configuration?.Port ?? 0;

            if (Connections.TryGetValue(port, out var connection))
                return connection;

            var socket = new NamedPipeClientStream(".", $"BuildNotifications.DummyBuildServer.{port}", PipeDirection.InOut);

            connection = new Connection(socket);
            Connections.Add(port, connection);
            return connection;
        }

        private async Task<ConnectionTestResult> TestConnection(object data)
        {
            try
            {
                using var connection = GetConnection(data as Configuration);
                await connection.Connect();
            }
            catch (Exception ex)
            {
                return ConnectionTestResult.Failure(ex.Message);
            }

            return ConnectionTestResult.Success;
        }

        IBuildProvider IBuildPlugin.ConstructProvider(object data)
        {
            var connection = GetConnection(data as Configuration);
            return new BuildProvider(connection);
        }

        Task<ConnectionTestResult> IBuildPlugin.TestConnection(object data)
        {
            return TestConnection(data);
        }

        public string DisplayName => "Dummy Build Server";

        public string IconSvgPath => "F1 M64,64z M0,0z M32.05,32L0,32Q0,45.3 9.35,54.6 18.75,64 32,64L32.05,64 32.05,32 M64,32Q64,18.75 54.6,9.35 45.3173828125,0.017578125 32.05,0L32.05,32 64,32z";

        public Type GetConfigurationType()
        {
            return typeof(Configuration);
        }

        public void SetCurrentConfiguration(object instance)
        {
            throw new NotImplementedException();
        }

        public void ConfigurationChanged()
        {
            throw new NotImplementedException();
        }

        Task<ConnectionTestResult> ISourceControlPlugin.TestConnection(object data)
        {
            return TestConnection(data);
        }

        public IBranchProvider ConstructProvider(object data)
        {
            var connection = GetConnection(data as Configuration);
            return new SourceControlProvider(connection);
        }

        private static readonly Dictionary<int, Connection> Connections = new Dictionary<int, Connection>();
    }
}