using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree.Search;

namespace BuildNotifications.Resources.Search;

internal class EmptySearchHistory : ISearchHistory
{
    public IEnumerable<string> SearchedTerms()
    {
        yield break;
    }

    public void AddEntry(string searchTerm, DateTime? timeSearchWasMade = null)
    {
        // unused
    }

    public void RemoveEntry(string searchTerm)
    {
        // unused
    }
}