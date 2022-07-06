using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Cache;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NLog.Fluent;

namespace BuildNotifications.Core.Pipeline;

internal class Pipeline : IPipeline
{
    public Pipeline(ITreeBuilder treeBuilder, IConfiguration configuration, IUserIdentityList userIdentityList)
    {
        _treeBuilder = treeBuilder;
        _configuration = configuration;
        _userIdentityList = userIdentityList;
        _buildCache = new PipelineCache<IBuild>();
        _branchCache = new PipelineCache<IBranch>();
        _definitionCache = new PipelineCache<IBuildDefinition>();
        _notificationFactory = new NotificationFactory(configuration, userIdentityList);
        _pipelineNotifier = new PipelineNotifier();

        _currentSearch = new EmptySearch();
    }

    internal void ReplaceCaches(IPipelineCache<IBuild> buildCache, IPipelineCache<IBranch> branchCache, IPipelineCache<IBuildDefinition> definitionCache)
    {
        _buildCache = buildCache;
        _branchCache = branchCache;
        _definitionCache = definitionCache;
    }

    private IBuildTree BuildTree()
    {
        Log.Debug().Message("Creating BuildTree").Write();
        var builds = _buildCache.ContentCopy().ToList();
        var branches = _branchCache.ContentCopy().ToList();
        var definitions = _definitionCache.ContentCopy().ToList();
        Log.Debug().Message($"{builds.Count} cached builds, {branches.Count} cached branches, {definitions.Count} cached definitions").Write();

        var tree = _treeBuilder.Build(builds, _oldTree, _currentSearch);
        Log.Debug().Message("Created tree.").Write();
        if (_configuration.GroupDefinition.Any())
        {
            Log.Debug().Message("Cutting tree.").Write();
            CutTree(tree);
        }

        return tree;
    }

    private void CleanupBuilds()
    {
        Log.Debug().Message("Cleaning up builds").Write();
        var builds = _buildCache.ContentCopy();
        var count = 0;
        IProject? projectOfBuild = null;

        foreach (var build in builds.Cast<EnrichedBuild>())
        {
            if (projectOfBuild == null || build.ProjectId != projectOfBuild.Guid)
                projectOfBuild = _projectList.FirstOrDefault(p => p.Guid == build.ProjectId);

            var hideWithNoBranch = projectOfBuild?.Config.HideBuildsOfDeletedBranches ?? true;
            var hideWithNoDefinition = projectOfBuild?.Config.HideBuildsOfDeletedDefinitions ?? true;

            var definitionExists = _definitionCache.ContainsValue(build.Definition);
            var branchExists = _branchCache.Contains(b => b.Equals(build.Branch));

            var shouldHideBecauseOfDefinition = hideWithNoDefinition && !definitionExists;
            var shouldHideBecauseOfBranch = hideWithNoBranch && !branchExists;

            if (shouldHideBecauseOfDefinition || shouldHideBecauseOfBranch)
            {
                _buildCache.RemoveValue(build);
                count += 1;
            }
        }

        Log.Debug().Message($"Cleaned {count} builds").Write();
    }

    private void CutTree(IBuildTreeNode tree)
    {
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

    private async Task<bool> FetchBranches()
    {
        Log.Debug().Message("Fetching branches").Write();
        var success = true;
        foreach (var project in _projectList)
        {
            Log.Debug().Message($"Fetching branches for project \"{project.Name}\"").Write();
            try
            {
                var branches = project.FetchExistingBranches();
                var count = 0;
                await foreach (var branch in branches)
                {
                    _branchCache.AddOrReplace(branch.CacheKey(project), branch);
                    count += 1;
                }

                Log.Debug().Message($"Added \"{count}\" branches in project \"{project.Name}\"").Write();

                var removedBranches = project.FetchRemovedBranches();

                count = 0;
                await foreach (var branch in removedBranches)
                {
                    _branchCache.Remove(branch.CacheKey(project));
                    count += 1;
                }

                Log.Debug().Message($"Removed \"{count}\" branches in project \"{project.Name}\"").Write();
            }
            catch (Exception ex)
            {
                ReportError("ErrorFetchingBranches", project.Name, ex);
                success = false;
            }
        }

        Log.Debug().Message("Done fetching branches").Write();
        return success;
    }

    private async Task<bool> FetchBuilds(UpdateModes updateMode)
    {
        Log.Debug().Message("Fetching builds").Write();
        var success = true;
        foreach (var project in _projectList)
        {
            try
            {
                Log.Debug().Message($"Fetching builds for project \"{project.Name}\". ID: \"{project.Guid}\"").Write();

                IAsyncEnumerable<IBuild> builds;
                if (LastUpdate.HasValue && !updateMode.HasFlag(UpdateModes.AllBuilds))
                {
                    Log.Debug().Message($"Fetching all builds since {LastUpdate.Value} for project \"{project.Name}\"").Write();
                    builds = project.FetchBuildsChangedSince(LastUpdate.Value);
                }
                else
                {
                    Log.Debug().Message($"Fetching all builds for project \"{project.Name}\"").Write();
                    builds = project.FetchAllBuilds();
                }

                var count = 0;
                await foreach (var build in builds)
                {
                    _buildCache.AddOrReplace(build.CacheKey(), build);
                    count += 1;
                }

                Log.Debug().Message($"Added \"{count}\" builds in project \"{project.Name}\". Build cache total: {_buildCache.Size}").Write();
                var removedBuilds = project.FetchRemovedBuilds();
                count = 0;
                await foreach (var build in removedBuilds)
                {
                    _buildCache.Remove(build.CacheKey());
                    count += 1;
                }

                Log.Debug().Message($"Removed \"{count}\" builds in project \"{project.Name}\"").Write();
            }
            catch (Exception ex)
            {
                var projectName = project.Name;
                ReportError("ErrorFetchingBuilds", projectName, ex);
                success = false;
            }
        }

        Log.Debug().Message("Done fetching builds").Write();
        LastUpdate = DateTime.UtcNow;
        return success;
    }

    private async Task<bool> FetchDefinitions()
    {
        Log.Debug().Message("Fetching definitions").Write();
        var success = true;
        foreach (var project in _projectList)
        {
            Log.Debug().Message($"Fetching definitions for project \"{project.Name}\"").Write();
            try
            {
                var definitions = project.FetchBuildDefinitions();
                var count = 0;
                await foreach (var definition in definitions)
                {
                    _definitionCache.AddOrReplace(definition.CacheKey(project), definition);
                    count += 1;
                }

                Log.Debug().Message($"Added \"{count}\" definitions in project \"{project.Name}\"").Write();

                var removedDefinitions = project.FetchRemovedBuildDefinitions();
                count = 0;
                await foreach (var definition in removedDefinitions)
                {
                    _definitionCache.Remove(definition.CacheKey(project));
                    count += 1;
                }

                Log.Debug().Message($"Added \"{count}\" definitions in project \"{project.Name}\"").Write();
            }
            catch (Exception ex)
            {
                ReportError("ErrorFetchingDefinitions", project.Name, ex);
                success = false;
            }
        }

        Log.Debug().Message("Done fetching definitions").Write();
        return success;
    }

    private void ReportError(string messageTextId, params object[] parameter)
    {
        var localizedMessage = StringLocalizer.Instance.GetText(messageTextId);
        var fullMessage = string.Format(StringLocalizer.CurrentCulture, localizedMessage, parameter);
        if (parameter.FirstOrDefault(x => x is Exception) is Exception exception)
            Log.Error().Message(fullMessage).Exception(exception).Write();
        else
            Log.Error().Message(fullMessage).Write();
    }

    private async Task UpdateBuilds()
    {
        Log.Debug().Message("Updating builds.").Write();
        foreach (var project in _projectList)
        {
            Log.Debug().Message($"Updating builds of project \"{project.Name}\".").Write();
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
        Log.Info().Message($"Adding project \"{project.Name}\"").Write();
        _projectList.Add(project);
        try
        {
            var currentUserIdentities = project.FetchCurrentUserIdentities();
            foreach (var currentUserIdentity in currentUserIdentities.WhereNotNull())
            {
                Log.Debug().Message($"Adding identity \"{currentUserIdentity.UniqueName}\" from project \"{project.Name}\"").Write();
                _userIdentityList.IdentitiesOfCurrentUser.Add(currentUserIdentity);
            }
        }
        catch (Exception e)
        {
            Log.Error().Message($"Failed to fetch identities of project {project.Name}").Exception(e).Write();
            ReportError("ErrorFetchingUserIdentities", project.Name, e);
        }
    }

    public void ClearProjects()
    {
        Log.Info().Message("Clearing projects and all cached data.").Write();
        _projectList.Clear();
        _definitionCache.Clear();
        _buildCache.Clear();
        _branchCache.Clear();
        LastUpdate = null;
        _oldTree = null;
        _userIdentityList.IdentitiesOfCurrentUser.Clear();
    }

    public void Search(ISpecificSearch specificSearch)
    {
        Log.Info().Message($"Applying search \"{specificSearch}\".").Write();
        _currentSearch = specificSearch;

        var tree = BuildTree();
        _pipelineNotifier.Notify(tree, Enumerable.Empty<INotification>());
        Log.Debug().Message($"Applied search \"{specificSearch}\".").Write();
    }

    public async Task<bool> Update(UpdateModes mode = UpdateModes.DeltaBuilds)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        Log.Info().Message("Starting update.").Write();
        var (buildTree, list, updateSuccess) = await Task.Run(async () =>
        {
            var previousBuildStatus = _buildCache.CachedValues().ToDictionary(p => p.Key, p => p.Value.Status);

            // Fetch branches first so they are known when fetching builds
            var branchTask = FetchBranches();
            var success = await branchTask;

            var definitionsTask = FetchDefinitions();
            var buildsTask = FetchBuilds(mode);

            success = success && (await Task.WhenAll(definitionsTask, buildsTask)).All(x => x);

            Log.Debug().Message("Everything is fetched.").Write();

            await UpdateBuilds();

            CleanupBuilds();

            var tree = BuildTree();

            var currentBuildNodes = tree.AllChildren().OfType<IBuildNode>();

            Log.Debug().Message("BuildTree is done. Producing notifications.").Write();
            // don't show any notifications for the initial fetch
            IBuildTreeBuildsDelta delta = _oldTree == null
                ? new BuildTreeBuildsDelta()
                : new BuildTreeBuildsDelta(currentBuildNodes, previousBuildStatus, _configuration.PartialSucceededTreatmentMode);

            var notifications = _notificationFactory.ProduceNotifications(delta).ToList();
            return (BuildTree: tree, Notifications: notifications, UpdateSuccess: success);
        });

        Log.Debug().Message("Calling notify. Notification count: {Count}", list.Count).Write();
        _pipelineNotifier.Notify(buildTree, list);

        _oldTree = buildTree;
        stopWatch.Stop();
        Log.Info().Message($"Update done in {stopWatch.ElapsedMilliseconds} ms.").Write();

        return updateSuccess;
    }

    public IReadOnlyList<IBuild> CachedBuilds() => _buildCache.ContentCopy().ToList();

    public IReadOnlyList<IBuildDefinition> CachedDefinitions() => _definitionCache.ContentCopy().ToList();

    public IReadOnlyList<IBranch> CachedBranches() => _branchCache.ContentCopy().ToList();

    public DateTime? LastUpdate { get; private set; }

    public IPipelineNotifier Notifier => _pipelineNotifier;

    private readonly ITreeBuilder _treeBuilder;
    private readonly IConfiguration _configuration;
    private readonly IUserIdentityList _userIdentityList;
    private readonly PipelineNotifier _pipelineNotifier;
    private readonly ConcurrentBag<IProject> _projectList = new();
    private readonly NotificationFactory _notificationFactory;
    private IPipelineCache<IBuild> _buildCache;
    private IPipelineCache<IBranch> _branchCache;
    private IPipelineCache<IBuildDefinition> _definitionCache;
    private IBuildTree? _oldTree;
    private ISpecificSearch _currentSearch;
}