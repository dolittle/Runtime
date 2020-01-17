// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Defines a system that can distribute <see cref="EventEnvelope">events</see> exactly once to an event processor.
    /// </summary>
    public interface IEventsDistributer
    {
        /// <summary>
        /// Gets the states of the event processors.
        /// </summary>
        IDictionary<EventStreamId, EventProcessorState> EventProcessorStates { get; }

        /// <summary>
        ///  Starts the distribution of <see cref="EventEnvelope">events</see> to the <see cref="ICanProcessStreamOfEvents">event stream processor</see>.
        /// </summary>
        /// <returns>A task.</returns>
        Task StartDistribution();
    }
}