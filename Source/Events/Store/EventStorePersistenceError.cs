namespace Dolittle.Runtime.Events.Store
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an error when trying to save to the event store
    /// </summary>
    [Serializable]
    public class EventStorePersistenceError : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the EventStorePersistenceError custom exception
        /// </summary>
        public EventStorePersistenceError()
        {}

        /// <summary>
        ///     Initializes a new instance of the EventStorePersistenceError custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        public EventStorePersistenceError(string message)
            : base(message)
        {}

        /// <summary>
        ///     Initializes a new instance of the EventStorePersistenceError custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        /// <param name="innerException">An inner exception that is the original source of the error</param>
        public EventStorePersistenceError(string message, Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        ///     Initializes a new instance of the EventStorePersistenceError custom exception
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the object data of the exception</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        protected EventStorePersistenceError(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}