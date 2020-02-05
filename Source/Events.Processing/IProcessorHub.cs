// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a hub for <see cref="StreamProcessor" /> interacting with a <see cref="RemoteEventProcessor" />.
    /// </summary>
    public interface IProcessorHub
    {
        /// <summary>
        /// Registers a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="processorId">The <see cref="EventProcessorId" />.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" /> that this stream processor listens to <see cref="Store.CommittedEvent" /> events from.</param>
        void Register(EventProcessorId processorId, StreamId sourceStreamId);
    }
}