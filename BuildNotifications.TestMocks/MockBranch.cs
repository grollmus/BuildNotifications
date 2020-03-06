using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.TestMocks
{
    public class MockBranch : IBranch
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
        public bool IsPullRequest => false;
    }
}