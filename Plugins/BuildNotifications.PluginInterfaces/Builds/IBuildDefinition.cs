using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds;

/// <summary>
/// Contains information about a build definition.
/// </summary>
[PublicAPI]
public interface IBuildDefinition : IEquatable<IBuildDefinition>
{
    /// <summary>
    /// Unique id of this definition.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Name of this definition.
    /// </summary>
    string Name { get; }
}