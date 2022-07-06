using BuildNotifications.PluginInterfaces;

namespace BuildNotifications.TestMocks;

public class MockUser : IUser
{
    public MockUser()
    {
        Id = string.Empty;
        DisplayName = string.Empty;
        UniqueName = string.Empty;
    }

    public MockUser(string id, string displayName, string uniqueName)
    {
        Id = id;
        DisplayName = displayName;
        UniqueName = uniqueName;
    }

    public string DisplayName { get; }
    public string Id { get; }
    public string UniqueName { get; }
}