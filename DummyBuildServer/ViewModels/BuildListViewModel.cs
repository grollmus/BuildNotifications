using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Plugin.DummyBuildServer;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;

namespace DummyBuildServer.ViewModels
{
    internal class BuildListViewModel : ViewModelBase
    {
        public BuildListViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            _selectedBranch = mainViewModel.Branches.Branches.FirstOrDefault();
            _selectedUser = mainViewModel.Users.Users.FirstOrDefault();
            _selectedDefinition = mainViewModel.BuildDefinitions.Definitions.FirstOrDefault();

            UpdateBuildCommand = new DelegateCommand(UpdateBuild, IsBuildSelected);
            EnqueueBuildCommand = new DelegateCommand(EnqueueBuild, IsBuildDataSelected);
            RemoveBuildCommand = new DelegateCommand(RemoveBuild, IsBuildSelected);
            RandomizeStatusOfAllBuildsCommand = new DelegateCommand(RandomizeStatusOfAllBuilds);
        }

        public IEnumerable<BuildStatus> AvailableBuildStatuses
        {
            get
            {
                yield return BuildStatus.Pending;
                yield return BuildStatus.Running;
                yield return BuildStatus.Cancelled;
                yield return BuildStatus.Succeeded;
                yield return BuildStatus.PartiallySucceeded;
                yield return BuildStatus.Failed;
            }
        }

        public int BuildProgress
        {
            get => _buildProgress;
            set
            {
                if (value == _buildProgress)
                    return;
                _buildProgress = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BuildViewModel> Builds { get; } = new ObservableCollection<BuildViewModel>();
        public ICommand EnqueueBuildCommand { get; }
        public ICommand RandomizeStatusOfAllBuildsCommand { get; set; }
        public ICommand RemoveBuildCommand { get; }

        public BranchViewModel? SelectedBranch
        {
            get => _selectedBranch;
            set
            {
                if (Equals(value, _selectedBranch))
                    return;
                _selectedBranch = value;
                OnPropertyChanged();
            }
        }

        public BuildStatus SelectedBuildStatus
        {
            get => _selectedBuildStatus;
            set
            {
                if (value == _selectedBuildStatus)
                    return;
                _selectedBuildStatus = value;
                OnPropertyChanged();
            }
        }

        public BuildDefinitionViewModel? SelectedDefinition
        {
            get => _selectedDefinition;
            set
            {
                if (Equals(value, _selectedDefinition))
                    return;
                _selectedDefinition = value;
                OnPropertyChanged();
            }
        }

        public UserViewModel? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (Equals(value, _selectedUser))
                    return;
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateBuildCommand { get; }

        public void EnqueueSpecificBuild(BuildDefinition definition, Branch branch)
        {
            var user = SelectedUser.User;

            var build = new Build
            {
                LastChangedTime = DateTime.Now,
                QueueTime = DateTime.Now,
                Definition = definition,
                BranchName = branch.FullName,
                RequestedBy = RandomUser(),
                RequestedFor = user,
                Status = BuildStatus.Pending,
                Id = (++_idCounter).ToString()
            };

            _mainViewModel.AddBuild(build);
            var buildViewModel = new BuildViewModel(build);
            buildViewModel.PropertyChanged += BuildViewModel_OnPropertyChanged;
            Builds.Add(buildViewModel);
        }

        private void BuildViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(BuildViewModel.IsSelected))
                return;

            OnSelectedBuildChanged();
        }

        private void EnqueueBuild(object arg)
        {
            EnqueueSpecificBuild(SelectedDefinition.Definition, SelectedBranch.Branch);
        }

        private bool IsBuildDataSelected(object arg)
        {
            return SelectedBranch != null && SelectedDefinition != null && SelectedUser != null;
        }

        private bool IsBuildSelected(object arg)
        {
            return SelectedBuilds().Any();
        }

        private void OnSelectedBuildChanged()
        {
            var firstBuild = SelectedBuilds().FirstOrDefault();

            if (firstBuild == null)
                return;

            BuildProgress = firstBuild!.Progress;
            SelectedBuildStatus = firstBuild.Build.Status;
        }

        private void RandomizeStatusOfAllBuilds(object obj)
        {
            var rnd = new Random();
            foreach (var build in Builds)
            {
                UpdateSpecificBuild(build.Build, (BuildStatus) rnd.Next(1, 7));
            }
        }

        private IUser RandomUser()
        {
            var asList = _mainViewModel.Users.Users.ToList();
            var index = new Random().Next(asList.Count);
            return asList[index].User;
        }

        private void RemoveBuild(object obj)
        {
            foreach (var selectedBuild in SelectedBuilds())
            {
                _mainViewModel.RemoveBuild(selectedBuild!.Build);
                selectedBuild.PropertyChanged -= BuildViewModel_OnPropertyChanged;
                Builds.Remove(selectedBuild);
            }
        }

        private IEnumerable<BuildViewModel> SelectedBuilds()
        {
            return Builds.Where(x => x.IsSelected).ToList();
        }

        private void UpdateBuild(object arg)
        {
            foreach (var selectedBuild in SelectedBuilds())
            {
                UpdateSpecificBuild(selectedBuild.Build, SelectedBuildStatus);
            }
        }

        private void UpdateSpecificBuild(Build build, BuildStatus toStatus)
        {
            build.Status = toStatus;
            build.Progress = BuildProgress;
            build.LastChangedTime = DateTime.Now;

            _mainViewModel.UpdateBuild(build);
        }

        private readonly MainViewModel _mainViewModel;
        private UserViewModel? _selectedUser;
        private BuildDefinitionViewModel? _selectedDefinition;
        private BuildStatus _selectedBuildStatus;
        private BranchViewModel? _selectedBranch;
        private int _buildProgress;

        private static int _idCounter;
    }
}