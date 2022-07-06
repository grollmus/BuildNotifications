using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds.Search;

/// <summary>
/// Defines a criteria which may be used within the global search.
/// </summary>
[PublicAPI]
public interface ISearchCriteria
{
    /// <summary>
    /// Localized examples for search terms for this search criteria. Only inputs. Not including the
    /// keyword.
    /// </summary>
    IEnumerable<string> LocalizedExamples { get; }

    /// <summary>
    /// Evaluates the given build by this criteria and the given user input.
    /// </summary>
    /// <param name="build">The build to be evaluated.</param>
    /// <param name="input">The user-made input for this criteria.</param>
    /// <returns>True when this criteria matches the build and it shall be included, false otherwise.</returns>
    bool IsBuildIncluded(IBuild build, string input);

    /// <summary>
    /// A short, localized description of what this search criteria does.
    /// </summary>
    /// <param name="forCulture">
    /// The current culture of the application. The returned string is expected to
    /// be in this cultures language.
    /// </param>
    /// <example>
    /// 'Specifies build of certain users.'
    /// </example>
    string LocalizedDescription(CultureInfo forCulture);

    /// <summary>
    /// The keyword to for this criteria. Case-insensitive. Is expected to be localized.
    /// </summary>
    /// <param name="forCulture">
    /// The current culture of the application. The returned string is expected to
    /// be in this cultures language.
    /// </param>
    /// <example>
    /// 'RequestedBy' could be the keyword for a search criteria, to search for the person which requested
    /// a build. E.g. 'RequestedBy: Me'
    /// </example>
    string LocalizedKeyword(CultureInfo forCulture);

    /// <summary>
    /// Resolve suggestions to auto-complete for the given input.
    /// </summary>
    /// <param name="input">The user-made input for this criteria.</param>
    /// <returns>Suggestions for the given input in order of relevancy. Most relevant first.</returns>
    IEnumerable<ISearchCriteriaSuggestion> Suggest(string input);
}