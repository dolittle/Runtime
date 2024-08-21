// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Represents an implementation of <see cref="ICanFetchEventsFromStream" />.
/// </summary>
public class EventsFromEventHorizonFetcher : ICanFetchEventsFromStream, IStreamEventWatcher
{
    readonly Channel<StreamEvent> _events;
    readonly IMetricsCollector _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventsFromEventHorizonFetcher"/> class.
    /// </summary>
    /// <param name="events">The <see cref="Channel{TResponse}" />.</param>
    /// <param name="metrics">The system for collecting metrics.</param>
    public EventsFromEventHorizonFetcher(Channel<StreamEvent> events, IMetricsCollector metrics)
    {
        _events = events;
        _metrics = metrics;
    }

    /// <inheritdoc/>
    public async Task<Try<IEnumerable<StreamEvent>>> Fetch(StreamPosition position, CancellationToken cancellationToken)
    {
        try
        {
            //TODO: This can be improved by taking as many as possible instead of just the first
            var @event = await _events.Reader.ReadAsync(cancellationToken).ConfigureAwait(false);
            _metrics.IncrementTotalEventsFetched();
            return new[] { @event };
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// Unused
    public Task<Try<StreamEvent>> FetchSingle(StreamPosition _, CancellationToken cancellationToken) =>
        throw new NotImplementedException("unused for eventhorizon");

    /// Unused
    public Task<Try<StreamEvent>> FetchLast(CancellationToken cancellationToken) =>
        throw new NotImplementedException("unused for eventhorizon");

    /// <inheritdoc />
    public async Task<Try<StreamPosition>> GetNextStreamPosition(CancellationToken cancellationToken)
        => new NotImplementedException("GetNextStreamPosition should never be used on this specific fetcher");

    /// <inheritdoc/>
    public void NotifyForEvent(ScopeId scope, StreamId stream, StreamPosition position)
    {
    }

    /// <inheritdoc/>
    public void NotifyForEvent(StreamId stream, StreamPosition position)
    {
    }

    /// <inheritdoc/>
    public Task WaitForEvent(ScopeId scope, StreamId stream, StreamPosition position, CancellationToken token) => Task.Delay(60 * 1000, token);

    /// <inheritdoc/>
    public Task WaitForEvent(ScopeId scope, StreamId stream, StreamPosition position, TimeSpan timeout, CancellationToken token) =>
        Task.Delay(60 * 1000, token);

    /// <inheritdoc/>
    public Task WaitForEvent(ScopeId scope, StreamId stream, TimeSpan timeout, CancellationToken token) => Task.Delay(60 * 1000, token);

    /// <inheritdoc/>
    public Task WaitForEvent(ScopeId scope, StreamId stream, CancellationToken token) => Task.Delay(60 * 1000, token);

    /// <inheritdoc/>
    public Task WaitForEvent(StreamId stream, StreamPosition position, TimeSpan timeout, CancellationToken token) => Task.Delay(60 * 1000, token);

    /// <inheritdoc/>
    public Task WaitForEvent(StreamId stream, StreamPosition position, CancellationToken token) => Task.Delay(60 * 1000, token);
}
