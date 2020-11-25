using System;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Search;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree.Search
{
    public class RuntimeSearchHistoryTests
    {
        [Fact]
        public void NewHistoryIsEmpty()
        {
            var history = new RuntimeSearchHistory();

            var entries = history.SearchedTerms();

            Assert.Empty(entries);
        }

        [Fact]
        public void AddingEntryToHistoryWillYieldThisEntry()
        {
            var history = new RuntimeSearchHistory();
            const string expectedTerm = "anything";
            history.AddEntry(expectedTerm);

            var entries = history.SearchedTerms();

            Assert.Contains(entries, e => e.Equals(expectedTerm, StringComparison.Ordinal));
        }

        [Fact]
        public void AddingEntriesToHistoryWillYieldTheseEntries()
        {
            var history = new RuntimeSearchHistory();
            var expectedTerms = new[] {"term1", "term2"};
            foreach (var term in expectedTerms)
            {
                history.AddEntry(term);
            }

            var entries = history.SearchedTerms();

            Assert.Equal(expectedTerms.Reverse(), entries.ToArray());
        }

        [Fact]
        public void TwiceAddedTermIsYieldedOnlyOnce()
        {
            var history = new RuntimeSearchHistory();
            const string expectedTerm = "anything";
            history.AddEntry(expectedTerm);
            history.AddEntry(expectedTerm);

            var entries = history.SearchedTerms();

            Assert.Single(entries);
        }

        [Fact]
        public void EntryAddedAgainWillUpdateItsUsedTime()
        {
            var history = new RuntimeSearchHistory();
            const string oldTerm = nameof(oldTerm);
            const string newTerm = nameof(newTerm);
            history.AddEntry(oldTerm);
            history.AddEntry(newTerm);
            history.AddEntry(oldTerm);

            var newestEntry = history.SearchedTerms().First();

            Assert.Equal(oldTerm, newestEntry);
        }

        [Fact]
        public void EntriesAreSortedByDateDescending()
        {
            var history = new RuntimeSearchHistory();
            const string oldTerm = nameof(oldTerm);
            const string newTerm = nameof(newTerm);
            history.AddEntry(oldTerm);
            history.AddEntry(newTerm);
            var inOrder = new[] {newTerm, oldTerm};

            var entries = history.SearchedTerms();

            Assert.Equal(inOrder, entries);
        }

        [Fact]
        public void ManuallyGivenDateIsRespected()
        {
            var history = new RuntimeSearchHistory();
            const string oldTerm = nameof(oldTerm);
            const string newTerm = nameof(newTerm);
            history.AddEntry(oldTerm, DateTime.Now + TimeSpan.FromHours(1));
            history.AddEntry(newTerm);
            var inOrder = new[] {oldTerm, newTerm};

            var entries = history.SearchedTerms();

            Assert.Equal(inOrder, entries);
        }

        [Fact]
        public void RemovedTermIsNotYielded()
        {
            var history = new RuntimeSearchHistory();
            const string expectedTerm = "anything";
            history.AddEntry(expectedTerm);
            history.RemoveEntry(expectedTerm);

            var entries = history.SearchedTerms();

            Assert.Empty(entries);
        }

        [Fact]
        public void RemovingTermThatIsNotInListDoesNothing()
        {
            var history = new RuntimeSearchHistory();
            const string anything = nameof(anything);
            history.RemoveEntry(anything);

            var entries = history.SearchedTerms();

            Assert.Empty(entries);
        }
    }
}