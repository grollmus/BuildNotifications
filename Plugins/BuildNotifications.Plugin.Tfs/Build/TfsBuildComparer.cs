using System.Collections.Generic;

namespace BuildNotifications.Plugin.Tfs.Build
{
    internal class TfsBuildComparer : IEqualityComparer<TfsBuild>
    {
        public bool Equals(TfsBuild x, TfsBuild y)
        {
            return x?.BuildId == y?.BuildId;
        }

        public int GetHashCode(TfsBuild obj)
        {
            return obj.BuildId;
        }
    }
}