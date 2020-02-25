using System.Collections.Generic;

namespace BuildNotifications.Plugin.Tfs.SourceControl
{
    internal class TfsBranchComparer : IEqualityComparer<TfsBranch>
    {
        public bool Equals(TfsBranch x, TfsBranch y)
        {
            return x?.Name.Equals(y?.Name) == true;
        }

        public int GetHashCode(TfsBranch obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}