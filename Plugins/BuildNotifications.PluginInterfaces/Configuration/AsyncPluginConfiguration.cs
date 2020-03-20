using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.PluginInterfaces.Host;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration
{
    /// <summary>
    /// Generic implementation for configurations, which require functionality to fetch values asynchronous.
    /// </summary>
    [PublicAPI]
    public abstract class AsyncPluginConfiguration : IPluginConfiguration, IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="uiDispatcher"></param>
        protected AsyncPluginConfiguration(IDispatcher uiDispatcher)
        {
            _uiDispatcher = uiDispatcher;
        }

        /// <summary>
        /// Creates a new calculator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="calculationTaskFactory"></param>
        /// <param name="handleResultCallback"></param>
        /// <returns>The created calculator.</returns>
        protected IAsyncValueCalculator CreateCalculator<T>(Func<CancellationToken, Task<IValueCalculationResult<T>>> calculationTaskFactory, Action<T> handleResultCallback)
        {
            var asyncValueCalculator = new AsyncValueCalculator<T>(_uiDispatcher, calculationTaskFactory, handleResultCallback);
            _calculators.Add(asyncValueCalculator);
            return asyncValueCalculator;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var asyncValueCalculator in _calculators)
            {
                asyncValueCalculator.Dispose();
            }
        }

        /// <inheritdoc />
        public abstract ILocalizer Localizer { get; }

        /// <inheritdoc />
        public abstract bool Deserialize(string serialized);

        /// <inheritdoc />
        public abstract IEnumerable<IOption> ListAvailableOptions();

        /// <inheritdoc />
        public abstract string Serialize();

        private readonly IDispatcher _uiDispatcher;

        private readonly List<IAsyncValueCalculator> _calculators = new List<IAsyncValueCalculator>();
    }
}