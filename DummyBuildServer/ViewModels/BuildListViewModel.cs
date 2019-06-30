using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DummyBuildServer.Models;
using JetBrains.Annotations;

namespace DummyBuildServer.ViewModels
{
    internal class BuildListViewModel : ViewModelBase
    {
        public BuildListViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            SelectedBranch = mainViewModel.Branches.Branches.FirstOrDefault();
            SelectedUser = mainViewModel.Users.Users.FirstOrDefault();
            SelectedDefinition = mainViewModel.BuildDefinitions.Definitions.FirstOrDefault();

            UpdateBuildCommand = new DelegateCommand(UpdateBuild, IsBuildSelected);
            EnqueueBuildCommand = new DelegateCommand(EnqueueBuild, IsBuildDataSelected);
        }

        public IEnumerable<BuildResult> AvailableBuildResults
        {
            get
            {
                yield return BuildResult.None;
                yield return BuildResult.Cancelled;
                yield return BuildResult.Warnings;
                yield return BuildResult.Errors;
                yield return BuildResult.Success;
            }
        }

        public IEnumerable<BuildStatus> AvailableBuildStatuses
        {
            get
            {
                yield return BuildStatus.Pending;
                yield return BuildStatus.Running;
                yield return BuildStatus.Finished;
            }
        }

        public int BuildProgress { get; set; }
        public ObservableCollection<BuildViewModel> Builds { get; } = new ObservableCollection<BuildViewModel>();
        public ICommand EnqueueBuildCommand { get; }
        public BranchViewModel SelectedBranch { get; set; }
        public BuildViewModel SelectedBuild { get; set; }
        public BuildResult SelectedBuildResult { get; set; }
        public BuildStatus SelectedBuildStatus { get; set; }
        public BuildDefinitionViewModel SelectedDefinition { get; set; }
        public UserViewModel SelectedUser { get; set; }
        public ICommand UpdateBuildCommand { get; }

        private void EnqueueBuild(object arg)
        {
            var user = SelectedUser.User;
            var branch = SelectedBranch.Branch;
            var definition = SelectedDefinition.Definition;

            var build = new Build(user, branch, definition);
            _mainViewModel.AddBuild(build);
            Builds.Add(new BuildViewModel(build));
        }

        private bool IsBuildDataSelected(object arg)
        {
            return SelectedBranch != null && SelectedDefinition != null && SelectedUser != null;
        }

        private bool IsBuildSelected(object arg)
        {
            return SelectedBuild != null;
        }

        [UsedImplicitly]
        private void OnSelectedBuildChanged()
        {
            BuildProgress = SelectedBuild.Progress;
        }

        private void UpdateBuild(object arg)
        {
            var build = SelectedBuild.Build;
            build.Status = SelectedBuildStatus;
            build.Result = SelectedBuildResult;
            build.Progress = BuildProgress;

            _mainViewModel.UpdateBuild(build);
        }

        private readonly MainViewModel _mainViewModel;
    }

    internal class BuildViewModel : ViewModelBase
    {
        public BuildViewModel(Build build)
        {
            Build = build;
        }

        public Build Build { get; }

        public int Progress { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Build.Id} - {Build.Definition.Name} on {Build.Branch.Name}";
        }
    }
}