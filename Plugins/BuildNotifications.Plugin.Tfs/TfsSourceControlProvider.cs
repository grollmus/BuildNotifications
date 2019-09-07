using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.SourceControl;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsSourceControlProvider : IBranchProvider
    {
        public TfsSourceControlProvider(VssConnection connection, string repositoryId)
        {
            _connection = connection;
            _repositoryId = Guid.Parse(repositoryId);
        }

        private IBranch Convert(GitRef branch)
        {
            return new TfsBranch(branch);
        }

        public async IAsyncEnumerable<IBranch> FetchExistingBranches()
        {
            var gitClient = await _connection.GetClientAsync<GitHttpClient>();
            var branches = await gitClient.GetBranchRefsAsync(_repositoryId);

            foreach (var branch in branches)
            {
                yield return Convert(branch);
            }
        }

        public async IAsyncEnumerable<IBranch> RemovedBranches()
        {
            await Task.CompletedTask;

            yield break;
        }

        private readonly VssConnection _connection;
        private readonly Guid _repositoryId;
    }
}