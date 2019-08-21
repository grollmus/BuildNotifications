using System;
using System.Collections.Generic;
using System.IO.Pipes;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Host;
using BuildNotifications.PluginInterfaces.Options;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    public class Plugin : ISourceControlPlugin, IBuildPlugin
    {
        private Connection GetConnection(IReadOnlyDictionary<string, string> data)
        {
            if (!data.TryGetValue(Constants.Connection.Port, out var portString))
                return null;

            if (!int.TryParse(portString, out var port))
                return null;

            if (Connections.TryGetValue(port, out var connection))
                return connection;

            var socket = new NamedPipeClientStream(".", $"BuildNotifications.DummyBuildServer.{port}", PipeDirection.InOut);

            connection = new Connection(socket);
            Connections.Add(port, connection);
            return connection;
        }

        private static IOptionSchema OptionSchema(IPluginHost host)
        {
            var schema = host.SchemaFactory.Schema();
            var option = host.SchemaFactory.Number(Constants.Connection.Port, "Port", "Port the dummy server is running at", 1, short.MaxValue, 1111, true);
            schema.Add(option);

            return schema;
        }

        /// <inheritdoc />
        IOptionSchema IBuildPlugin.GetSchema(IPluginHost host)
        {
            return OptionSchema(host);
        }

        /// <inheritdoc />
        IReadOnlyDictionary<string, string> IBuildPlugin.Serialize(IBuildProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        IBuildProvider IBuildPlugin.ConstructProvider(IReadOnlyDictionary<string, string> data)
        {
            var connection = GetConnection(data);
            return new BuildProvider(connection);
        }

        /// <inheritdoc />
        IBranchProvider ISourceControlPlugin.ConstructProvider(IReadOnlyDictionary<string, string> data)
        {
            var connection = GetConnection(data);
            return new SourceControlProvider(connection);
        }

        /// <inheritdoc />
        IOptionSchema ISourceControlPlugin.GetSchema(IPluginHost host)
        {
            return OptionSchema(host);
        }

        /// <inheritdoc />
        IReadOnlyDictionary<string, string> ISourceControlPlugin.Serialize(IBuildProvider provider)
        {
            throw new NotImplementedException();
        }

        private static readonly Dictionary<int, Connection> Connections = new Dictionary<int, Connection>();

        public string DisplayName => "Dummy Build Server";

        public string IconSvgPath => "F1 M64,64z M0,0z M32.05,32L0,32Q0,45.3 9.35,54.6 18.75,64 32,64L32.05,64 32.05,32 M64,32Q64,18.75 54.6,9.35 45.3173828125,0.017578125 32.05,0L32.05,32 64,32z";
    }
}