using System;
using System.Collections.Generic;
using System.Linq;
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

        private TfsBranch Convert(GitRef branch)
        {
            return new TfsBranch(branch);
        }

        public async IAsyncEnumerable<IBranch> FetchExistingBranches()
        {
            var gitClient = await _connection.GetClientAsync<GitHttpClient>();
            var branches = await gitClient.GetBranchRefsAsync(_repositoryId);

            foreach (var branch in branches)
            {
                var converted = Convert(branch);
                _knownBranches.Add(converted);
                yield return converted;
            }
        }

        public async IAsyncEnumerable<IBranch> RemovedBranches()
        {
            var gitClient = await _connection.GetClientAsync<GitHttpClient>();
            var branches = await gitClient.GetBranchRefsAsync(_repositoryId);

            var deletedBranches = _knownBranches.Except(branches.Select(Convert), new TfsBranchComparer());

            foreach (var branch in deletedBranches)
            {
                yield return branch;
            }
        }

        public Task<bool> IsRealBranch(string branchName)
        {
            var prefixedName = TfsBranch.BranchNamePrefix + branchName;
            var isReal = _knownBranches.Any(b => b.Name == branchName)
                         || _knownBranches.Any(b => b.Name == prefixedName);

            return Task.FromResult(isReal);
        }

        private readonly HashSet<TfsBranch> _knownBranches = new HashSet<TfsBranch>(new TfsBranchComparer());

        private readonly VssConnection _connection;
        private readonly Guid _repositoryId;
    }
}