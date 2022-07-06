using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline;

internal class NullBranchProvider : IBranchProvider
{
    public int ExistingBranchCount => 0;
    public IBranchNameExtractor NameExtractor => new NullBranchNameExtractor();

    public async IAsyncEnumerable<IBranch> FetchExistingBranches()
    {
        await Task.CompletedTask;
        yield break;
    }

    public async IAsyncEnumerable<IBranch> RemovedBranches()
    {
        await Task.CompletedTask;
        yield break;
    }
}