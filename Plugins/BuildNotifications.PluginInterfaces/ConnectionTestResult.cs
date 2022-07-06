using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces;

/// <summary>
/// Result of a connection test made by a plugin.
/// </summary>
[PublicAPI]
public sealed class ConnectionTestResult
{
    private ConnectionTestResult(bool success, string message)
        : this(success, new[] { message })
    {
    }

    private ConnectionTestResult(bool success, IEnumerable<string> messages)
    {
        IsSuccess = success;
        Errors = messages.ToList();
    }

    /// <summary>
    /// The combined errors of this result.
    /// </summary>
    public string ErrorMessage => string.Join(Environment.NewLine, Errors);

    /// <summary>
    /// Messages that should be displayed to the user.
    /// </summary>
    public IEnumerable<string> Errors { get; }

    /// <summary>
    /// Indicates whether the test was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// A successful test result.
    /// </summary>
    public static ConnectionTestResult Success { get; } = new(true, Enumerable.Empty<string>());

    /// <summary>
    /// Constructs a new test result with a failure message.
    /// </summary>
    /// <param name="message">The failure message.</param>
    /// <returns>The constructed result.</returns>
    public static ConnectionTestResult Failure(string message) => new(false, message);

    /// <summary>
    /// Constructs a new test result with multiple failure messages.
    /// </summary>
    /// <param name="messages">The failure messages.</param>
    /// <returns>The constructed result.</returns>
    public static ConnectionTestResult Failure(IEnumerable<string> messages) => new(false, messages);
}