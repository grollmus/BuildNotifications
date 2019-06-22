using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BuildNotifications.ViewModel.Utils
{
    public class RemoveTrackingObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : IRemoveTracking
    {
        private readonly ObservableCollection<T> _list;

        public TimeSpan RemoveDelay { get; set; }

        public RemoveTrackingObservableCollection() : this(TimeSpan.FromSeconds(0.15), Enumerable.Empty<T>())
        {
        }

        public RemoveTrackingObservableCollection(TimeSpan removeDelay) : this(removeDelay, Enumerable.Empty<T>())
        {
        }

        public RemoveTrackingObservableCollection(TimeSpan removeDelay, IEnumerable<T> initialValues)
        {
            RemoveDelay = removeDelay;
            _list = new ObservableCollection<T>(initialValues);
            _list.CollectionChanged += (sender, args) => CollectionChanged?.Invoke(sender, args);
        }

        public void Add(T item)
        {
            lock (_list)
            {
                _list.Add(item);
            }
        }

        public void Clear()
        {
            lock (_list)
            {
                _list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_list)
            {
                return _list.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_list)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            RemoveWithDelay(item);
            return true;
        }

        private async void RemoveWithDelay(T item)
        {
            item.IsRemoving = true;

            await Task.Delay(RemoveDelay);
            lock (_list)
            {
                if (_list.Contains(item))
                    _list.Remove(item);
            }
        }

        public int Count
        {
            get
            {
                lock (_list)
                {
                    return _list.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            lock (_list)
            {
                return _list.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item)
        {
            lock (_list)
            {
                return _list.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_list)
            {
                _list.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_list)
            {
                _list.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (_list)
                {
                    return _list[index];
                }
            }
            set
            {
                lock (_list)
                {
                    _list[index] = value;
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}