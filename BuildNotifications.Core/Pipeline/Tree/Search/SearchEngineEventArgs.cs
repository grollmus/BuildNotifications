using System;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    public class SearchEngineEventArgs : EventArgs
    {
        public ISpecificSearch CreatedSearch { get; }

        public string ParsedText { get; }

        public SearchEngineEventArgs(ISpecificSearch createdSearch, string parsedText)
        {
            CreatedSearch = createdSearch;
            ParsedText = parsedText;
        }
    }
}