using JetBrains.Annotations;

namespace BuildNotifications.Services
{
    [UsedImplicitly]
    internal class ReleaseToApply
    {
        public string? ReleaseNotes { get; set; }
        public string? Version { get; set; }
    }
}