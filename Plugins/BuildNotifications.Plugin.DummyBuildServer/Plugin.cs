using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
            {
                return null;
            }

            if (!int.TryParse(portString, out var port))
            {
                return null;
            }

            if (_connections.TryGetValue(port, out var connection))
            {
                return connection;
            }

            var client = new TcpClient();
            client.Connect(new IPEndPoint(IPAddress.Loopback, port));
            connection = new Connection(client);
            _connections.Add(port, connection);
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

        private readonly Dictionary<int, Connection> _connections = new Dictionary<int, Connection>();
    }
}