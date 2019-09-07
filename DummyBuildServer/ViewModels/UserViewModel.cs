using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.ViewModels
{
    internal class UserViewModel : ViewModelBase
    {
        public UserViewModel(User user)
        {
            User = user;
            Name = user.DisplayName;
        }

        public string Name { get; set; }
        public User User { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}