using System;
using System.Collections.Generic;
using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    /// <summary>
    /// A project is a combination of a BuildProvider and SourceControlProvider
    /// that can be used to gather information about a single software project.
    /// </summary>
    public interface IProject
    {
        /// <summary>
        /// Branch provider that is used to fetch branch information for this project.
        /// </summary>
        IBranchProvider BranchProvider { get; set; }

        /// <summary>
        /// Build provider that is used to fetch build information for this project.
        /// </summary>
        IBuildProvider BuildProvider { get; set; }

        /// <summary>
        /// Configuration for this project.
        /// </summary>
        IProjectConfiguration Config { get; set; }

        /// <summary>
        /// Display name of this project.
        /// </summary>
        string Name { get; set; }

        IAsyncEnumerable<IBuild> FetchAllBuilds();
        IAsyncEnumerable<IBuild> FetchBuildsChangedSince(DateTime lastUpdate);
    }
}