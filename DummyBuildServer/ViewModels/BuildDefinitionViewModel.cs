using DummyBuildServer.Models;

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

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}