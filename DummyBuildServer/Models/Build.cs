using System;

namespace DummyBuildServer.Models
{
    internal class Build : IEquatable<Build>
    {
        public Build(User requestedBy, Branch branch, BuildDefinition definition)
        {
            RequestedBy = requestedBy;
            Branch = branch;
            Definition = definition;

            Id = ++_idCounter;
        }

        public Branch Branch { get; set; }
        public BuildDefinition Definition { get; set; }
        public int Id { get; }
        public int Progress { get; set; }
        public User RequestedBy { get; set; }
        public BuildResult Result { get; set; }
        public BuildStatus Status { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Build) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(Build left, Build right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Build left, Build right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public bool Equals(Build other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id == other.Id;
        }

        private static int _idCounter;
    }

    internal enum BuildResult
    {
        None,
        Success,
        Cancelled,
        Warnings,
        Errors
    }

    internal enum BuildStatus
    {
        Pending,
        Running,
        Finished
    }
}