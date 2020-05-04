// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Store.Streams
{
    /// <summary>
    /// Defines a system that can fetch a range of <see cref="StreamEvent">events</see> from <see cref="StreamId">streams</see>.
    /// </summary>
    public interface IFetchRangeOfEventsFromStreams
    {
        /// <summary>
        /// Fetch a range of events in an incluse <see cref="StreamPositionRange" /> in a <see cref="StreamId">stream</see>.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="streamId"><see cref="StreamId">the stream in the event store</see>.</param>
        /// <param name="range">The <see cref="StreamPositionRange" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IEnumerable{T}" /> of <see cref="StreamEvent" />.</returns>
        Task<IEnumerable<StreamEvent>> FetchRange(ScopeId scope, StreamId streamId, StreamPositionRange range, CancellationToken cancellationToken);
    }
}
