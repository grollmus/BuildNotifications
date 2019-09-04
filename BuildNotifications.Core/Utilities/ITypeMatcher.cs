using System;

namespace BuildNotifications.Core.Utilities
{
    /// <summary>
    /// Utility for search for types.
    /// </summary>
    internal interface ITypeMatcher
    {
        /// <summary>
        /// Determine whether <paramref name="type" /> is matched by <paramref name="typeName" />.
        /// </summary>
        /// <remarks>
        /// A type is considered matching if the name of the type, its namespace,
        /// and assembly (if specified in <paramref name="typeName" />) are matching.
        /// When an assembly name is given the version and public key token is ignored.
        /// </remarks>
        /// <param name="type">Type to check for match.</param>
        /// <param name="typeName">Type name to use for checking.</param>
        /// <returns><c>true</c> if the types match; otherwise <c>false</c>.</returns>
        bool MatchesType(Type type, string? typeName);
    }
}