using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Tests
{
    internal class MockBranch : IBranch
    {
        public MockBranch(string name)
        {
            Name = DisplayName = name;
        }

        public bool Equals(IBranch other)
        {
            var mock = other as MockBranch;
            return mock?.Name.Equals(Name) == true;
        }

        public string DisplayName { get; }
        public string Name { get; }
    }
}