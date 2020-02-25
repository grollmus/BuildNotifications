using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;

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

        public ListOptionItemViewModel<TItem>? SelectedValue { get; set; }

        protected abstract IEnumerable<TItem> ModelValues { get; }

        private void Init()
        {
            if (_valuesFetched)
                return;

            _availableValues = new List<ListOptionItemViewModel<TItem>>(
                ModelValues.Select(v => new ListOptionItemViewModel<TItem>(v, v?.ToString()))
            );

            _valuesFetched = true;
            SelectedValue = AvailableValues.FirstOrDefault(v => Equals(v.Value, _initialValue));
        }

        private readonly TItem _initialValue;

        private bool _valuesFetched;
        private List<ListOptionItemViewModel<TItem>>? _availableValues;
    }

    public class ConnectionOptionViewModel : ListOptionBaseViewModel<ConnectionData>
    {
        public ConnectionOptionViewModel(string displayName, IEnumerable<ConnectionData> connections, ConnectionData value)
            : base(displayName, value)
        {
            ModelValues = connections.ToList();
        }

        protected override IEnumerable<ConnectionData> ModelValues { get; }
    }
}