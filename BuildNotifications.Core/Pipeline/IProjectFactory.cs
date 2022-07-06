using System;
using BuildNotifications.Core.Config;

namespace BuildNotifications.Core.Pipeline;

/// <summary>
/// Factory to construct projects from user configured connections.
/// </summary>
internal interface IProjectFactory
{
    /// <summary>
    /// Gets called whenever an error occurs when constructing a project.
    /// </summary>
    event EventHandler<ErrorNotificationEventArgs> ErrorOccured;

    /// <summary>
    /// Constructs a project as a combination of connection data.
    /// </summary>
    /// <param name="config">Configuration that is used to construct the project.</param>
    /// <returns>The constructed project or <c>null</c> if construction failed.</returns>
    IProject? Construct(IProjectConfiguration config);
}