using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public abstract class CollectionOptionBaseViewModel<TValue> : OptionViewModelBase<ObservableCollection<TValue>>
    {
        protected CollectionOptionBaseViewModel(IEnumerable<TValue> value, string displayName)
            : base(new ObservableCollection<TValue>(value), displayName)
        {
        }
    }
}