namespace BuildNotifications.ViewModel.Settings.Options
{
    public class NumberOptionViewModel : OptionViewModelBase<int>
    {
        public NumberOptionViewModel(int value, int minValue, int maxValue, string displayName)
            : base(value, displayName)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public int MaxValue { get; }
        public int MinValue { get; }
    }
}