using System;

namespace BuildNotifications.Plugin.Tfs.Configuration
{
    [Flags]
    internal enum ConfigurationFlags
    {
        None = 0,
        HideRepository = 0x1
    }
}