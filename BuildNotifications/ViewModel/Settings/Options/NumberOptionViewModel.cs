namespace BuildNotifications.ViewModel.Settings.Options
{
    public class NumberOptionViewModel : OptionViewModelBase<int>
    {
        public NumberOptionViewModel(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}