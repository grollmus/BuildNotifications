using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.PluginInterfaces;
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
            {
                LogTo.Error("Given URL was empty.");
                return null;
            }

            url = AppendCollectionName(data.CollectionName, url);

            if (_connections.TryGetValue(url, out var cachedConnection))
                return cachedConnection;

            var credentials = CreateCredentials(data);

            var connection = new VssConnection(new Uri(url), credentials);

            _connections.Add(url, connection);

            return connection;
        }

        internal async Task<ConnectionTestResult> TestConnection(TfsConfiguration data)
        {
            try
            {
                var credentials = CreateCredentials(data);
                using var connection = new VssConnection(new Uri(data.Url), credentials);

                await connection.ConnectAsync();

                return ConnectionTestResult.Success;
            }
            catch (Exception ex)
            {
                return ConnectionTestResult.Failure(ex.Message);
            }
        }

        private static string AppendCollectionName(string collectionName, string url)
        {
            if (!url.EndsWith(collectionName, StringComparison.OrdinalIgnoreCase))
            {
                if (url.EndsWith('/'))
                    url += collectionName;
                else
                    url += "/" + collectionName;
            }

            return url;
        }

        private VssCredentials CreateCredentials(TfsConfiguration data)
        {
            if (data.AuthenticationType == AuthenticationType.Windows)
                return new VssCredentials();

            var username = data.Username;
            var pw = data.Password;
            return new VssCredentials(new VssBasicCredential(username, pw));
        }

        private readonly Dictionary<string, VssConnection> _connections = new Dictionary<string, VssConnection>(StringComparer.OrdinalIgnoreCase);
    }
}