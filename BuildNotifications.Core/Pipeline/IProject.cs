using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline;

/// <summary>
/// A project is a combination of a BuildProvider and SourceControlProvider
/// that can be used to gather information about a single software project.
/// </summary>
public interface IProject
{
    /// <summary>
    /// Configuration for this project.
    /// </summary>
    IProjectConfiguration Config { get; set; }

    /// <summary>
    /// Unique identifier for this project.
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    /// Display name of this project.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Fetches all builds for this project.
    /// </summary>
    /// <returns>List of builds.</returns>
    IAsyncEnumerable<IBuild> FetchAllBuilds();

    /// <summary>
    /// Fetches all available definitions for this project.
    /// </summary>
    /// <returns>List of all build definitions</returns>
    IAsyncEnumerable<IBuildDefinition> FetchBuildDefinitions();

    /// <summary>
    /// Fetches all builds for this project that have been changed since a given time.
    /// </summary>
    /// <param name="lastUpdate">Time to fetch updated builds since.</param>
    /// <returns>List of builds.</returns>
    IAsyncEnumerable<IBuild> FetchBuildsChangedSince(DateTime lastUpdate);

    /// <summary>
    /// Fetches all user identities which represent the current user. That is, the user that is actually
    /// currently running
    /// BuildNotifications.
    /// </summary>
    IEnumerable<IUser> FetchCurrentUserIdentities();

    /// <summary>
    /// Fetches all branches that exist for this project.
    /// </summary>
    /// <returns>List of branches.</returns>
    IAsyncEnumerable<IBranch> FetchExistingBranches();

    /// <summary>
    /// Fetches all branches that no longer exist for this project.
    /// </summary>
    /// <returns>List of branches.</returns>
    IAsyncEnumerable<IBranch> FetchRemovedBranches();

    /// <summary>
    /// Fetches all build definitions that no longer exist for this project.
    /// </summary>
    /// <returns>List of all build definitions.</returns>
    IAsyncEnumerable<IBuildDefinition> FetchRemovedBuildDefinitions();

    /// <summary>
    /// Fetches all builds that no longer exist for this project.
    /// </summary>
    /// <returns>List of builds.</returns>
    IAsyncEnumerable<IBuild> FetchRemovedBuilds();

    /// <summary>
    /// Updates the display name of the branches for the given builds.
    /// </summary>
    Task UpdateBuildBranches(IEnumerable<IBuild> builds, IEnumerable<IBranch> branches);

    /// <summary>
    /// Updates given builds.
    /// </summary>
    Task UpdateBuilds(IEnumerable<IBuild> builds);
}