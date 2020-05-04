// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Defines a system that knows about <see cref="ICanFetchEventsFromPublicStreams">event fetchers</see>.
    /// </summary>
    public interface IPublicEventFetchers
    {
        /// <summary>
        /// Gets an instance of <see cref="ICanFetchEventsFromPublicStreams" />.
        /// </summary>
        /// <param name="publicStreamId">The public <see cref="StreamId" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, whn resolved, returns the <see cref="ICanFetchEventsFromPublicStreams" />.</returns>
        Task<ICanFetchEventsFromPublicStreams> GetFetcherFor(StreamId publicStreamId, CancellationToken cancellationToken);
    }
}
