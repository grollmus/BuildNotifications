using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.ViewModels
{
    internal class BranchListViewModel : ViewModelBase
    {
        public BranchListViewModel(MainViewModel mainViewModel, IEnumerable<Branch> branches)
        {
            _mainViewModel = mainViewModel;

            foreach (var branch in branches)
            {
                Branches.Add(new BranchViewModel(branch));
            }

            AddBranchCommand = new DelegateCommand(AddBranch, IsBranchNameFilled);
            RemoveBranchCommand = new DelegateCommand(RemoveBranch, IsBranchSelected);
        }

        public ICommand AddBranchCommand { get; }
        public ObservableCollection<BranchViewModel> Branches { get; } = new ObservableCollection<BranchViewModel>();
        public string? BranchName { get; set; }
        public ICommand RemoveBranchCommand { get; }
        public BranchViewModel? SelectedBranch { get; set; }

        private void AddBranch(object arg)
        {
            var branch = new Branch(BranchName!);
            _mainViewModel.AddBranch(branch);

            Branches.Add(new BranchViewModel(branch));

            BranchName = string.Empty;
            SelectedBranch = null;
        }

        private bool IsBranchNameFilled(object arg)
        {
            return !string.IsNullOrWhiteSpace(BranchName);
        }

        private bool IsBranchSelected(object arg)
        {
            return SelectedBranch != null;
        }

        private void RemoveBranch(object arg)
        {
            _mainViewModel.RemoveBranch(SelectedBranch!.Branch);
            Branches.Remove(SelectedBranch);

            BranchName = string.Empty;
            SelectedBranch = null;
        }

        private readonly MainViewModel _mainViewModel;
    }
}