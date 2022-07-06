using System.Collections.Generic;

namespace BuildNotifications.Plugin.DummyServer;

internal class BuildComparer : IEqualityComparer<Build>, IEqualityComparer<BuildDefinition>
{
    public bool Equals(Build x, Build y) => x?.Id.Equals(y?.Id) == true;

    public int GetHashCode(Build obj) => obj.Id.GetHashCode();

    public bool Equals(BuildDefinition x, BuildDefinition y) => x?.Id.Equals(y?.Id) == true;

    public int GetHashCode(BuildDefinition obj) => obj.Id.GetHashCode();
}