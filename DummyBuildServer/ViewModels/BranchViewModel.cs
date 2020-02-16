using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.ViewModels
{
    internal class BranchViewModel : ViewModelBase
    {
        public BranchViewModel(Branch branch)
        {
            Branch = branch;
            Name = branch.FullName;
        }

        public Branch Branch { get; }
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}