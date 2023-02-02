// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Store.Streams.Legacy;

static class Extensions
{
    public static async Task<EventLogSequenceNumber> FetchEventLogSequenceNumberAsync(this ICanFetchRangeOfEventsFromStream fetcher, StreamProcessorId id, StreamPosition position,
        CancellationToken cancellationToken)
    {
        var evt = await fetcher.FetchRange(new StreamPositionRange(position, 1), cancellationToken).Take(1).SingleAsync(cancellationToken);
        if (position != evt.Position)
        {
            throw new StreamProcessorStateDoesNotExist(id);
        }

        return evt.Event.EventLogSequenceNumber;
    }
}
