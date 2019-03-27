using BuildNotifications.Core.Config;

namespace BuildNotifications.Core.Pipeline
{
    /// <summary>
    /// Factory to construct projects from user configured connections.
    /// </summary>
    internal interface IProjectFactory
    {
        /// <summary>
        /// Constructs a project as a combination of connection data.
        /// </summary>
        /// <param name="buildConnectionData">Connection to use for fetching build information.</param>
        /// <param name="sourceConnectionData">Connection to use for fetching source control information.</param>
        /// <returns>The constructed project or <c>null</c> if construction failed.</returns>
        IProject? Construct(IConnectionData buildConnectionData, IConnectionData sourceConnectionData);
    }
}