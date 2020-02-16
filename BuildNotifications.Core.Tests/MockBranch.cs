using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Tests
{
    internal class MockBranch : IBranch
    {
        public MockBranch(string name)
        {
            FullName = DisplayName = name;
        }

        public bool Equals(IBranch other)
        {
            var mock = other as MockBranch;
            return mock?.FullName.Equals(FullName) == true;
        }

        public string DisplayName { get; }
        public string FullName { get; }
    }
}