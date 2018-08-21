namespace Dolittle.Runtime.Events.Store
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the error when an <see cref="SingleEventTypeEventStream" /> is created with events from more than one Event Type.
    /// </summary>
    [Serializable]
    public class MultipleEventTypesInSingleEventTypeEventStream : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the MultipleEventTypesInSingleEventTypeEventStream custom exception
        /// </summary>
        public MultipleEventTypesInSingleEventTypeEventStream()
        {}

        /// <summary>
        ///     Initializes a new instance of the MultipleEventTypesInSingleEventTypeEventStream custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        public MultipleEventTypesInSingleEventTypeEventStream(string message)
            : base(message)
        {}

        /// <summary>
        ///     Initializes a new instance of the MultipleEventTypesInSingleEventTypeEventStream custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        /// <param name="innerException">An inner exception that is the original source of the error</param>
        public MultipleEventTypesInSingleEventTypeEventStream(string message, Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        ///     Initializes a new instance of the MultipleEventTypesInSingleEventTypeEventStream custom exception
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the object data of the exception</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        protected MultipleEventTypesInSingleEventTypeEventStream(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}