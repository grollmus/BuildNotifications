namespace BuildNotifications.Core.Utilities
{
    /// <summary>
    /// Provides serialization functionality.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes an object from its string representation.
        /// </summary>
        /// <typeparam name="T">Type to deserialize.</typeparam>
        /// <param name="serialized">String representation to deserialize</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize<T>(string serialized);

        /// <summary>
        /// Serializes an object into a string representation.
        /// </summary>
        /// <param name="value">Object to serialize.</param>
        /// <returns>String representing the object.</returns>
        string Serialize(object value);
    }
}