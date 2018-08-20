using System;
using System.Runtime.Serialization;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CommitIsADuplicate : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the CommitIsADuplicate custom exception
        /// </summary>
        public CommitIsADuplicate()
        {}
    
        /// <summary>
        ///     Initializes a new instance of the CommitIsADuplicate custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        public CommitIsADuplicate(string message)
            : base(message)
        {}
    
        /// <summary>
        ///     Initializes a new instance of the CommitIsADuplicate custom exception
        /// </summary>
        /// <param name="message">A message describing the exception</param>
        /// <param name="innerException">An inner exception that is the original source of the error</param>
        public CommitIsADuplicate(string message, Exception innerException)
            : base(message, innerException)
        {}
    
        /// <summary>
        ///     Initializes a new instance of the CommitIsADuplicate custom exception
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the object data of the exception</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        protected CommitIsADuplicate(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}
    }
}