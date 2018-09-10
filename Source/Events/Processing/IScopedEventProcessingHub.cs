namespace Dolittle.Runtime.Events.Processing
{
    using Dolittle.Runtime.Events.Store;

    /// <summary>
    /// 
    /// </summary>
    public interface IScopedEventProcessingHub 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processor"></param>
        void Register(ScopedEventProcessor processor);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="committedEventStream"></param>
        void Process(CommittedEventStream committedEventStream);
        /// <summary>
        /// 
        /// </summary>
        void BeginProcessingEvents();
    }
}