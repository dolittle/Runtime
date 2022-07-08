// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Defines a system that can fetch a range of <see cref="StreamEvent">events</see> from <see cref="StreamId">streams</see>.
/// </summary>
public interface ICanFetchRangeOfEventsFromStream
{
    /// <summary>
    /// Fetch a range of events in an include <see cref="StreamPositionRange" /> in a <see cref="StreamId">stream</see>.
    /// </summary>
    /// <param name="range">The <see cref="StreamPositionRange" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The <see cref="IAsyncEnumerable{T}" /> of <see cref="StreamEvent" />.</returns>
    IAsyncEnumerable<StreamEvent> FetchRange(StreamPositionRange range, CancellationToken cancellationToken);
}
