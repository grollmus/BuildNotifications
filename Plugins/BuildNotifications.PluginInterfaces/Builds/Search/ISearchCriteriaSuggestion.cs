using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds.Search
{
    /// <summary>
    /// Describes a suggestion based on a given input for a certain <see cref="ISearchCriteria"/>.
    /// </summary>
    /// <example>
    /// For a criteria describing a certain user, the given input might be "M". A suggestions could be "Me", "Max", "Mike".
    /// </example>
    [PublicAPI]
    public interface ISearchCriteriaSuggestion
    {
        /// <summary>
        /// The suggested string.
        /// </summary>
        string Suggestion { get; }

        /// <summary>
        /// Whether this suggestion is based on keyword.
        /// </summary>
        bool IsKeyword { get; }
    }
}