using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.ViewModels
{
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
            return $"{Build.Id} - {Build.Definition.Name} on {Build.BranchName}";
        }
    }
}