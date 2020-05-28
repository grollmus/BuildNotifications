using System;
using System.Collections.Generic;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    /// <summary>
    /// Describes a history of made searches in the past.
    /// </summary>
    public interface ISearchHistory
    {
        /// <summary>
        /// Retrieves the previously made search terms.
        /// </summary>
        /// <returns>Made search terms in descending order from most recent to oldest.</returns>
        IEnumerable<string> SearchedTerms();

        /// <summary>
        /// Adds a created search instance to this history
        /// </summary>
        /// <param name="searchTerm">The term that was searched.</param>
        /// <param name="timeSearchWasMade">Optional timestamp for when the search was made. If not specified, the current time is used.</param>
        void AddEntry(string searchTerm, DateTime? timeSearchWasMade = null);

        /// <summary>
        /// Removes the given search term from history.
        /// </summary>
        /// <param name="searchTerm">The term to remove.</param>
        void RemoveEntry(string searchTerm);
    }
}