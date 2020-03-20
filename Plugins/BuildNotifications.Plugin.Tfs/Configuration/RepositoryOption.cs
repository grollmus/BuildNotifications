using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.Plugin.Tfs.SourceControl;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace BuildNotifications.Plugin.Tfs.Configuration
{
    internal class RepositoryOption : ListOption<TfsRepository?>
    {
        public RepositoryOption()
            : base(null, TextIds.RepositoryName, TextIds.RepositoryDescription)
        {
        }

        public override IEnumerable<ListOptionItem<TfsRepository?>> AvailableValues
        {
            get { return _availableRepositories.Select(r => new ListOptionItem<TfsRepository?>(r, r.RepositoryName ?? string.Empty)); }
        }

        public async Task<IEnumerable<TfsRepository>> FetchAvailableRepositories(TfsConfigurationRawData rawData)
        {
            IsLoading = true;
            try
            {
                if (string.IsNullOrEmpty(rawData.Url) || string.IsNullOrEmpty(rawData.CollectionName))
                    return Enumerable.Empty<TfsRepository>();
                if (rawData.AuthenticationType == AuthenticationType.Token && string.IsNullOrEmpty(rawData.Token?.PlainText()))
                    return Enumerable.Empty<TfsRepository>();
                if (rawData.AuthenticationType == AuthenticationType.Account && (string.IsNullOrEmpty(rawData.Username) || string.IsNullOrEmpty(rawData.Password?.PlainText())))
                    return Enumerable.Empty<TfsRepository>();

                var pool = new TfsConnectionPool();
                var vssConnection = pool.CreateConnection(rawData);
                if (vssConnection == null)
                {
                    _availableRepositories.Clear();
                    return Enumerable.Empty<TfsRepository>();
                }

                if (rawData.Project == null)
                {
                    _availableRepositories.Clear();
                    return Enumerable.Empty<TfsRepository>();
                }

                var gitClient = vssConnection.GetClient<GitHttpClient>();
                var repositories = await gitClient.GetRepositoriesAsync(rawData.Project.Id);

                return repositories.Select(r => new TfsRepository(r)).ToList();
            }
            catch (Exception e)
            {
                LogTo.InfoException("Failed to fetch repositories", e);
                return Enumerable.Empty<TfsRepository>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected override bool ValidateValue(TfsRepository? value) => value != null && !string.IsNullOrEmpty(value.Id);

        private List<TfsRepository> _availableRepositories = new List<TfsRepository>();

        public void SetAvailableRepositories(IEnumerable<TfsRepository> fetchedRepositories)
        {
            _availableRepositories = fetchedRepositories.ToList();
            RaiseAvailableValuesChanged();
        }
    }
}