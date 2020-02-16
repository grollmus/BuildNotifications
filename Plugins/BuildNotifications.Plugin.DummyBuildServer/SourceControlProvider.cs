using System.Collections.Generic;
using System.Linq;
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

        public IBranchNameExtractor NameExtractor => new BranchNameExtractor();

        public async IAsyncEnumerable<IBranch> FetchExistingBranches()
        {
            var json = await _connection.Query(Constants.Queries.Branches);
            var list = JsonConvert.DeserializeObject<List<Branch>>(json);

            foreach (var branch in list)
            {
                _knownBranches.Add(branch);

                yield return branch;
            }
        }

        public async IAsyncEnumerable<IBranch> RemovedBranches()
        {
            var json = await _connection.Query(Constants.Queries.Branches);
            var list = JsonConvert.DeserializeObject<List<Branch>>(json);

            var deletedBranches = _knownBranches.Except(list, new BranchComparer());

            foreach (var branch in deletedBranches)
            {
                yield return branch;
            }
        }

        private readonly HashSet<Branch> _knownBranches = new HashSet<Branch>(new BranchComparer());
        private readonly Connection _connection;
    }
}