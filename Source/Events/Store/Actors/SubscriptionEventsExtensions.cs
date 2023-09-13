// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;

namespace Dolittle.Runtime.Events.Store.Actors;

public static class SubscriptionEventsExtensions
{
    /// <summary>
    /// Add two <see cref="SubscriptionEvents"/> together, mutating the first one.
    /// They will need to be sequential, and match 
    /// </summary>
    /// <param name="updated"></param>
    /// <param name="added"></param>
    /// <returns></returns>
    public static SubscriptionEvents Merge(this SubscriptionEvents updated, SubscriptionEvents added)
    {
        if (updated.ToOffset != (added.FromOffset - 1))
        {
            throw new ArgumentException("The updated events must be sequential with the added events", nameof(updated));
        }

        updated.Events.AddRange(added.Events);
        updated.ToOffset = added.ToOffset;
        return updated;
    }

    /// <summary>
    /// Remove events earlier than the given offset. Does not mutate the original, but can return the original if no change is needed.
    /// If all events are before the cutoff, null is returned.
    /// </summary>
    /// <param name="existing"></param>
    /// <param name="fromOffset"></param>
    /// <returns></returns>
    public static SubscriptionEvents? CutoffEarlierThan(this SubscriptionEvents existing, ulong fromOffset)
    {
        if (existing.ToOffset < fromOffset)
        {
            // Already exported
            return null;
        }

        if (existing.FromOffset == fromOffset)
        {
            return existing; // No change
        }

        return new SubscriptionEvents
        {
            FromOffset = fromOffset,
            ToOffset = existing.ToOffset,
            Events = { existing.Events.SkipWhile(evt => evt.EventLogSequenceNumber < fromOffset) },
        };
    }
}
