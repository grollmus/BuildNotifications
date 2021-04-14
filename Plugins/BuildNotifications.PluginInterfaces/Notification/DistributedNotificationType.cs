namespace BuildNotifications.PluginInterfaces.Notification
{
    /// <summary>
    /// Available notification types that can be distributed
    /// </summary>
    public enum DistributedNotificationType
    {
        /// <summary>
        /// Notification regarding a branch
        /// </summary>
        Branch,

        /// <summary>
        /// Notification regarding a build definition
        /// </summary>
        Definition,

        /// <summary>
        /// Notification regarding a single build
        /// </summary>
        Build,

        /// <summary>
        /// Notification regarding mutliple builds
        /// </summary>
        Builds,

        /// <summary>
        /// Error
        /// </summary>
        GeneralError,

        /// <summary>
        /// Notification regarding a build definition and a branch
        /// </summary>
        DefinitionAndBranch,

        /// <summary>
        /// General
        /// </summary>
        General
    }
}