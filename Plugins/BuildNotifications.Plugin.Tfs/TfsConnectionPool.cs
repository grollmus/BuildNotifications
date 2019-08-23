using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsConnectionPool
    {
        internal VssConnection? CreateConnection(TfsConfiguration data)
        {
            var url = data.Url;
            if (string.IsNullOrWhiteSpace(url))
                return null;

            if (_connections.TryGetValue(url, out var cachedConnection))
                return cachedConnection;

            var credentials = CreateCredentials(data);

            var connection = new VssConnection(new Uri(url), credentials);

            _connections.Add(url, connection);

            return connection;
        }

        private VssCredentials CreateCredentials(TfsConfiguration data)
        {
            if (data.AuthenticationType == AuthenticationType.Windows)
                return new VssCredentials();

            var username = data.Username;
            var pw = data.Password;
            return new VssCredentials(new VssBasicCredential(username, pw));
        }

        private readonly Dictionary<string, VssConnection> _connections = new Dictionary<string, VssConnection>();
    }
}