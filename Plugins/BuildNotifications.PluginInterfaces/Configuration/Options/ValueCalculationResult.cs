namespace BuildNotifications.PluginInterfaces.Configuration.Options
{
    /// <inheritdoc />
    public class ValueCalculationResult<T> : IAsyncValueCalculationResult<T>
    {
        internal ValueCalculationResult(T value, bool success)
        {
            Value = value;
            Success = success;
        }

        /// <inheritdoc />
        public bool Success { get; }

        /// <inheritdoc />
        public T Value { get; }
    }

    /// <summary>
    /// Result of a asynchrous value calculation.
    /// </summary>
    public class ValueCalculationResult
    {
        /// <summary>
        /// Creates a result for a value calculation that indicates a fail.
        /// </summary>
        /// <typeparam name="T">Type of value that is contained in this result.</typeparam>
        /// <returns>The created result.</returns>
        public static ValueCalculationResult<T> Fail<T>()
        {
            return new ValueCalculationResult<T>(default!, false);
        }

        /// <summary>
        /// Creates a successful result for a value calculation.
        /// </summary>
        /// <typeparam name="T">Type of value that is contained in this result.</typeparam>
        /// <param name="value">value </param>
        /// <returns>The created result.</returns>
        public static ValueCalculationResult<T> Success<T>(T value)
        {
            return new ValueCalculationResult<T>(value, true);
        }
    }
}