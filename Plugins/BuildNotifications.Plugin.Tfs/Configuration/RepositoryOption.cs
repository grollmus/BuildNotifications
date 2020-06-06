using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Plugin.Tfs.SourceControl;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using NLog.Fluent;

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
                if (rawData.Project == null)
                    return Enumerable.Empty<TfsRepository>();

                var pool = new TfsConnectionPool();
                var vssConnection = pool.CreateConnection(rawData);
                if (vssConnection == null)
                    return Enumerable.Empty<TfsRepository>();

                var gitClient = vssConnection.GetClient<GitHttpClient>();
                var repositories = await gitClient.GetRepositoriesAsync(rawData.Project.Id);

                return repositories.Select(r => new TfsRepository(r)).ToList();
            }
            catch (Exception e)
            {
                Log.Info().Message("Failed to fetch repositories").Exception(e).Write();
                return Enumerable.Empty<TfsRepository>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void SetAvailableRepositories(IEnumerable<TfsRepository> fetchedRepositories)
        {
            _availableRepositories = fetchedRepositories.ToList();
            RaiseAvailableValuesChanged();
            Value = _availableRepositories.FirstOrDefault(r => Equals(r, Value));
        }

        protected override bool ValidateValue(TfsRepository? value) => value != null && !string.IsNullOrEmpty(value.Id);

        private List<TfsRepository> _availableRepositories = new List<TfsRepository>();
    }
}