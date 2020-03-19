using System.Collections.Generic;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds.Search
{
    /// <summary>
    /// Defines a criteria which may be used within the global search.
    /// </summary>
    [PublicAPI]
    public interface ISearchCriteria
    {
        /// <summary>
        /// The keyword to search by this criteria. Case-insensitive. Is expected to be localized.
        /// </summary>
        /// <example>
        /// For a search criteria by which user the keyword could be 'RequestedBy'. E.g. 'RequestedBy: Me'
        /// </example>
        string LocalizedKeyword { get; }

        /// <summary>
        /// A short, localized description of what this search criteria does.
        /// </summary>
        /// <example>
        /// 'Specifies build of certain users.'
        /// </example>
        string LocalizedDescription { get; }

        /// <summary>
        /// Resolve suggestions to auto-complete for the given input.
        /// </summary>
        /// <param name="input">The user-made input for this criteria.</param>
        /// <returns>Suggestions for the given input in order of relevancy. Most relevant first.</returns>
        IEnumerable<ISearchCriteriaSuggestion> Suggest(string input);

        /// <summary>
        /// Evaluates the given build by this criteria and the given user input.
        /// </summary>
        /// <param name="build">The build to be evaluated.</param>
        /// <param name="input">The user-made input for this criteria.</param>
        /// <returns>True when this criteria matches the build and it shall be included, false otherwise.</returns>
        bool IsBuildIncluded(IBuild build, string input);

        /// <summary>
        /// Localized examples for search terms for this search criteria.
        /// </summary>
        IEnumerable<string> LocalizedExamples { get; }
    }
}
