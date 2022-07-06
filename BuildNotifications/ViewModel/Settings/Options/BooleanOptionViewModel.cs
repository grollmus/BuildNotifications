namespace BuildNotifications.ViewModel.Settings.Options;

public class BooleanOptionViewModel : OptionViewModelBase<bool>
{
    public BooleanOptionViewModel(bool value, string displayName)
        : base(value, displayName)
    {
    }
}