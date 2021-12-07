// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Represents the sequence number of the Event Log as a natural number, corresponding to the number of events that has been committed to the Event Store.
/// </summary>
public record EventLogSequenceNumber(ulong Value) : ConceptAs<ulong>(Value)
{
    /// <summary>
    /// The initial sequence number of the Event Store before any Events are committed.
    /// </summary>
    public static readonly EventLogSequenceNumber Initial = 0;

    /// <summary>
    /// Implicitly convert a <see cref="ulong"/> to an <see cref="EventLogSequenceNumber"/>.
    /// </summary>
    /// <param name="number">The number.</param>
    public static implicit operator EventLogSequenceNumber(ulong number) => new(number);
}