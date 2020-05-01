namespace BuildNotifications.ViewModel.Utils
{
    /// <summary>
    /// Defines flags that can be used to influence remove behavior of collections.
    /// </summary>
    public enum RemoveFlag
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