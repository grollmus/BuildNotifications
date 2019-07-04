using System;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class Build : IBuild
    {
        public Build()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool Equals(IBuild other)
        {
            return Id == (other as Build)?.Id;
        }

        /// <inheritdoc />
        public string BranchName { get; }

        /// <inheritdoc />
        public IBuildDefinition Definition { get; }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public DateTime? LastChangedTime { get; }

        /// <inheritdoc />
        public DateTime? QueueTime { get; }

        /// <inheritdoc />
        public IUser RequestedBy { get; }

        /// <inheritdoc />
        public IUser RequestedFor { get; }

        /// <inheritdoc />
        public BuildStatus Status { get; }
    }
}