using System.Collections.Generic;

namespace BuildNotifications.Plugin.Tfs
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