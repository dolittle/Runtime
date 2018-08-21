/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Artifacts;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines something that is capable of processing an event 
    /// </summary>
    public interface IEventProcessor
    {
        /// <summary>
        /// Gets the identifier for the <see cref="IEventProcessor"/>
        /// </summary>
        EventProcessorIdentifier Identifier { get; }

        /// <summary>
        /// Gets the <see cref="Artifact"/> for the <see cref="IEvent">event type</see>
        /// it represents
        /// </summary>
        Artifact Event { get; }

        /// <summary>
        /// Process an event 
        /// </summary>
        /// <param name="eventEnvelope"><see cref="Store.EventEnvelope"/> for event to process</param>
        void Process(Store.EventEnvelope eventEnvelope);
    }
}