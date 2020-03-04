﻿using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Builds.Sight;

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
        /// Adds a sight for filtering and highlighting data.
        /// </summary>
        /// <param name="sight">The sight to add.</param>
        void AddSight(ISight sight);
        
        /// <summary>
        /// Refreshes the build tree and causes all effects by changed sights to be in effect immediately.
        /// </summary>
        void ApplySightChanges();

        /// <summary>
        /// Clears all projects and cached data.
        /// </summary>
        void ClearProjects();

        /// <summary>
        /// Filters builds in the pipeline to match the term.
        /// </summary>
        /// <param name="searchTerm">
        /// Term to search for.
        /// Use empty string to clear search filter
        /// </param>
        void Search(string searchTerm);

        /// <summary>
        /// Updates the pipeline i.e. fetch data from projects, group builds and
        /// raise notifications.
        /// </summary>
        Task Update();
    }
}