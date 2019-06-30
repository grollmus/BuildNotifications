using DummyBuildServer.Models;

namespace DummyBuildServer.ViewModels
{
    internal class UserViewModel : ViewModelBase
    {
        public User User { get; }

        public UserViewModel(User user)
        {
            User = user;
            Name = user.UserName;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }

        public string Name { get; set; }
    }
}