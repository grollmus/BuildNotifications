using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.ViewModels
{
    internal class BuildDefinitionViewModel : ViewModelBase
    {
        public BuildDefinitionViewModel(BuildDefinition definition)
        {
            Definition = definition;
            Name = definition.Name;
        }

        public BuildDefinition Definition { get; }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}