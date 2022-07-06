using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces;

namespace BuildNotifications.ViewModel.Settings;

public class UserViewModel
{
    public UserViewModel(IUser user)
    {
        User = user;
    }

    public string Description => string.Format(StringLocalizer.CurrentCulture, StringLocalizer.UserDescription, User.DisplayName, User.Id, User.UniqueName);

    public string Name => User.DisplayName;
    public IUser User { get; }
}