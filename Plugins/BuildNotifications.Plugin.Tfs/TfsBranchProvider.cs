using System;
using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsBranchProvider : IBranchProvider
    {
        /// <inheritdoc />
        public IAsyncEnumerable<IBranch> FetchExistingBranches()
        {
            throw new NotImplementedException();
        }
    }
}