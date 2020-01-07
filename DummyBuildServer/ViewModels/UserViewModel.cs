using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.ViewModels
{
    internal class UserViewModel : ViewModelBase
    {
        public UserViewModel(User user)
        {
            User = user;
            _name = user.DisplayName;
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name)
                    return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public User User { get; }

        public override string ToString()
        {
            return Name;
        }

        private string _name;
    }
}