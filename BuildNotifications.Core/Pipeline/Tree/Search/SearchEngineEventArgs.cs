using System;

namespace BuildNotifications.Core.Pipeline.Tree.Search;

public class SearchEngineEventArgs : EventArgs
{
    public SearchEngineEventArgs(ISpecificSearch createdSearch, string parsedText)
    {
        CreatedSearch = createdSearch;
        ParsedText = parsedText;
    }

    public ISpecificSearch CreatedSearch { get; }

    public string ParsedText { get; }
}