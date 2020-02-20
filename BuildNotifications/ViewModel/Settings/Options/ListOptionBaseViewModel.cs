using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public abstract class ListOptionBaseViewModel<TItem> : OptionViewModelBase<TItem>
    {
        protected ListOptionBaseViewModel(string displayName, TItem value = default)
            : base(value, displayName)
        {
            AvailableValues = new List<ListOptionItemViewModel<TItem>>(
                // ReSharper disable once VirtualMemberCallInConstructor
                ModelValues.Select(v => new ListOptionItemViewModel<TItem>(v, v?.ToString()))
            );
            SelectedValue = AvailableValues.FirstOrDefault(v => Equals(v.Value, value));
        }

        public IEnumerable<ListOptionItemViewModel<TItem>> AvailableValues { get; }
        public ListOptionItemViewModel<TItem> SelectedValue { get; set; }

        protected abstract IEnumerable<TItem> ModelValues { get; }
    }
}