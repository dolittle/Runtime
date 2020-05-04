// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a hub for <see cref="StreamProcessor" />.
    /// </summary>
    public interface IStreamProcessors
    {
        /// <summary>
        /// Registers a <see cref="StreamProcessor" />.
        /// </summary>
        /// <param name="streamDefinition">The <see cref="StreamDefinition" /> of the stream the <see cref="StreamProcessor" /> will be registered on.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="eventsFromStreamsFetcher">The <see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="StreamProcessorRegistration"/>.</returns>
        StreamProcessorRegistration Register(StreamDefinition streamDefinition, IEventProcessor eventProcessor, IFetchEventsFromStreams eventsFromStreamsFetcher, CancellationToken cancellationToken);

        /// <summary>
        /// Unregister a <see cref="StreamProcessor"/> from the in memory map.
        /// </summary>
        /// <remarks>Does not actually stop the stream processor. They are only stopped through the <see cref="CancellationToken" /> they are given.</remarks>
        /// <param name="streamProcessorId">The <see cref="StreamProcessorId" />.</param>
        void Unregister(StreamProcessorId streamProcessorId);
    }
}