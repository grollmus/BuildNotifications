using System;
using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.SourceControl;
using Microsoft.VisualStudio.Services.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsSourceControlProvider : IBranchProvider
    {
        public TfsSourceControlProvider(VssConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IBranch> FetchExistingBranches()
        {
            throw new NotImplementedException();
        }

        private readonly VssConnection _connection;
    }
}