using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    /// <summary>
    /// The build pipeline that processes builds
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// Object that raises events when pipeline fetches new data or updates existing data.
        /// </summary>
        IPipelineNotifier Notifier { get; }

        /// <summary>
        /// Add a project as a source for fetching data when pipeline is updated.
        /// </summary>
        /// <param name="project">Project to add.</param>
        void AddProject(IProject project);

        /// <summary>
        /// Clears all projects and cached data.
        /// </summary>
        void ClearProjects();

        /// <summary>
        /// Applies the given search to the pipeline.
        /// </summary>
        /// <param name="specificSearch">The search that shall be applied.</param>
        void Search(ISpecificSearch specificSearch);

        /// <summary>
        /// Updates the pipeline i.e. fetch data from projects, group builds and
        /// raise notifications.
        /// </summary>
        /// <param name="mode">Mode to use for update.</param>
        Task Update(UpdateModes mode = UpdateModes.DeltaBuilds);

        /// <summary>
        /// Gets a snapshot of the currently cached builds.
        /// </summary>
        /// <returns>Read only list. Instance is not updated.</returns>
        IReadOnlyList<IBuild> CachedBuilds();

        /// <summary>
        /// Gets a snapshot of the currently cached definitions.
        /// </summary>
        /// <returns>Read only list. Instance is not updated.</returns>
        IReadOnlyList<IBuildDefinition> CachedDefinitions();

        /// <summary>
        /// Gets a snapshot of the currently cached branches.
        /// </summary>
        /// <returns>Read only list. Instance is not updated.</returns>
        IReadOnlyList<IBranch> CachedBranches();

        /// <summary>
        /// Time of the last successful update. Null if it never happened.
        /// </summary>
        DateTime? LastUpdate { get; }
    }
}