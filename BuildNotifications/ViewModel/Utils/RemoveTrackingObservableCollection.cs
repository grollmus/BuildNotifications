using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core;

namespace BuildNotifications.ViewModel.Utils
{
    public class RemoveTrackingObservableCollection<T> : IList<T>, INotifyCollectionChanged
        where T : IRemoveTracking
    {
        public RemoveTrackingObservableCollection()
            : this(TimeSpan.FromSeconds(0.15), Enumerable.Empty<T>())
        {
        }

        public RemoveTrackingObservableCollection(TimeSpan removeDelay)
            : this(removeDelay, Enumerable.Empty<T>())
        {
        }

        public RemoveTrackingObservableCollection(TimeSpan removeDelay, IEnumerable<T> initialValues)
        {
            RemoveDelay = removeDelay;
            _list = new ObservableCollection<T>(initialValues);
            _list.CollectionChanged += (sender, args) =>
            {
                CollectionChanged?.Invoke(sender, args);

                if (_batchChangesTokens <= 0)
                {
                    _recordedEvents.Add(args);
                    return;
                }

                BatchCollectionChanged?.Invoke(sender, args);
            };

            _sortFunction = _list;
        }

        public TimeSpan RemoveDelay { get; set; }

        public void DontSort()
        {
            _sortFunction = _list;
            Sort();
        }

        public void InvokeSort()
        {
            Sort();
        }

        public void Sort<TKey>(Func<T, TKey> keySelector)
        {
            _sortFunction = _list.OrderBy(keySelector);

            Sort();
        }

        public void SortDescending<TKey>(Func<T, TKey> keySelector)
        {
            _sortFunction = _list.OrderByDescending(keySelector);

            Sort();
        }

        private async Task RemoveWithDelay(T item)
        {
            item.IsRemoving = true;

            await Task.Delay(RemoveDelay);
            if (_list.Contains(item))
                _list.Remove(item);
        }

        private void Sort()
        {
            if (_sortFunction == null)
                return;

            var sortedItemsList = _sortFunction.ToList();

            foreach (var item in sortedItemsList)
            {
                var oldIndex = IndexOf(item);
                var newIndex = sortedItemsList.IndexOf(item);
                if (oldIndex == newIndex)
                    continue;

                _list.Move(oldIndex, newIndex);
            }
        }

        public void Add(T item)
        {
            if (item != null)
                item.IsRemoving = false;
            _list.Add(item);

            Sort();
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            RemoveWithDelay(item).FireAndForget();
            return true;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            return _list.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);

            Sort();
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public event NotifyCollectionChangedEventHandler? BatchCollectionChanged;

        private readonly ObservableCollection<T> _list;

        private IEnumerable<T> _sortFunction;

        #region ChangeBatching

        public void AddRange(IEnumerable<T> items)
        {
#pragma warning disable S1481 // Unused local variables should be removed - it *is* used. However the changes are implicit side effects
            using var batchChanges = BatchChanges();
#pragma warning restore S1481

            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void RemoveRange(IEnumerable<T> items)
        {
#pragma warning disable S1481 // Unused local variables should be removed - it *is* used. However the changes are implicit side effects
            using var batchChanges = BatchChanges();
#pragma warning restore S1481

            foreach (var item in items)
            {
                Remove(item);
            }
        }

        /// <summary>
        /// Every invocation of CollectionChanged is piped to BatchCollectionChanged. This flag toggles this behaviour.
        /// </summary>
#pragma warning disable S3459 // Unassigned members should be removed - it *is* used. The code analyzer just fails to properly resolve the usage
        private int _batchChangesTokens;
#pragma warning restore S3459

        private readonly IList<NotifyCollectionChangedEventArgs> _recordedEvents = new List<NotifyCollectionChangedEventArgs>();

        private void InvokeBatchedChanges()
        {
            if (_batchChangesTokens != 0 || _recordedEvents.Count == 0)
                return;

            // replay all events. Batch all sequential add/remove events

            ReplayChangedEventsBatched(_recordedEvents);

            _recordedEvents.Clear();
        }

        private void ReplayChangedEventsBatched(IList<NotifyCollectionChangedEventArgs> recordedEvents)
        {
            var allAddedItems = new HashSet<T>();
            var allRemovedItems = new HashSet<T>();

            void PublishBatchedEvent()
            {
                if (allAddedItems.Count != 0)
                    BatchCollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, allAddedItems));

                if (allRemovedItems.Count != 0)
                    BatchCollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, allRemovedItems));

                allAddedItems.Clear();
                allRemovedItems.Clear();
            }

            foreach (var recordedEvent in recordedEvents)
            {
                switch (recordedEvent.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var addedItem in recordedEvent.NewItems.OfType<T>())
                        {
                            allAddedItems.Add(addedItem);
                            allRemovedItems.Remove(addedItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var removedItem in recordedEvent.OldItems.OfType<T>())
                        {
                            allAddedItems.Remove(removedItem);
                            allRemovedItems.Add(removedItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Move:
                        PublishBatchedEvent();
                        BatchCollectionChanged?.Invoke(this, recordedEvent);
                        break;
                }
            }

            PublishBatchedEvent();
        }

        private void ReplayChangedEvents(IList<NotifyCollectionChangedEventArgs> recordedEvents)
        {
            var allAddedItems = new HashSet<T>();
            var allRemovedItems = new HashSet<T>();

            void PublishEventSoFar()
            {
                foreach (var addedItem in allAddedItems)
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItem.Yield()));
                }

                foreach (var removedItem in allRemovedItems)
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem.Yield()));
                }

                allAddedItems.Clear();
                allRemovedItems.Clear();
            }

            foreach (var recordedEvent in recordedEvents)
            {
                switch (recordedEvent.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var addedItem in recordedEvent.NewItems.OfType<T>())
                        {
                            allAddedItems.Add(addedItem);
                            allRemovedItems.Remove(addedItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var removedItem in recordedEvent.OldItems.OfType<T>())
                        {
                            allAddedItems.Remove(removedItem);
                            allRemovedItems.Add(removedItem);
                        }

                        break;
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Move:
                        PublishEventSoFar();
                        BatchCollectionChanged?.Invoke(this, recordedEvent);
                        break;
                }
            }

            PublishEventSoFar();
        }

        #endregion

        /// <summary>
        /// Makes the collection batch all add and remove actions. After the returned disposable is disposed, all add and remove actions are replaced.
        /// <see cref="BatchCollectionChanged"/> is called once for add and remove with all changed items in one list. While
        /// <see cref="CollectionChanged"/> will call all changes of each item in single events.
        /// </summary>
        /// <returns>Disposable when disposed, replays all changes. If multiple instances are used, only the last disposed one will trigger the events.</returns>
        public IDisposable BatchChanges() => new BatchChangesToken(this);

        private class BatchChangesToken : IDisposable
        {
            private readonly RemoveTrackingObservableCollection<T> _collection;

            public BatchChangesToken(RemoveTrackingObservableCollection<T> collection)
            {
                _collection = collection;
                collection._batchChangesTokens += 1;
            }

            public void Dispose()
            {
                _collection._batchChangesTokens -= 1;
                _collection.InvokeBatchedChanges();
            }
        }
    }
}