// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Defines a system that can fetch <see cref="StreamEvent">events</see> from <see cref="StreamId">streams</see>.
/// </summary>
public interface ICanFetchEventsFromStream
{
    /// <summary>
    /// Fetch the events from a given <see cref="StreamPosition" />.
    /// </summary>
    /// <param name="position"><see cref="StreamPosition">the position in the stream</see>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>The <see cref="StreamEvent" />.</returns>
    Task<Try<IEnumerable<StreamEvent>>> Fetch(StreamPosition position, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the <see cref="StreamPosition"/> that will be used for the next event written to the stream. 
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="StreamPosition"/> offset.</returns>
    Task<Try<StreamPosition>> GetNextStreamPosition(CancellationToken cancellationToken);
}
