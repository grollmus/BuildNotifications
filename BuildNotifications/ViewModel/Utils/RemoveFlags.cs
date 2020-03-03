using System;

namespace BuildNotifications.ViewModel.Utils
{
    /// <summary>
    /// Defines flags that can be used to influence remove behavior of collections.
    /// </summary>
    [Flags]
    public enum RemoveFlags
    {
        /// <summary>
        /// Defaults
        /// </summary>
        None = 0,

        /// <summary>
        /// Remove item immediately without delay or animations.
        /// </summary>
        Immediately = 0x1
    }
}