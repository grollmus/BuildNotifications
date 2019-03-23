using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds
{
    /// <summary>
    /// Provider that can obtain builds from a source.
    /// </summary>
    [PublicAPI]
    public interface IBuildProvider
    {
        /// <summary>
        /// Identity of the user on whose behalf requests are made by this provider.
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// Fetches all builds for a given build definition.
        /// </summary>
        /// <param name="definition">Definition to fetch builds for.</param>
        /// <returns>List of all builds available for the given definition.</returns>
        IAsyncEnumerable<IBuild> FetchBuildsForDefinition(IBuildDefinition definition);

        /// <summary>
        /// Fetches all builds for since a given date and time.
        /// </summary>
        /// <param name="date">Date and time since when to fetch builds.</param>
        /// <returns>List of all builds since <paramref name="date" />.</returns>
        IAsyncEnumerable<IBuild> FetchBuildsSince(DateTime date);

        /// <summary>
        /// Fetches all existing build definitions.
        /// </summary>
        /// <returns>List of all existing build definitions.</returns>
        IAsyncEnumerable<IBuildDefinition> FetchExistingBuildDefinitions();
    }
}