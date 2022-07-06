using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.Core.Pipeline.Tree.Search;

/// <summary>
/// Search history that is retained for the duration of the runtime
/// </summary>
public class RuntimeSearchHistory : ISearchHistory
{
    private DateTime TimeTermWasUsed(string searchTerm)
    {
        if (_timeStampsOfTerms.TryGetValue(searchTerm, out var dateTime))
            return dateTime;

        return DateTime.MinValue;
    }

    public IEnumerable<string> SearchedTerms() => _pastSearchedTerms.OrderByDescending(TimeTermWasUsed);

    public void AddEntry(string searchTerm, DateTime? timeSearchWasMade = null)
    {
        var timeStamp = timeSearchWasMade ?? DateTime.Now;

        _pastSearchedTerms.Add(searchTerm);
        _timeStampsOfTerms[searchTerm] = timeStamp;
    }

    public void RemoveEntry(string searchTerm)
    {
        _pastSearchedTerms.Remove(searchTerm);
    }

    private readonly HashSet<string> _pastSearchedTerms = new();

    private readonly IDictionary<string, DateTime> _timeStampsOfTerms = new Dictionary<string, DateTime>();
}