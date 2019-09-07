using System.Collections.Generic;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class BranchComparer : IEqualityComparer<Branch>
    {
        public bool Equals(Branch x, Branch y)
        {
            return x?.Name.Equals(y?.Name) == true;
        }

        public int GetHashCode(Branch obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}