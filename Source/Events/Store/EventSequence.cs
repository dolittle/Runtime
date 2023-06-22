// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Represents the basis for a sequence of <see cref="Event" >events</see>.
/// </summary>
/// <typeparam name="TEvent">IReadOnlyList of CommittedEvent or UncommittedEvent.</typeparam>
public abstract class EventSequence<TEvent> : IReadOnlyList<TEvent>
    where TEvent : Event
{
    readonly IReadOnlyList<TEvent> _events;
    /// <summary>
    /// Initializes a new instance of the <see cref="EventSequence{T}"/> class.
    /// </summary>
    /// <param name="events">IReadOnlyList of events.</param>
    protected EventSequence(IReadOnlyList<TEvent> events)
    {
        foreach (var @event in events)
        {
            ThrowIfEventIsNull(@event);   
        }
        _events = events;
    }

    /// <inheritdoc/>
    public int Count => _events.Count;

    /// <summary>
    /// Gets a value indicating whether or not there are any events in the committed sequence.
    /// </summary>
    public bool HasEvents => Count > 0;


    /// <inheritdoc/>
    public TEvent this[int index] => _events[index];

    /// <inheritdoc/>
    public IEnumerator<TEvent> GetEnumerator() => _events.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _events.GetEnumerator();

    static void ThrowIfEventIsNull(TEvent @event)
    {
        if (@event == null)
        {
            throw new EventCanNotBeNull();
        }
    }
}
