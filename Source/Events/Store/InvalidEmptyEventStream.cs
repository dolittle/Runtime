namespace Dolittle.Runtime.Events.Store
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the error when an <see cref="EventStream" /> is created with no events.
    /// </summary>
    [Serializable]
    public class InvalidEmptyEventStream : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the InvalidEmptyEventStream custom exception
        /// </summary>
        public InvalidEmptyEventStream()
        {}

        /// <summary>
        ///     Initializes a new instance of the InvalidEmptyEventStream custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        public InvalidEmptyEventStream(string message)
            : base(message)
        {}

        /// <summary>
        ///     Initializes a new instance of the InvalidEmptyEventStream custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        /// <param name="innerException">An inner exception that is the original source of the error</param>
        public InvalidEmptyEventStream(string message, Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        ///     Initializes a new instance of the InvalidEmptyEventStream custom exception
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the object data of the exception</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        protected InvalidEmptyEventStream(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}