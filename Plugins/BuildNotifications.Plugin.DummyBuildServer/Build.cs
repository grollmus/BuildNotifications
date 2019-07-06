using System;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class Build : IBuild
    {
        /// <inheritdoc />
        public bool Equals(IBuild other)
        {
            return Id == (other as Build)?.Id;
        }

        /// <inheritdoc />
        public string BranchName { get; set; }

        /// <inheritdoc />
        public IBuildDefinition Definition { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public DateTime? LastChangedTime { get; set; }

        /// <inheritdoc />
        public int Progress { get; set; }

        /// <inheritdoc />
        public DateTime? QueueTime { get; set; }

        /// <inheritdoc />
        public IUser RequestedBy { get; set; }

        /// <inheritdoc />
        public IUser RequestedFor { get; set; }

        /// <inheritdoc />
        public BuildStatus Status { get; set; }
    }
}