using System;
using System.Runtime.Serialization;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents an instance of a concurrency conflict in an Event Stream for a specific <see cref="IEventSource" />
    /// </summary>
    [Serializable]
    public class EventSourceConcurrencyConflict : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the EventSourceConcurrencyConflict custom exception
        /// </summary>
        public EventSourceConcurrencyConflict()
        {}
    
        /// <summary>
        ///     Initializes a new instance of the EventSourceConcurrencyConflict custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        public EventSourceConcurrencyConflict(string message)
            : base(message)
        {}
    
        /// <summary>
        ///     Initializes a new instance of the EventSourceConcurrencyConflict custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        /// <param name="innerException">An inner exception that is the original source of the error</param>
        public EventSourceConcurrencyConflict(string message, Exception innerException)
            : base(message, innerException)
        {}
    
        /// <summary>
        ///     Initializes a new instance of the EventSourceConcurrencyConflict custom exception
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the object data of the exception</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        protected EventSourceConcurrencyConflict(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}