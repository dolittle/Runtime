using System;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines how we get and set the Event Processor Offset (last event processed)
    /// </summary>
    public interface IEventProcessorOffsetRepository
    {
        /// <summary>
        /// Gets the Offset (last event processed) for this <see cref="IEventProcessor" />
        /// </summary>
        /// <param name="eventProcessorId"><see cref="EventProcessorId">Id</see> of the <see cref="IEventProcessor" /> </param>
        /// <returns><see cref="CommittedEventVersion">Offset</see>  for this <see cref="IEventProcessor" /></returns>
        CommittedEventVersion Get(EventProcessorId eventProcessorId);
        /// <summary>
        /// Sets the Offset (last event processed) for this <see cref="IEventProcessor" />
        /// </summary>
        /// <param name="eventProcessorId"><see cref="EventProcessorId">Id</see> of the <see cref="IEventProcessor" /> </param>
        /// <param name="committedEventVersion"><see cref="CommittedEventVersion">Offset</see> to persist for this <see cref="IEventProcessor" /></param>
        void Set(EventProcessorId eventProcessorId, CommittedEventVersion committedEventVersion);
    }
}