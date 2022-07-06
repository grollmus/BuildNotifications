using System.Threading;
using System.Threading.Tasks;

namespace BuildNotifications.Services;

internal interface IAppUpdater
{
    Task<UpdateCheckResult?> CheckForUpdates(CancellationToken cancellationToken = default);
    Task PerformUpdate(CancellationToken cancellationToken = default);
}