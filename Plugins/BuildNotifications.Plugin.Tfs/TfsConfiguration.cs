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

        private string? _lastFetchedUrl;
        private IEnumerable<object> _lastProjectFetchResult = Enumerable.Empty<object>();
        private IEnumerable<object> _lastFetchResult = Enumerable.Empty<object>();
        private readonly string _lastFetchProject = string.Empty;

        public async Task<IEnumerable<object>> FetchRepositories()
        {
            if (_lastFetchedUrl == Url || Url == null ||
                Project == null || _lastFetchProject == Project?.Id)
            {
                if (_lastFetchResult != null)
                    return _lastFetchResult;

                return DefaultRepositoryReturnValue();
            }

            var urlToFetch = Url;

            var result = await FetchRepositoriesInternal(urlToFetch);
            _lastFetchResult = result.ToList();

            return _lastFetchResult;
        }

        public async Task<IEnumerable<object>> FetchProjects()
        {
            if (_lastFetchedUrl == Url || Url == null)
            {
                if (_lastProjectFetchResult != null)
                    return _lastProjectFetchResult;

                return DefaultProjectReturnValue();
            }

            var urlToFetch = Url;

            var result = await FetchProjectsInternal(urlToFetch);
            _lastProjectFetchResult = result.ToList();

            return _lastProjectFetchResult;
        }

        private IEnumerable<object> DefaultProjectReturnValue()
        {
            if (Project != null)
                return new List<object> {Project};

            return Enumerable.Empty<object>();
        }

        private IEnumerable<object> DefaultRepositoryReturnValue()
        {
            if (Repository != null)
                return new List<object> {Repository};

            return Enumerable.Empty<object>();
        }

        private async Task<IEnumerable<object>> FetchProjectsInternal(string urlToFetch)
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
                _lastFetchedUrl = urlToFetch;
            }
        }

        private async Task<IEnumerable<object>> FetchRepositoriesInternal(string urlToFetch)
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
                _lastFetchedUrl = urlToFetch;
            }
        }
    }
}