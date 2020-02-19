using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
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
        /// <param name="predicate">Predicate to determine if this command can be executed.</param>
        public CommandOption(Action action, Func<bool> predicate, string nameTextId, string descriptionTextId) : base(nameTextId, descriptionTextId)
        {
            _action = action;
            _predicate = predicate;
        }

        /// <summary>Constructor.</summary>
        /// <param name="nameTextId">Text id used for localizing the name of this option.</param>
        /// <param name="descriptionTextId">Text id used for localizing the description of this option.</param>
        /// <param name="action">Action to execute using this command.</param>
        public CommandOption(Action action, string nameTextId, string descriptionTextId)
            : this(action, () => true, nameTextId, descriptionTextId)
        {
        }

        /// <inheritdoc />
        public bool CanExecute()
        {
            return _predicate();
        }

        /// <inheritdoc />
        public void Execute()
        {
            IsLoading = true;
            try
            {
                if (CanExecute())
                    _action();
            }
            finally
            {
                IsLoading = false;
            }
        }

        private readonly Action _action;
        private readonly Func<bool> _predicate;
    }
}