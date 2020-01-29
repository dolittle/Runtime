// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Defines a hub for <see cref="StreamProcessor" />.
    /// </summary>
    public interface IStreamProcessorHub
    {
        /// <summary>
        /// Registers and starts a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="sourceStreamId">The <see cref="StreamId" />.</param>
        void Register(IEventProcessor eventProcessor, StreamId sourceStreamId);
    }
}