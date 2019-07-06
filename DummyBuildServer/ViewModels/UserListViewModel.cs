using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.ViewModels
{
    internal class UserListViewModel : ViewModelBase
    {
        public UserListViewModel(MainViewModel mainViewModel, IEnumerable<User> users)
        {
            _mainViewModel = mainViewModel;
            foreach (var user in users)
            {
                Users.Add(new UserViewModel(user));
            }

            AddUserCommand = new DelegateCommand(AddUser, IsUserNameFilled);
            RemoveUserCommand = new DelegateCommand(RemoveUser, IsUserSelected);
        }

        public ICommand AddUserCommand { get; }
        public ICommand RemoveUserCommand { get; }

        public UserViewModel? SelectedUser { get; set; }
        public string? UserName { get; set; }
        public ObservableCollection<UserViewModel> Users { get; } = new ObservableCollection<UserViewModel>();

        private void AddUser(object obj)
        {
            var user = new User(UserName!);
            _mainViewModel.AddUser(user);
            Users.Add(new UserViewModel(user));

            SelectedUser = null;
            UserName = string.Empty;
        }

        private bool IsUserNameFilled(object arg)
        {
            return !string.IsNullOrWhiteSpace(UserName);
        }

        private bool IsUserSelected(object arg)
        {
            return SelectedUser != null;
        }

        private void RemoveUser(object obj)
        {
            _mainViewModel.RemoveUser(SelectedUser!.User);
            Users.Remove(SelectedUser!);

            SelectedUser = null;
            UserName = string.Empty;
        }

        private readonly MainViewModel _mainViewModel;
    }
}