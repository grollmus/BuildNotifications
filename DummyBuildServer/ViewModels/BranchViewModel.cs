using DummyBuildServer.Models;

namespace DummyBuildServer.ViewModels
{
    internal class BranchViewModel : ViewModelBase
    {
        public BranchViewModel(Branch branch)
        {
            Branch = branch;
            Name = branch.Name;
        }

        public Branch Branch { get; }
        public string Name { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}