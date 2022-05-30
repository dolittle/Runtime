// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Represents an implementation <see cref="ICommitExternalEvents"/>. 
/// </summary>
public class ExternalEventsCommitter : ICommitExternalEvents
{
    readonly IWriteEventHorizonEvents _receivedEventsWriter;
    readonly IEventProcessorPolicies _policies;
    readonly EventStoreClient _eventStoreClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventProcessor"/> class.
    /// </summary>
    /// <param name="consentId">THe <see cref="ConsentId" />.</param>
    /// <param name="subscription">The <see cref="Subscription" />.</param>
    /// <param name="receivedEventsWriter">The <see cref="IWriteEventHorizonEvents" />.</param>
    /// <param name="policies">The <see cref="IEventProcessorPolicies" />.</param>
    /// <param name="metrics">The system for collecting metrics.</param>
    /// <param name="eventStoreClient">The <see cref="EventStoreClient"/>.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public ExternalEventsCommitter(
        IWriteEventHorizonEvents receivedEventsWriter,
        IEventProcessorPolicies policies,
        EventStoreClient eventStoreClient)
    {
        _receivedEventsWriter = receivedEventsWriter;
        _policies = policies;
        _eventStoreClient = eventStoreClient;
    }

    /// <inheritdoc />
    public async Task Commit(CommittedEvents events, ConsentId consent, ScopeId scope)
    {
        foreach (var @event in events)
        {
            var sequenceNumber = await _policies.WriteEvent.ExecuteAsync(
                _ => _receivedEventsWriter.Write(@event, consent, scope, CancellationToken.None),
                CancellationToken.None).ConfigureAwait(false);
                
            await _eventStoreClient.CommitExternal(new CommitExternalEventsRequest
            {
                ScopeId = scope.ToProtobuf(),
                Event = new CommittedEvent(
                    sequenceNumber,
                    @event.Occurred,
                    @event.EventSource,
                    @event.ExecutionContext,
                    @event.Type,
                    @event.Public,
                    @event.Content).ToProtobuf()
            }, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
