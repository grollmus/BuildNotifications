namespace BuildNotifications.PluginInterfaces.SourceControl
{
    /// <summary>
    /// Extracts parts of branch names from full names.
    /// </summary>
    public interface IBranchNameExtractor
    {
        /// <summary>
        /// Extract name from branch that can be used for display in the UI.
        /// </summary>
        /// <param name="fullBranchName">Full name of the branch.</param>
        /// <returns>A name that can be used to display the branch in the UI.</returns>
        string ExtractDisplayName(string fullBranchName);
    }
}