using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.SourceControl;
using Newtonsoft.Json;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class SourceControlProvider : IBranchProvider
    {
        public SourceControlProvider(Connection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBranch> FetchExistingBranches()
        {
            var json = await _connection.Query(Constants.Queries.Branches);

            foreach (var branch in JsonConvert.DeserializeObject<List<Branch>>(json))
            {
                yield return branch;
            }
        }

        private readonly Connection _connection;
    }
}