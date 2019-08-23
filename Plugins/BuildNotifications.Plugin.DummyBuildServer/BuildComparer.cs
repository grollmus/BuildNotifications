using System.Collections.Generic;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class BuildComparer : IEqualityComparer<Build>, IEqualityComparer<BuildDefinition>
    {
        public bool Equals(Build x, Build y)
        {
            return x?.Id.Equals(y?.Id) == true;
        }

        public int GetHashCode(Build obj)
        {
            return obj.Id.GetHashCode();
        }

        public bool Equals(BuildDefinition x, BuildDefinition y)
        {
            return x?.Id.Equals(y?.Id) == true;
        }

        public int GetHashCode(BuildDefinition obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}