using System.Collections.Generic;

namespace BuildNotifications.Plugin.Tfs.SourceControl
{
    internal class TfsBranchComparer : IEqualityComparer<TfsBranch>
    {
        public bool Equals(TfsBranch x, TfsBranch y)
        {
            return x?.FullName.Equals(y?.FullName) == true;
        }

        public int GetHashCode(TfsBranch obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}