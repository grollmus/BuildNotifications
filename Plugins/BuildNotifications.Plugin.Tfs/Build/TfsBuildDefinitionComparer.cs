using System.Collections.Generic;

namespace BuildNotifications.Plugin.Tfs.Build
{
    internal class TfsBuildDefinitionComparer : IEqualityComparer<TfsBuildDefinition>
    {
        public bool Equals(TfsBuildDefinition? x, TfsBuildDefinition? y) => x?.NativeId == y?.NativeId;

        public int GetHashCode(TfsBuildDefinition obj)
        {
            return obj.NativeId;
        }
    }
}