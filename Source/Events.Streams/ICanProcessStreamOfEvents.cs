// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Defines a system that process event streams.
    /// </summary>
    public interface ICanProcessStreamOfEvents
    {
        /// <summary>
        /// Processes a stream of events.
        /// </summary>
        /// <param name="eventStream">The stream of events.</param>
        /// <returns>A task.</returns>
        Task Process(IObservable<EventEnvelope> eventStream);
    }
}