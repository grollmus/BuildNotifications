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
        public TfsSourceControlProvider(VssConnection connection, Guid repositoryId, Guid projectId)
        {
            _connection = connection;
            _projectId = projectId;
            _repositoryId = repositoryId;
        }

        private TfsBranch Convert(GitRef branch)
        {
            return new TfsBranch(branch);
        }

        private TfsBranch Convert(GitPullRequest branch)
        {
            return new TfsBranch(branch.PullRequestId);
        }

        private async Task<List<GitPullRequest>> FetchPullRequests(GitHttpClient gitClient)
        {
            var searchCriteria = new GitPullRequestSearchCriteria
            {
                RepositoryId = _repositoryId,
                Status = PullRequestStatus.Active
            };
            var prs = await gitClient.GetPullRequestsByProjectAsync(_projectId, searchCriteria);
            return prs;
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

            var pullRequests = await FetchPullRequests(gitClient);

            foreach (var pullRequest in pullRequests)
            {
                var converted = Convert(pullRequest);
                _knownBranches.Add(converted);
                yield return converted;
            }
        }

        public async IAsyncEnumerable<IBranch> RemovedBranches()
        {
            var gitClient = await _connection.GetClientAsync<GitHttpClient>();
            var branches = await gitClient.GetBranchRefsAsync(_repositoryId);
            var pullRequests = await FetchPullRequests(gitClient);

            var existing = branches.Select(Convert).Concat(pullRequests.Select(Convert));

            var deletedBranches = _knownBranches.Except(existing, new TfsBranchComparer());

            foreach (var branch in deletedBranches)
            {
                yield return branch;
            }
        }

        private readonly HashSet<TfsBranch> _knownBranches = new HashSet<TfsBranch>(new TfsBranchComparer());

        private readonly VssConnection _connection;
        private readonly Guid _projectId;
        private readonly Guid _repositoryId;
    }
}