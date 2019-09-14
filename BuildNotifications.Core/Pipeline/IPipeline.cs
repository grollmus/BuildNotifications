using System.Threading.Tasks;

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