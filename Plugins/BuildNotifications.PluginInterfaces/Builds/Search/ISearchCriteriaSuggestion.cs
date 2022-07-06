using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds.Search;

/// <summary>
/// Describes a suggestion based on a given input for a certain <see cref="ISearchCriteria" />.
/// </summary>
/// <example>
/// For a criteria describing a certain user, the given input might be "M". A suggestions could be
/// "Me", "Max", "Mike".
/// </example>
[PublicAPI]
public interface ISearchCriteriaSuggestion
{
    /// <summary>
    /// Whether this suggestion is a keyword. E.g. input "br" would yield a "branch:" suggestion. Which
    /// itself is a keyword, rather than a value for a keyword.
    /// </summary>
    bool IsKeyword { get; }

    /// <summary>
    /// The suggested string.
    /// </summary>
    string Suggestion { get; }
}