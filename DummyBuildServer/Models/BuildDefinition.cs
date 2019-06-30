namespace DummyBuildServer.Models
{
    internal class BuildDefinition
    {
        public BuildDefinition(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}