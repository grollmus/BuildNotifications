using System.Collections.Generic;
using JetBrains.Annotations;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class BranchComparer : IEqualityComparer<Branch>
    {
        public bool Equals([CanBeNull] Branch x, [CanBeNull] Branch y)
        {
            return x?.Name.Equals(y?.Name) == true;
        }

        public int GetHashCode(Branch obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}