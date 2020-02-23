using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.Plugin.Tfs.Configuration;
using BuildNotifications.PluginInterfaces;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsConnectionPool
    {
        internal VssConnection? CreateConnection(TfsConfigurationRawData data, bool useCache = true)
        {
            var url = data.Url;
            if (string.IsNullOrWhiteSpace(url))
            {
                LogTo.Error(ErrorMessages.UrlWasEmpty);
                return null;
            }

            if (!string.IsNullOrWhiteSpace(data.CollectionName))
                url = AppendCollectionName(data.CollectionName, url);

            if (useCache && _connections.TryGetValue(url, out var cachedConnection))
                return cachedConnection;

            var credentials = CreateCredentials(data);

            var connection = new VssConnection(new Uri(url), credentials);

            if (useCache)
                _connections.Add(url, connection);

            return connection;
        }

        internal async Task<ConnectionTestResult> TestConnection(TfsConfigurationRawData data)
        {
            if (string.IsNullOrWhiteSpace(data.Url))
                return ConnectionTestResult.Failure(ErrorMessages.UrlWasEmpty);

            try
            {
                using var connection = CreateConnection(data, false);
                if (connection == null)
                    return ConnectionTestResult.Failure(ErrorMessages.MissingData);

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

        private VssCredentials CreateCredentials(TfsConfigurationRawData data)
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

        private bool NeedsToAppendCollectionName(TfsConfigurationRawData data) => IsOnPremiseServer(data.Url);

        private readonly Dictionary<string, VssConnection> _connections = new Dictionary<string, VssConnection>(StringComparer.OrdinalIgnoreCase);
    }
}