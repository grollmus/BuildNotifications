using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using NLog.Fluent;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsConnectionPool
    {
        internal VssConnection? CreateConnection(TfsConfiguration data)
        {
            var url = data.Url;
            if (string.IsNullOrWhiteSpace(url))
            {
                Log.Error().Message(ErrorMessages.UrlWasEmpty).Write();
                return null;
            }

            if (NeedsToAppendCollectionName(data) && !string.IsNullOrWhiteSpace(data.CollectionName))
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
            if (string.IsNullOrWhiteSpace(data.Url))
                return ConnectionTestResult.Failure(ErrorMessages.UrlWasEmpty);

            try
            {
                var credentials = CreateCredentials(data);
                using var connection = new VssConnection(new Uri(data.Url), credentials);

                await connection.ConnectAsync();

                if (!connection.HasAuthenticated
                    || !IsAuthenticatedId(connection.AuthenticatedIdentity.Id))
                    return ConnectionTestResult.Failure(ErrorMessages.AuthenticationFailed);

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
            if (data.AuthenticationType == AuthenticationType.Token)
                return new VssBasicCredential("", data.Token?.PlainText());

            var username = data.Username;
            var pw = data.Password;
            return new VssCredentials(new VssBasicCredential(username, pw?.PlainText()));
        }

        private bool IsAuthenticatedId(Guid authenticatedIdentityId) => !authenticatedIdentityId.Equals(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

        private bool IsOnPremiseServer(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            var uri = new Uri(url);
            return uri.Host != "dev.azure.com";
        }

        private bool NeedsToAppendCollectionName(TfsConfiguration data) => IsOnPremiseServer(data.Url);

        private readonly Dictionary<string, VssConnection> _connections = new Dictionary<string, VssConnection>(StringComparer.OrdinalIgnoreCase);
    }
}