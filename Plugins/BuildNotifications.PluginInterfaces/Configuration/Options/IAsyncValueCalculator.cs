using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options;

/// <summary>
/// Interface for asynchronous calculation of option values.
/// </summary>
[PublicAPI]
public interface IAsyncValueCalculator : IDisposable
{
    /// <summary>
    /// Marks the given options as affected by this calculation.
    /// Every time this calculator runs the <see cref="IOption.IsLoading" /> flag
    /// of all affected options will be set.
    /// </summary>
    /// <param name="options">Options that are affected by this calculator.</param>
    void Affect(params IOption[] options);

    /// <summary>
    /// Attaches this value calculator to an option.
    /// </summary>
    /// <remarks>Calculation of of value will start when the value of any attached option changed.</remarks>
    /// <param name="option">Option to attach to.</param>
    void Attach(params IValueOption[] option);

    /// <summary>
    /// Detaches this value calculator from a previously attached option.
    /// </summary>
    /// <param name="option"></param>
    void Detach(params IValueOption[] option);

    /// <summary>
    /// Removes the given options from this calculator's list of
    /// affected options.
    /// </summary>
    /// <param name="options">Options that are no longer affected by this calculator.</param>
    void RemoveAffect(params IOption[] options);

    /// <summary>
    /// Manually triggers an update.
    /// </summary>
    void Update();
}