using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search;

public interface ISearchBlock
{
    /// <summary>
    /// The original text entered for this block. Including spaces and control characters.
    /// </summary>
    string EnteredText { get; }

    ISearchCriteria SearchCriteria { get; }

    /// <summary>
    /// The searched term for this block. Parsed for easier handling. That is without control characters or
    /// spaces.
    /// </summary>
    string SearchedTerm { get; }

    bool IsBuildIncluded(IBuild build);
}