using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Cache;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Tree;
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
        }

        private void CleanupBuilds()
        {
            var builds = _buildCache.ContentCopy();

            foreach (var build in builds)
            {
                if (!_definitionCache.ContainsValue(build.Definition))
                    _buildCache.RemoveValue(build);
            }
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
            foreach (var project in _projectList)
            {
                try
                {
                    var projectId = project.GetHashCode();

                    var branches = project.FetchExistingBranches();
                    await foreach (var branch in branches)
                    {
                        _branchCache.AddOrReplace(projectId, branch.Name.GetHashCode(), branch);
                    }
                }
                catch (Exception ex)
                {
                    LogTo.WarnException("Exception when trying to fetch branches from project", ex);
                    _pipelineNotifier.StoreError(ex, "ErrorFetchingBranches", project.Name);
                }
            }
        }

        private async Task FetchBuilds()
        {
            foreach (var project in _projectList)
            {
                try
                {
                    var projectId = project.GetHashCode();

                    var builds = _lastUpdate.HasValue
                        ? project.FetchBuildsChangedSince(_lastUpdate.Value)
                        : project.FetchAllBuilds();

                    await foreach (var build in builds)
                    {
                        _buildCache.AddOrReplace(projectId, build.Id.GetHashCode(), build);
                    }

                    var removedBuilds = project.FetchRemovedBuilds();

                    await foreach (var build in removedBuilds)
                    {
                        _buildCache.Remove(projectId, build.Id.GetHashCode());
                    }
                }
                catch (Exception ex)
                {
                    var projectName = project.Name;
                    LogTo.WarnException($"Exception when trying to fetch builds for project {projectName}", ex);
                    _pipelineNotifier.StoreError(ex, "ErrorFetchingBuilds", projectName);
                }
            }

            _lastUpdate = DateTime.Now;
        }

        private async Task FetchDefinitions()
        {
            foreach (var project in _projectList)
            {
                try
                {
                    var projectId = project.GetHashCode();

                    var definitions = project.FetchBuildDefinitions();
                    await foreach (var definition in definitions)
                    {
                        _definitionCache.AddOrReplace(projectId, definition.Id.GetHashCode(), definition);
                    }

                    var removedDefinitions = project.FetchRemovedBuildDefinitions();
                    await foreach (var definition in removedDefinitions)
                    {
                        _definitionCache.Remove(projectId, definition.Id.GetHashCode());
                    }
                }
                catch (Exception ex)
                {
                    LogTo.WarnException("Exception when trying to fetch BuildDefinitions from project", ex);
                    _pipelineNotifier.StoreError(ex, "ErrorFetchingDefinitions", project.Name);
                }
            }
        }

        /// <inheritdoc />
        public void AddProject(IProject project)
        {
            _projectList.Add(project);
        }

        public void ClearProjects()
        {
            _projectList.Clear();
            _definitionCache.Clear();
            _buildCache.Clear();
            _branchCache.Clear();
            _lastUpdate = null;
        }

        /// <inheritdoc />
        public async Task Update()
        {
            var treeResult = await Task.Run(async () =>
            {
                var previousBuildStatus = _buildCache.ContentCopy().ToDictionary(x => (BuildId: x.Id, Project: x.ProjectName), x => x.Status);
                var branchTask = FetchBranches();
                var definitionsTask = FetchDefinitions();
                var buildsTask = FetchBuilds();

                await Task.WhenAll(branchTask, definitionsTask, buildsTask);

                CleanupBuilds();

                var builds = _buildCache.ContentCopy().ToList();
                var branches = _branchCache.ContentCopy();
                var definitions = _definitionCache.ContentCopy();

                var tree = _treeBuilder.Build(builds, branches, definitions, _oldTree);
                CutTree(tree);

                var currentBuildNodes = tree.AllChildren().OfType<IBuildNode>();
                IBuildTreeBuildsDelta delta;

                // don't show any notifications for the initial fetch
                if (_oldTree == null)
                    delta = new BuildTreeBuildsDelta();
                else
                    delta = new BuildTreeBuildsDelta(currentBuildNodes, previousBuildStatus);

                var notifications = _notificationFactory.ProduceNotifications(delta).ToList();
                return (BuildTree: tree, Notifications: notifications);
            });

            _pipelineNotifier.Notify(treeResult.BuildTree, treeResult.Notifications);
            // notify the previously stored errors. This is to ensure that the notifications happens within the same thread
            _pipelineNotifier.NotifyErrors();

            _oldTree = treeResult.BuildTree;
        }

        private void HandleBuildUpdate(IBuildNode existingNode, IBuildNode updatedNode, BuildTreeBuildsDelta buildTreeBuildsDelta)
        {
            // status did not change, nothing to note
            if (existingNode.Status == updatedNode.Status)
                return;

            switch (updatedNode.Status)
            {
                case BuildStatus.Cancelled:
                    buildTreeBuildsDelta.CancelledBuilds.Add(existingNode);
                    break;
                case BuildStatus.Succeeded:
                case BuildStatus.PartiallySucceeded:
                    buildTreeBuildsDelta.SucceededBuilds.Add(existingNode);
                    break;
                case BuildStatus.Failed:
                    buildTreeBuildsDelta.FailedBuilds.Add(existingNode);
                    break;
            }
        }

        public IPipelineNotifier Notifier => _pipelineNotifier;

        private readonly ITreeBuilder _treeBuilder;
        private readonly IConfiguration _configuration;
        private readonly IPipelineCache<IBuild> _buildCache;
        private readonly IPipelineCache<IBranch> _branchCache;
        private readonly IPipelineCache<IBuildDefinition> _definitionCache;
        private readonly PipelineNotifier _pipelineNotifier;
        private readonly ConcurrentBag<IProject> _projectList = new ConcurrentBag<IProject>();
        private DateTime? _lastUpdate;
        private IBuildTree? _oldTree;
        private NotificationFactory _notificationFactory;
    }
}