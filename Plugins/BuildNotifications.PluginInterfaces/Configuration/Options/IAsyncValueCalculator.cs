using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <summary>
    /// Interface for asynchronous calculation of option values.
    /// </summary>
    [PublicAPI]
    public interface IAsyncValueCalculator : IDisposable
    {
        /// <summary>
        /// Attaches this value calculator to an option.
        /// </summary>
        /// <remarks>Calculation of of value will start when the value of any attached option changed.</remarks>
        /// <param name="option">Option to attach to.</param>
        void Attach(IValueOption option);

        /// <summary>
        /// Detaches this value calculator from a previously attached option.
        /// </summary>
        /// <param name="option"></param>
        void Detach(IValueOption option);

        /// <summary>
        /// Manually triggers an update.
        /// </summary>
        void Update();
    }
}