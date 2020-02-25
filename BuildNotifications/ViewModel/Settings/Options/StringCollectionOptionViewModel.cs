using System.Collections.Generic;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public class StringCollectionOptionViewModel : CollectionOptionBaseViewModel<string?>
    {
        public StringCollectionOptionViewModel(string displayName, IEnumerable<string> value)
            : base(value, displayName)
        {
        }
    }
}