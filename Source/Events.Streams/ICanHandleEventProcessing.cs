// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Streams
{
    /// <summary>
    /// Defines a system that can handle the processing of events.
    /// </summary>
    /// <typeparam name="TResult">The processing result.</typeparam>
    ///
    public interface ICanHandleEventProcessing<TResult>
        where TResult : Processing.ProcessingResult
    {
        /// <summary>
        /// Handles the processing of an event.
        /// </summary>
        /// <param name="eventStreamId">The event stream id.</param>
        /// <param name="event">The event.</param>
        /// <returns><typeparamref name="TResult" />.</returns>
        TResult Process(EventStreamId eventStreamId, EventEnvelope @event);
    }
}