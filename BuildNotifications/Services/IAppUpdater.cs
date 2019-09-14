using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace BuildNotifications.Services
{
    internal interface IAppUpdater
    {
        Task<UpdateCheckResult?> CheckForUpdates(CancellationToken cancellationToken = default);
        Task PerformUpdate(CancellationToken cancellationToken = default);
    }

    [UsedImplicitly]
    internal class UpdateCheckResult
    {
        public string? CurrentVersion { get; set; }
        public string? FutureVersion { get; set; }
        public ReleaseToApply[]? ReleasesToApply { get; set; }
    }

    [UsedImplicitly]
    internal class ReleaseToApply
    {
        public string? ReleaseNotes { get; set; }
        public string? Version { get; set; }
    }
}