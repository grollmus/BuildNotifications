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
            _list.CollectionChanged += (sender, args) => { CollectionChanged?.Invoke(sender, args); };

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
            if (item.IsRemoving && _list.Contains(item))
                _list.Remove(item);
        }

        private void Sort()
        {
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
            if (_list.Contains(item))
            {
                item.IsRemoving = false;
                _list.Remove(item);
            }

            _list.Add(item);

            Sort();
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item) => _list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            RemoveWithDelay(item).IgnoreResult();
            return true;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator() => _list.ToList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item) => _list.IndexOf(item);

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

        private readonly ObservableCollection<T> _list;

        private IEnumerable<T> _sortFunction;
    }
}