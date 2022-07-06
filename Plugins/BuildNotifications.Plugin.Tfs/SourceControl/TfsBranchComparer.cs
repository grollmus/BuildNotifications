using System;
using System.Collections.Generic;

namespace BuildNotifications.Plugin.Tfs.SourceControl;

internal class TfsBranchComparer : IEqualityComparer<TfsBranch>
{
    public bool Equals(TfsBranch? x, TfsBranch? y) => x?.FullName.Equals(y?.FullName, StringComparison.InvariantCulture) == true;

    public int GetHashCode(TfsBranch obj) => obj.FullName.GetHashCode(StringComparison.InvariantCulture);
}