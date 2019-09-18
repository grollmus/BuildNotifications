using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.PluginInterfaces;
using JetBrains.Annotations;
using ReflectSettings.Attributes;

namespace BuildNotifications.Plugin.Tfs
{
    [NoReorder]
    public class TfsConfiguration
    {
        public string? Url { get; set; }

        public string? CollectionName { get; set; }

        [CalculatedValuesAsync(nameof(FetchProjectIds))]
        public string? ProjectId { get; set; }

        public string? RepositoryId { get; set; }

        public AuthenticationType AuthenticationType { get; set; }

        [CalculatedVisibility(nameof(UsernameHidden))]
        public string? Username { get; set; }

        [CalculatedVisibility(nameof(PasswordHidden))]
        public PasswordString? Password { get; set; }

        [CalculatedVisibility(nameof(TokenHidden))]
        public PasswordString? Token { get; set; }

        public bool UsernameHidden() => AuthenticationType != AuthenticationType.Account;

        public bool PasswordHidden() => AuthenticationType != AuthenticationType.Account;

        public bool TokenHidden() => AuthenticationType != AuthenticationType.Token;

        private string? _lastFetchedUrl;
        private IEnumerable<object> _lastFetchResult = Enumerable.Empty<object>();

        public async Task<IEnumerable<object>> FetchProjectIds()
        {
            if (_lastFetchedUrl == Url || Url == null)
            {
                if (_lastFetchResult != null)
                    return _lastFetchResult;

                if (ProjectId != null)
                    return new List<string> {ProjectId};
                else
                    return Enumerable.Empty<string>();
            }

            var urlToFetch = Url;

            var result = await FetchProjectIdsInternal(urlToFetch);
            _lastFetchResult = result.ToList();

            return _lastFetchResult;
        }

        private async Task<IEnumerable<object>> FetchProjectIdsInternal(string urlToFetch)
        {
            try
            {
                // do some fetch logic
                await Task.Delay(1000);
                if (Url == "SomeWorkingUrl")
                    return new List<string> {"ID1", "ID2", "ID3"};
                else
                    throw new Exception("Hmm");
                //return Enumerable.Empty<string>();
            }
            catch (Exception e)
            {
                LogTo.InfoException("Failed to fetch projectIds", e);
                if (ProjectId != null)
                    return new List<object> {ProjectId};
                else
                    return Enumerable.Empty<object>();
            }
            finally
            {
                _lastFetchedUrl = urlToFetch;
            }
        }
    }

    public enum AuthenticationType
    {
        Windows,
        Account,
        Token
    }
}