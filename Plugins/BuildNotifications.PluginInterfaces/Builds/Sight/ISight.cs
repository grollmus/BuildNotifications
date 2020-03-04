namespace BuildNotifications.PluginInterfaces.Builds.Sight
{
    /// <summary>
    /// Describes a way too filter or highlight builds.
    /// </summary>
    public interface ISight
    {
        /// <summary>
        /// Whether this sight is active or not.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Whether build shall be highlighted by this sight.
        /// </summary>
        bool IsHighlighted(IBuild build);
        
        /// <summary>
        /// Whether build shall be shown by this sight.
        /// </summary>
        bool IsBuildShown(IBuild build);
    }
}
