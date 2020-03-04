using System.Collections.Generic;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class BranchComparer : IEqualityComparer<Branch>
    {
        public bool Equals(Branch x, Branch y)
        {
            return x?.FullName.Equals(y?.FullName) == true;
        }

        public int GetHashCode(Branch obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}