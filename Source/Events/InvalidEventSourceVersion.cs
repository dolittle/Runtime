namespace Dolittle.Runtime.Events
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Indicates that an <see cref="EventSourceVersion" /> was invalid (e.g. trying to increment the Sequence Number of the NoVersion or get the Previous Version of NoVersion)
    /// </summary>
    [Serializable]
    public class InvalidEventSourceVersion : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the InvalidEventSourceVersion custom exception
        /// </summary>
        public InvalidEventSourceVersion()
        {}

        /// <summary>
        ///     Initializes a new instance of the InvalidEventSourceVersion custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        public InvalidEventSourceVersion(string message)
            : base(message)
        {}

        /// <summary>
        ///     Initializes a new instance of the InvalidEventSourceVersion custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        /// <param name="innerException">An inner exception that is the original source of the error</param>
        public InvalidEventSourceVersion(string message, Exception innerException)
            : base(message, innerException)
        {}

        /// <summary>
        ///     Initializes a new instance of the InvalidEventSourceVersion custom exception
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the object data of the exception</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        protected InvalidEventSourceVersion(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}