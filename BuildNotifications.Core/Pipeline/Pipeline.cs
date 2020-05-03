using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Cache;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class Pipeline : IPipeline
    {
        public Pipeline(ITreeBuilder treeBuilder, IConfiguration configuration)
        {
            _treeBuilder = treeBuilder;
            _configuration = configuration;
            _buildCache = new PipelineCache<IBuild>();
            _branchCache = new PipelineCache<IBranch>();
            _definitionCache = new PipelineCache<IBuildDefinition>();
            _notificationFactory = new NotificationFactory(configuration);
            _pipelineNotifier = new PipelineNotifier();

            _searchTerm = string.Empty;
        }

        private IBuildTree BuildTree()
        {
            LogTo.Debug("Creating BuildTree");
            var builds = _buildCache.ContentCopy().ToList();
            var branches = _branchCache.ContentCopy().ToList();
            var definitions = _definitionCache.ContentCopy().ToList();
            LogTo.Debug($"{builds.Count} cached builds, {branches.Count} cached branches, {definitions.Count} cached definitions");

            var tree = _treeBuilder.Build(builds, branches, definitions, _oldTree, _searchTerm);
            LogTo.Debug("Created tree.");
            if (_configuration.GroupDefinition.Any())
            {
                LogTo.Debug("Cutting tree.");
                CutTree(tree);
            }

            return tree;
        }

        private void CleanupBuilds()
        {
            LogTo.Debug("Cleaning up builds");
            var builds = _buildCache.ContentCopy();
            var count = 0;
            foreach (var build in builds.Cast<EnrichedBuild>())
            {
                var definitionExists = _definitionCache.ContainsValue(build.Definition);
                var branchExists = _branchCache.Contains(b => b.Equals(build.Branch));
                if (!definitionExists || !branchExists)
                {
                    _buildCache.RemoveValue(build);
                    count += 1;
                }
            }

            LogTo.Debug($"Cleaned {count} builds");
        }

        private void CutTree(IBuildTreeNode tree)
        {
            if (tree == null)
                return;

            var buildChildrenToRemove = tree.Children.OfType<IBuildNode>()
                .OrderByDescending(x => x.Build.LastChangedTime ?? DateTime.MinValue)
                .Skip(_configuration.BuildsToShow)
                .ToList();

            foreach (var node in buildChildrenToRemove)
            {
                tree.RemoveChild(node);
            }

            foreach (var child in tree.Children)
            {
                CutTree(child);
            }
        }

        private async Task FetchBranches()
        {
            LogTo.Debug("Fetching branches");
            foreach (var project in _projectList)
            {
                LogTo.Debug($"Fetching branches for project \"{project.Name}\"");
                try
                {
                    var branches = project.FetchExistingBranches();
                    var count = 0;
                    await foreach (var branch in branches)
                    {
                        _branchCache.AddOrReplace(branch.CacheKey(project), branch);
                        count += 1;
                    }

                    LogTo.Debug($"Added \"{count}\" branches in project \"{project.Name}\"");

                    var removedBranches = project.FetchRemovedBranches();

                    count = 0;
                    await foreach (var branch in removedBranches)
                    {
                        _branchCache.Remove(branch.CacheKey(project));
                        count += 1;
                    }

                    LogTo.Debug($"Removed \"{count}\" branches in project \"{project.Name}\"");
                }
                catch (Exception ex)
                {
                    ReportError("ErrorFetchingBranches", project.Name, ex);
                }
            }

            LogTo.Debug("Done fetching branches");
        }

        private async Task FetchBuilds()
        {
            LogTo.Debug("Fetching builds");
            foreach (var project in _projectList)
            {
                try
                {
                    LogTo.Debug($"Fetching builds for project \"{project.Name}\". ID: \"{project.Guid}\"");

                    if (_lastUpdate.HasValue)
                        LogTo.Debug($"Fetching all builds since {_lastUpdate.Value} for project \"{project.Name}\"");
                    else
                        LogTo.Debug($"Fetching all builds for project \"{project.Name}\"");

                    var builds = _lastUpdate.HasValue
                        ? project.FetchBuildsChangedSince(_lastUpdate.Value)
                        : project.FetchAllBuilds();

                    var count = 0;
                    await foreach (var build in builds)
                    {
                        _buildCache.AddOrReplace(build.CacheKey(), build);
                        count += 1;
                    }

                    LogTo.Debug($"Added \"{count}\" builds in project \"{project.Name}\"");
                    var removedBuilds = project.FetchRemovedBuilds();
                    count = 0;
                    await foreach (var build in removedBuilds)
                    {
                        _buildCache.Remove(build.CacheKey());
                        count += 1;
                    }

                    LogTo.Debug($"Removed \"{count}\" builds in project \"{project.Name}\"");
                }
                catch (Exception ex)
                {
                    var projectName = project.Name;
                    ReportError("ErrorFetchingBuilds", projectName, ex);
                }
            }

            LogTo.Debug("Done fetching builds");
            _lastUpdate = DateTime.UtcNow;
        }

        private async Task FetchDefinitions()
        {
            LogTo.Debug("Fetching definitions");
            foreach (var project in _projectList)
            {
                LogTo.Debug($"Fetching definitions for project \"{project.Name}\"");
                try
                {
                    var definitions = project.FetchBuildDefinitions();
                    var count = 0;
                    await foreach (var definition in definitions)
                    {
                        _definitionCache.AddOrReplace(definition.CacheKey(project), definition);
                        count += 1;
                    }

                    LogTo.Debug($"Added \"{count}\" definitions in project \"{project.Name}\"");

                    var removedDefinitions = project.FetchRemovedBuildDefinitions();
                    count = 0;
                    await foreach (var definition in removedDefinitions)
                    {
                        _definitionCache.Remove(definition.CacheKey(project));
                        count += 1;
                    }

                    LogTo.Debug($"Added \"{count}\" definitions in project \"{project.Name}\"");
                }
                catch (Exception ex)
                {
                    ReportError("ErrorFetchingDefinitions", project.Name, ex);
                }
            }

            LogTo.Debug("Done fetching definitions");
        }

        private void ReportError(string messageTextId, params object[] parameter)
        {
            var localizedMessage = StringLocalizer.Instance.GetText(messageTextId);
            var fullMessage = string.Format(StringLocalizer.CurrentCulture, localizedMessage, parameter);
            if (parameter.FirstOrDefault(x => x is Exception) is Exception exception)
                LogTo.ErrorException(fullMessage, exception);
            else
                LogTo.Error(fullMessage);
        }

        private async Task UpdateBuilds()
        {
            LogTo.Debug("Updating builds.");
            foreach (var project in _projectList)
            {
                LogTo.Debug($"Updating builds of project \"{project.Name}\".");
                var projectId = project.Guid.ToString();
                var buildsForProject = _buildCache.Values(projectId).ToList();
                var branchesForProject = _branchCache.Values(projectId);

                await Task.WhenAll(
                    project.UpdateBuilds(buildsForProject),
                    project.UpdateBuildBranches(buildsForProject, branchesForProject)
                );
            }
        }

        public void AddProject(IProject project)
        {
            LogTo.Info($"Adding project \"{project.Name}\"");
            _projectList.Add(project);
            try
            {
                var currentUserIdentities = project.FetchCurrentUserIdentities();
                foreach (var currentUserIdentity in currentUserIdentities.Where(x => x != null))
                {
                    LogTo.Debug($"Adding identity \"{currentUserIdentity.UniqueName}\" from project \"{project.Name}\"");
                    _configuration.IdentitiesOfCurrentUser.Add(currentUserIdentity);
                }
            }
            catch (Exception e)
            {
                ReportError("ErrorFetchingUserIdentities", project.Name, e);
            }
        }

        public void ClearProjects()
        {
            LogTo.Info("Clearing projects and all cached data.");
            _projectList.Clear();
            _definitionCache.Clear();
            _buildCache.Clear();
            _branchCache.Clear();
            _lastUpdate = null;
            _oldTree = null;
            _configuration.IdentitiesOfCurrentUser.Clear();
        }

        public void Search(string searchTerm)
        {
            LogTo.Info($"Applying search \"{searchTerm}\".");
            _searchTerm = searchTerm;

            var tree = BuildTree();
            _pipelineNotifier.Notify(tree, Enumerable.Empty<INotification>());
            LogTo.Debug($"Applied search \"{searchTerm}\".");
        }

        public async Task Update()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            LogTo.Info("Starting update.");
            var treeResult = await Task.Run(async () =>
            {
                var previousBuildStatus = _buildCache.CachedValues().ToDictionary(p => p.Key, p => p.Value.Status);
                var branchTask = FetchBranches();
                var definitionsTask = FetchDefinitions();
                var buildsTask = FetchBuilds();

                await Task.WhenAll(branchTask, definitionsTask, buildsTask);

                LogTo.Debug("Everything is fetched.");

                await UpdateBuilds();

                CleanupBuilds();

                var tree = BuildTree();

                var currentBuildNodes = tree.AllChildren().OfType<IBuildNode>();
                IBuildTreeBuildsDelta delta;

                LogTo.Debug("BuildTree is done. Producing notifications.");
                // don't show any notifications for the initial fetch
                if (_oldTree == null)
                    delta = new BuildTreeBuildsDelta();
                else
                    delta = new BuildTreeBuildsDelta(currentBuildNodes, previousBuildStatus, _configuration.PartialSucceededTreatmentMode);

                var notifications = _notificationFactory.ProduceNotifications(delta).ToList();
                return (BuildTree: tree, Notifications: notifications);
            });

            LogTo.Debug("Calling notify.");
            _pipelineNotifier.Notify(treeResult.BuildTree, treeResult.Notifications);

            _oldTree = treeResult.BuildTree;
            stopWatch.Stop();
            LogTo.Info($"Update done in {stopWatch.ElapsedMilliseconds} ms.");
        }

        public IPipelineNotifier Notifier => _pipelineNotifier;

        private readonly ITreeBuilder _treeBuilder;
        private readonly IConfiguration _configuration;
        private readonly IPipelineCache<IBuild> _buildCache;
        private readonly IPipelineCache<IBranch> _branchCache;
        private readonly IPipelineCache<IBuildDefinition> _definitionCache;
        private readonly PipelineNotifier _pipelineNotifier;
        private readonly ConcurrentBag<IProject> _projectList = new ConcurrentBag<IProject>();
        private readonly NotificationFactory _notificationFactory;
        private DateTime? _lastUpdate;
        private IBuildTree? _oldTree;
        private string _searchTerm;
    }
}