namespace Dolittle.Runtime.Events.Processing
{
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Execution;

    /// <summary>
    /// Represents a combination of a <see cref="CommittedEventStream" /> and the related <see cref="ExecutionContext" /> 
    /// </summary>
    public class CommittedEventStreamWithContext
    {
        /// <summary>
        /// The <see cref="CommittedEventStream" />
        /// </summary>
        /// <value></value>
        public CommittedEventStream EventStream { get; }
        /// <summary>
        /// The <see cref="ExecutionContext" />
        /// </summary>
        /// <value></value>
        public ExecutionContext Context { get; }

        /// <summary>
        /// Instantiats a new instance of <see cref="CommittedEventStream" />
        /// </summary>
        /// <param name="stream">The committed event stream</param>
        /// <param name="context">The execution context</param>
        public CommittedEventStreamWithContext(CommittedEventStream stream, ExecutionContext context)
        {
            EventStream = stream;
            Context = context;
        }
    }    
}