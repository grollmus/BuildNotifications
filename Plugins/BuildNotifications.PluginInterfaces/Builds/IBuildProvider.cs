using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// Fetches all available builds.
        /// </summary>
        /// <param name="buildsPerGroup">Amount of builds to fetch per group (e.g. branch or definition).</param>
        /// <returns>List of all builds.</returns>
        IAsyncEnumerable<IBaseBuild> FetchAllBuilds(int buildsPerGroup);

        /// <summary>
        /// Fetches all builds that have been changed since a given date and time.
        /// </summary>
        /// <remarks>
        /// Changed means the build has been newly created or changed one or more
        /// of its properties (e.g. status or progress).
        /// <paramref name="date" /> is given in UTC.
        /// </remarks>
        /// <param name="date">Date and time since when to fetch builds.</param>
        /// <returns>List of all builds changed since <paramref name="date" />.</returns>
        IAsyncEnumerable<IBaseBuild> FetchBuildsChangedSince(DateTime date);

        /// <summary>
        /// Fetches all builds for a given build definition.
        /// </summary>
        /// <param name="definition">Definition to fetch builds for.</param>
        /// <returns>List of all builds available for the given definition.</returns>
        IAsyncEnumerable<IBaseBuild> FetchBuildsForDefinition(IBuildDefinition definition);

        /// <summary>
        /// Fetches all existing build definitions.
        /// </summary>
        /// <returns>List of all existing build definitions.</returns>
        IAsyncEnumerable<IBuildDefinition> FetchExistingBuildDefinitions();

        /// <summary>
        /// Returns a list of build definitions that no longer exist in the source.
        /// </summary>
        /// <returns>List of removed build definitions.</returns>
        IAsyncEnumerable<IBuildDefinition> RemovedBuildDefinitions();

        /// <summary>
        /// Returns a list of builds that no longer exist in the source.
        /// </summary>
        /// <returns>List of removed builds.</returns>
        IAsyncEnumerable<IBaseBuild> RemovedBuilds();

        /// <summary>
        /// Updates data (e.g. progress) for the given builds.
        /// </summary>
        /// <param name="builds">List of builds to update</param>
        Task UpdateBuilds(IEnumerable<IBaseBuild> builds);
    }
}