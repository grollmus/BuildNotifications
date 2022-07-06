using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.Options;

public abstract class CollectionOptionBaseViewModel<TValue, TOption> : OptionViewModelBase
    where TOption : OptionViewModelBase<TValue>
{
    protected CollectionOptionBaseViewModel(IEnumerable<TValue> value, string displayName)
        : base(displayName, string.Empty)
    {
        Values = new ObservableCollection<TOption>(value.Select(v =>
        {
            var vm = CreateNewValue(v);
            vm.ValueChanged += Item_ValueChanged;
            return vm;
        }));
        Values.CollectionChanged += Values_CollectionChanged;

        AddNewItemCommand = new DelegateCommand(AddNewItem, CanAddNewItem);
        RemoveItemCommand = new DelegateCommand<TValue>(RemoveItem, CanRemoveItem);
    }

    public ICommand AddNewItemCommand { get; }
    public ICommand RemoveItemCommand { get; }

    public ObservableCollection<TOption> Values { get; }

    protected virtual void AddNewItem()
    {
        var value = CreateNewValue();
        value.ValueChanged += Item_ValueChanged;
        Values.Add(value);
    }

    protected virtual bool CanAddNewItem() => true;
    protected virtual bool CanRemoveItem(TValue value) => true;

    protected abstract TOption CreateNewValue();
    protected abstract TOption CreateNewValue(TValue value);

    protected virtual void OnCollectionChanged()
    {
        RaiseValueChanged();
    }

    protected virtual void OnItemChanged()
    {
        RaiseValueChanged();
    }

    protected virtual void RemoveItem(TValue value)
    {
        var item = Values.FirstOrDefault(it => Equals(it.Value, value));
        if (item != null)
        {
            item.ValueChanged -= Item_ValueChanged;
            Values.Remove(item);
        }
    }

    private void Item_ValueChanged(object? sender, EventArgs e)
    {
        OnItemChanged();
    }

    private void Values_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnCollectionChanged();
    }
}