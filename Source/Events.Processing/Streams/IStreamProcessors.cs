// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Defines a hub for <see cref="AbstractStreamProcessor" />.
    /// </summary>
    public interface IStreamProcessors
    {
        /// <summary>
        /// Registers a <see cref="AbstractStreamProcessor" />.
        /// </summary>
        /// <param name="streamDefinition">The <see cref="StreamDefinition" /> of the stream the <see cref="AbstractStreamProcessor" /> will be registered on.</param>
        /// <param name="eventProcessor">The <see cref="IEventProcessor" />.</param>
        /// <param name="eventsFromStreamsFetcher">The <see cref="IFetchEventsFromStreams" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolve, returns the <see cref="StreamProcessorRegistration"/>.</returns>
        Task<StreamProcessorRegistration> Register(StreamDefinition streamDefinition, IEventProcessor eventProcessor, IFetchEventsFromStreams eventsFromStreamsFetcher, CancellationToken cancellationToken);
    }
}