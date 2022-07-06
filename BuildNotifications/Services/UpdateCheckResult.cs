using JetBrains.Annotations;

namespace BuildNotifications.Services;

[UsedImplicitly]
internal class UpdateCheckResult
{
    public string? CurrentVersion { get; set; }
    public string? FutureVersion { get; set; }
    public ReleaseToApply[]? ReleasesToApply { get; set; }
}