using System;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Defines the functionality required for an EventStore implementation
    /// </summary>
    public interface IEventStore : ICommitEventStreams, IFetchCommittedEvents, IFetchEventSourceVersion, IDisposable
    {
         
    }
}