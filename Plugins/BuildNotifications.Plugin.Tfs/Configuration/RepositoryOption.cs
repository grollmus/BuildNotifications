using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
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

        public async Task FetchAvailableRepositories(TfsConfigurationRawData rawData)
        {
            try
            {
                var pool = new TfsConnectionPool();
                var vssConnection = pool.CreateConnection(rawData);
                if (vssConnection == null)
                {
                    _availableRepositories.Clear();
                    return;
                }

                if (rawData.Project == null)
                {
                    _availableRepositories.Clear();
                    return;
                }

                var gitClient = vssConnection.GetClient<GitHttpClient>();
                var repositories = await gitClient.GetRepositoriesAsync(rawData.Project.Id);

                _availableRepositories = repositories.Select(r => new TfsRepository(r)).ToList();
            }
            catch (Exception e)
            {
                LogTo.InfoException("Failed to fetch repositories", e);
            }
            finally
            {
                RaiseAvailableValuesChanged();
            }
        }

        private List<TfsRepository> _availableRepositories = new List<TfsRepository>();
    }
}