using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public abstract class CollectionOptionBaseViewModel<TValue, TOption> : OptionViewModelBase
        where TOption : OptionViewModelBase<TValue>
    {
        protected CollectionOptionBaseViewModel(IEnumerable<TValue> value, string displayName)
        {
            DisplayName = displayName;
            Values = new ObservableCollection<TOption>(value.Select(CreateNewValue));

            AddNewItemCommand = new DelegateCommand(AddNewItem, CanAddNewItem);
            RemoveItemCommand = new DelegateCommand<TValue>(RemoveItem, CanRemoveItem);
        }

        public ICommand AddNewItemCommand { get; }
        public string DisplayName { get; }
        public ICommand RemoveItemCommand { get; }

        public ObservableCollection<TOption> Values { get; }

        protected virtual void AddNewItem()
        {
            Values.Add(CreateNewValue());
        }

        protected virtual bool CanAddNewItem() => true;
        protected virtual bool CanRemoveItem(TValue value) => true;

        protected abstract TOption CreateNewValue();
        protected abstract TOption CreateNewValue(TValue value);

        protected virtual void RemoveItem(TValue value)
        {
            var item = Values.FirstOrDefault(it => Equals(it.Value, value));
            if (item != null)
                Values.Remove(item);
        }
    }
}