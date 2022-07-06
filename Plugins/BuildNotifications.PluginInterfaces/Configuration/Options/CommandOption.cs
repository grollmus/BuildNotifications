using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options;

/// <summary>
/// Base class for a command option.
/// </summary>
[PublicAPI]
public class CommandOption : Option, ICommandOption
{
    /// <summary>Constructor.</summary>
    /// <param name="nameTextId">Text id used for localizing the name of this option.</param>
    /// <param name="descriptionTextId">Text id used for localizing the description of this option.</param>
    /// <param name="action">Action to execute using this command.</param>
    /// <param name="canExecute">Predicate to determine if this command can be executed.</param>
    public CommandOption(Func<Task> action, Func<bool> canExecute, string nameTextId, string descriptionTextId)
        : base(nameTextId, descriptionTextId)
    {
        _action = action;
        _predicate = canExecute;
    }

    /// <summary>Constructor.</summary>
    /// <param name="nameTextId">Text id used for localizing the name of this option.</param>
    /// <param name="descriptionTextId">Text id used for localizing the description of this option.</param>
    /// <param name="action">Action to execute using this command.</param>
    public CommandOption(Func<Task> action, string nameTextId, string descriptionTextId)
        : this(action, () => true, nameTextId, descriptionTextId)
    {
    }

    /// <inheritdoc />
    public bool CanExecute() => _predicate();

    /// <inheritdoc />
    public async Task Execute()
    {
        IsLoading = true;
        try
        {
            if (CanExecute())
                await _action();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private readonly Func<Task> _action;
    private readonly Func<bool> _predicate;
}