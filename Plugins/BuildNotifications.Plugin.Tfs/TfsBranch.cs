using BuildNotifications.PluginInterfaces.SourceControl;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsBranch : IBranch
    {
        public TfsBranch(GitRef branch)
        {
            DisplayName = ExtractDisplayName(branch.Name);
            Name = branch.Name;
            _id = branch.ObjectId;
        }

        private string ExtractDisplayName(string branchName)
        {
            return branchName.Replace("refs/heads/", "");
        }

        /// <inheritdoc />
        public bool Equals(IBranch other)
        {
            return _id == (other as TfsBranch)?._id;
        }

        /// <inheritdoc />
        public string DisplayName { get; }

        /// <inheritdoc />
        public string Name { get; }

        private string _id;
    }
}