using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.PluginInterfaces;
using JetBrains.Annotations;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using ReflectSettings.Attributes;

namespace BuildNotifications.Plugin.Tfs
{
    [NoReorder]
    public class TfsConfiguration
    {
        public string? Url { get; set; }

        public string? CollectionName { get; set; }

        [CalculatedValuesAsync(nameof(FetchProjects))]
        public TfsProject? Project { get; set; }

        [CalculatedValuesAsync(nameof(FetchRepositories))]
        public TfsRepository? Repository { get; set; }

        public AuthenticationType AuthenticationType { get; set; }

        [CalculatedVisibility(nameof(UsernameHidden))]
        public string? Username { get; set; }

        [CalculatedVisibility(nameof(PasswordHidden))]
        public PasswordString? Password { get; set; }

        [CalculatedVisibility(nameof(TokenHidden))]
        public PasswordString? Token { get; set; }

        public bool UsernameHidden()
        {
            return AuthenticationType != AuthenticationType.Account;
        }

        public bool PasswordHidden()
        {
            return AuthenticationType != AuthenticationType.Account;
        }

        public bool TokenHidden()
        {
            return AuthenticationType != AuthenticationType.Token;
        }

        private IEnumerable<object> _lastProjectFetchResult = Enumerable.Empty<object>();
        private IEnumerable<object> _lastRepoFetchResult = Enumerable.Empty<object>();

        private string? _lastFetchedRepoKey;

        private string? _lastFetchedProjectKey;

        private string CurrentConfigAsKey(bool withProject)
        {
            return $"{Username}{Password?.GetHashCode()}{Token?.GetHashCode()}{AuthenticationType}{Url}{CollectionName}{(withProject ? Project?.Id + _lastProjectFetchResult.Count() : string.Empty)}";
        }

        private bool IsCurrentConfigTheSameAsKey(string? key, bool withProject) => CurrentConfigAsKey(withProject).Equals(key, StringComparison.OrdinalIgnoreCase);

        public async Task<IEnumerable<object>> FetchRepositories()
        {
            if (IsCurrentConfigTheSameAsKey(_lastFetchedRepoKey, true))
            {
                if (_lastRepoFetchResult != null)
                    return _lastRepoFetchResult;

                return DefaultRepositoryReturnValue();
            }

            var result = await FetchRepositoriesInternal();
            _lastRepoFetchResult = result.ToList();

            return _lastRepoFetchResult;
        }

        public async Task<IEnumerable<object>> FetchProjects()
        {
            if (IsCurrentConfigTheSameAsKey(_lastFetchedProjectKey, false))
            {
                if (_lastProjectFetchResult != null)
                    return _lastProjectFetchResult;

                return DefaultProjectReturnValue();
            }

            var result = await FetchProjectsInternal();
            _lastProjectFetchResult = result.ToList();

            return _lastProjectFetchResult;
        }

        private IEnumerable<object> DefaultProjectReturnValue()
        {
            if (Project != null && !string.IsNullOrEmpty(Project.Id))
                return new List<object> {Project};

            return Enumerable.Empty<object>();
        }

        private IEnumerable<object> DefaultRepositoryReturnValue()
        {
            if (Repository != null && !string.IsNullOrEmpty(Repository.Id))
                return new List<object> {Repository};

            return Enumerable.Empty<object>();
        }

        private async Task<IEnumerable<object>> FetchProjectsInternal()
        {
            try
            {
                var pool = new TfsConnectionPool();
                var vssConnection = pool.CreateConnection(this);
                if (vssConnection == null)
                    return DefaultProjectReturnValue();
                var projectClient = vssConnection.GetClient<ProjectHttpClient>();
                var projects = await projectClient.GetProjects(ProjectState.WellFormed);

                return projects.Select(p => new TfsProject(p));
            }
            catch (Exception e)
            {
                LogTo.InfoException("Failed to fetch projectIds", e);
                return DefaultProjectReturnValue();
            }
            finally
            {
                _lastFetchedProjectKey = CurrentConfigAsKey(false);
            }
        }

        private async Task<IEnumerable<object>> FetchRepositoriesInternal()
        {
            try
            {
                var pool = new TfsConnectionPool();
                var vssConnection = pool.CreateConnection(this);
                if (vssConnection == null)
                    return DefaultRepositoryReturnValue();

                var gitClient = vssConnection.GetClient<GitHttpClient>();
                var repositories = await gitClient.GetRepositoriesAsync(Project!.Id);

                return repositories.Select(r => new TfsRepository(r));
            }
            catch (Exception e)
            {
                LogTo.InfoException("Failed to fetch projectIds", e);
                return DefaultRepositoryReturnValue();
            }
            finally
            {
                _lastFetchedRepoKey = CurrentConfigAsKey(true);
            }
        }
    }
}