namespace Dolittle.Runtime.Events.Store
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the exceptional situation when we cannot communicate with the intended <see cref="IEventStore"/>
    /// </summary>
    [Serializable]
    public class EventStoreUnavailable : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the EventStoreUnavailable custom exception
        /// </summary>
        public EventStoreUnavailable()
        {}
    
        /// <summary>
        ///     Initializes a new instance of the EventStoreUnavailable custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        public EventStoreUnavailable(string message)
            : base(message)
        {}
    
        /// <summary>
        ///     Initializes a new instance of the EventStoreUnavailable custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        /// <param name="innerException">An inner exception that is the original source of the error</param>
        public EventStoreUnavailable(string message, Exception innerException)
            : base(message, innerException)
        {}
    
        /// <summary>
        ///     Initializes a new instance of the EventStoreUnavailable custom exception
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the object data of the exception</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        protected EventStoreUnavailable(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}