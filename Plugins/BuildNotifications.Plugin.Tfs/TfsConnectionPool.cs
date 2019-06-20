using System;
using System.Collections.Generic;
using Anotar.NLog;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsConnectionPool
    {
        internal VssConnection? CreateConnection(IReadOnlyDictionary<string, string?> data)
        {
            if (!data.TryGetValue(TfsConstants.Connection.Url, out var url) || string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            if (_connections.TryGetValue(url, out var cachedConnection))
            {
                return cachedConnection;
            }

            var credentials = CreateCredentials(data);

            var connection = new VssConnection(new Uri(url), credentials);

            _connections.Add(url, connection);

            return connection;
        }

        private VssCredentials CreateCredentials(IReadOnlyDictionary<string, string?> data)
        {
            if (!data.TryGetValue(TfsConstants.Connection.AuthenticationType, out var authType))
            {
                authType = TfsConstants.Connection.WindowsAuthentication;
            }

            if (authType == TfsConstants.Connection.AccountAuthentication)
            {
                if (data.TryGetValue(TfsConstants.Connection.UserName, out var userName) &&
                    data.TryGetValue(TfsConstants.Connection.Password, out var password))
                {
                    return new VssCredentials(new VssBasicCredential(userName, password));
                }

                LogTo.Warn("AuthenticationType was set to Account but credentials are missing in configuration");
            }

            return new VssCredentials();
        }

        private readonly Dictionary<string, VssConnection> _connections = new Dictionary<string, VssConnection>();
    }
}