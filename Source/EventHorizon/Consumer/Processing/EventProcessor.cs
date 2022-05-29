// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Actors;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Represents an implementation of <see cref="IEventProcessor" />.
/// </summary>
public class EventProcessor : IEventProcessor
{
    readonly ConsentId _consentId;
    readonly SubscriptionId _subscriptionId;
    readonly IWriteEventHorizonEvents _receivedEventsWriter;
    readonly IEventProcessorPolicies _policies;
    readonly IMetricsCollector _metrics;
    readonly EventStoreClient _eventStoreClient;
    readonly ILogger _logger;

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
    public EventProcessor(
        ConsentId consentId,
        SubscriptionId subscription,
        IWriteEventHorizonEvents receivedEventsWriter,
        IEventProcessorPolicies policies,
        IMetricsCollector metrics,
        EventStoreClient eventStoreClient,
        ILogger logger)
    {
        _consentId = consentId;
        Scope = subscription.ScopeId;
        Identifier = subscription.ProducerTenantId.Value;
        _subscriptionId = subscription;
        _receivedEventsWriter = receivedEventsWriter;
        _policies = policies;
        _metrics = metrics;
        _eventStoreClient = eventStoreClient;
        _logger = logger;
    }

    /// <inheritdoc/>
    public ScopeId Scope { get; }

    /// <inheritdoc/>
    public EventProcessorId Identifier { get; }

    /// <inheritdoc/>
    public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, ExecutionContext executionContext, CancellationToken cancellationToken) => Process(@event, cancellationToken);

    /// <inheritdoc/>
    public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        Log.RetryProcessEvent(_logger, _subscriptionId);
        return Process(@event, cancellationToken);
    }

    async Task<IProcessingResult> Process(CommittedEvent @event, CancellationToken cancellationToken)
    {
        _metrics.IncrementTotalEventHorizonEventsProcessed();
        Log.ProcessEvent(_logger, @event.Type.Id, Scope, _subscriptionId.ProducerMicroserviceId, _subscriptionId.ProducerTenantId);
        try
        {
            await _policies.WriteEvent.ExecuteAsync(
                cancellationToken => _receivedEventsWriter.Write(@event, _consentId, Scope, cancellationToken),
                cancellationToken).ConfigureAwait(false);
            await _eventStoreClient.CommitExternal(new CommitExternalEventsRequest
            {
                ScopeId = Scope.ToProtobuf(),
                Event = @event.ToProtobuf()
            }, cancellationToken);
        }
        catch
        {
            _metrics.IncrementTotalEventHorizonEventWritesFailed();
        }
        return new SuccessfulProcessing();
    }
}
