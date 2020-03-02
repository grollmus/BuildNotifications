namespace BuildNotifications.ViewModel.Settings.Options
{
    public class CollectionItem<TValue> : BaseViewModel
    {
        public CollectionItem(TValue value, string? displayName = null)
        {
            Value = value;
            DisplayName = displayName ?? value?.ToString() ?? string.Empty;
        }

        public string DisplayName { get; set; }

        public TValue Value { get; set; }
    }
}