namespace Dolittle.Runtime.Events.Processing
{
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Runtime.Execution;

    /// <summary>
    /// Represents a combination of a <see cref="CommittedEventStream" /> and the related <see cref="IExecutionContext" /> 
    /// </summary>
    public class CommittedEventStreamWithContext
    {
        /// <summary>
        /// The <see cref="CommittedEventStream" />
        /// </summary>
        /// <value></value>
        public CommittedEventStream EventStream { get; }
        /// <summary>
        /// The <see cref="IExecutionContext" />
        /// </summary>
        /// <value></value>
        public IExecutionContext Context { get; }

        /// <summary>
        /// Instantiats a new instance of <see cref="CommittedEventStream" />
        /// </summary>
        /// <param name="stream">The committed event stream</param>
        /// <param name="context">The execution context</param>
        public CommittedEventStreamWithContext(CommittedEventStream stream, IExecutionContext context)
        {
            EventStream = stream;
            Context = context;
        }
    }    
}