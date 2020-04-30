// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines a system that can fetch <see cref="StreamEvent">events</see> from <see cref="StreamId">streams</see>.
    /// </summary>
    public interface ICanFetchEventsFromStream
    {
        /// <summary>
        /// Fetch the event at a given <see cref="StreamPosition" />.
        /// </summary>
        /// <param name="streamPosition"><see cref="StreamPosition">the position in the stream</see>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="StreamEvent" />.</returns>
        Task<StreamEvent> Fetch(StreamPosition streamPosition, CancellationToken cancellationToken);

        /// <summary>
        /// Fetch a range of events in an incluse <see cref="StreamPositionRange" />.
        /// </summary>
        /// <param name="range">The <see cref="StreamPositionRange" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IEnumerable{T}" /> of <see cref="StreamEvent" />.</returns>
        Task<IEnumerable<StreamEvent>> FetchRange(StreamPositionRange range, CancellationToken cancellationToken);
    }
}
