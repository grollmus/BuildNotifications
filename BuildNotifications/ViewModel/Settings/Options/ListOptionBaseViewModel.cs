using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BuildNotifications.ViewModel.Settings.Options;

public abstract class ListOptionBaseViewModel<TItem> : OptionViewModelBase<TItem>
{
    protected ListOptionBaseViewModel(string displayName, TItem value = default!)
        : base(ValueOrNewInstance(value), displayName)
    {
        _initialValue = ValueOrNewInstance(value);
    }

    public IEnumerable<ListOptionItemViewModel<TItem>> AvailableValues
    {
        get
        {
            Init();
            return _availableValues;
        }
    }

    public ListOptionItemViewModel<TItem>? SelectedValue
    {
        get => _selectedValue;
        set
        {
            if (Equals(_selectedValue, value))
                return;

            _selectedValue = value;
            OnPropertyChanged(nameof(SelectedValue));

            if (_selectedValue != null)
                Value = _selectedValue.Value;
        }
    }

    protected abstract IEnumerable<TItem> ModelValues { get; }

    protected virtual string DisplayNameFor(TItem item) => item?.ToString() ?? string.Empty;

    protected void InvalidateAvailableValues()
    {
        _shouldFetchValues = true;
        OnPropertyChanged(nameof(AvailableValues));
    }

    private void Init()
    {
        if (!_shouldFetchValues)
            return;

        _availableValues.Clear();
        foreach (var option in ModelValues.Select(v => new ListOptionItemViewModel<TItem>(v, DisplayNameFor(v))))
        {
            _availableValues.Add(option);
        }

        _shouldFetchValues = false;

        var valueToSelect = Value;
        if (!_initialized)
        {
            _initialized = true;
            valueToSelect = _initialValue;
        }

        SelectedValue = AvailableValues.FirstOrDefault(v => Equals(v.Value, valueToSelect));
    }

    private static TItem ValueOrNewInstance(object? item)
    {
        if (item is TItem tItem)
            return tItem;

        if (item != null)
            return (TItem)item;

        return Activator.CreateInstance<TItem>();
    }

    private readonly TItem _initialValue;
    private readonly ObservableCollection<ListOptionItemViewModel<TItem>> _availableValues = new();
    private bool _shouldFetchValues = true;
    private ListOptionItemViewModel<TItem>? _selectedValue;
    private bool _initialized;
}