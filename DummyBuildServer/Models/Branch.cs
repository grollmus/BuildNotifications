namespace DummyBuildServer.Models
{
    internal class Branch
    {
        public Branch(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}