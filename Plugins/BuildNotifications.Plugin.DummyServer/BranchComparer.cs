using System.Collections.Generic;

namespace BuildNotifications.Plugin.DummyServer;

internal class BranchComparer : IEqualityComparer<Branch>
{
    public bool Equals(Branch x, Branch y) => x?.FullName.Equals(y?.FullName) == true;

    public int GetHashCode(Branch obj) => obj.FullName.GetHashCode();
}