using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public abstract class ListOptionBaseViewModel<TItem> : OptionViewModelBase<TItem>
    {
        protected ListOptionBaseViewModel(string displayName, TItem value = default)
            : base(value, displayName)
        {
            _initialValue = value;
        }

        public IEnumerable<ListOptionItemViewModel<TItem>> AvailableValues
        {
            get
            {
                Init();
                return _availableValues ?? new List<ListOptionItemViewModel<TItem>>();
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
                if (_selectedValue != null)
                    Value = _selectedValue.Value;
            }
        }

        protected abstract IEnumerable<TItem> ModelValues { get; }

        protected virtual string? DisplayNameFor(TItem item) => item?.ToString();

        private void Init()
        {
            if (_valuesFetched)
                return;

            _availableValues = new List<ListOptionItemViewModel<TItem>>(
                ModelValues.Select(v => new ListOptionItemViewModel<TItem>(v, DisplayNameFor(v)))
            );

            _valuesFetched = true;
            SelectedValue = AvailableValues.FirstOrDefault(v => Equals(v.Value, _initialValue));
        }

        private readonly TItem _initialValue;

        private bool _valuesFetched;
        private List<ListOptionItemViewModel<TItem>>? _availableValues;
        private ListOptionItemViewModel<TItem>? _selectedValue;
    }
}