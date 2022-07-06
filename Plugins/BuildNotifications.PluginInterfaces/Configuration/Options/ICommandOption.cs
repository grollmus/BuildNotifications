using System.Threading.Tasks;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options;

/// <summary>
/// A configuration option that exposes a command.
/// </summary>
[PublicAPI]
public interface ICommandOption : IOption
{
    /// <summary>
    /// Determine whether this command can currently be executed.
    /// </summary>
    /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c>.</returns>
    bool CanExecute();

    /// <summary>
    /// Executes the associated command.
    /// </summary>
    Task Execute();
}