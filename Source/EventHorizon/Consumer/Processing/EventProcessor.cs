// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Resilience;

namespace Dolittle.Runtime.EventHorizon.Consumer.Processing;

/// <summary>
/// Represents an implementation of <see cref="IEventProcessor" />.
/// </summary>
public class EventProcessor : IEventProcessor
{
    readonly ConsentId _consentId;
    readonly SubscriptionId _subscriptionId;
    readonly IWriteEventHorizonEvents _receivedEventsWriter;
    readonly IAsyncPolicyFor<EventProcessor> _policy;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventProcessor"/> class.
    /// </summary>
    /// <param name="consentId">THe <see cref="ConsentId" />.</param>
    /// <param name="subscription">The <see cref="Subscription" />.</param>
    /// <param name="receivedEventsWriter">The <see cref="IWriteEventHorizonEvents" />.</param>
    /// <param name="policy">The <see cref="IAsyncPolicyFor{T}" /> <see cref="EventProcessor" />.</param>
    /// <param name="metrics">The system for collecting metrics.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public EventProcessor(
        ConsentId consentId,
        SubscriptionId subscription,
        IWriteEventHorizonEvents receivedEventsWriter,
        IAsyncPolicyFor<EventProcessor> policy,
        IMetricsCollector metrics,
        ILogger logger)
    {
        _consentId = consentId;
        Scope = subscription.ScopeId;
        Identifier = subscription.ProducerTenantId.Value;
        _subscriptionId = subscription;
        _receivedEventsWriter = receivedEventsWriter;
        _policy = policy;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public ScopeId Scope { get; }

    /// <inheritdoc/>
    public EventProcessorId Identifier { get; }

    /// <inheritdoc/>
    public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken) => Process(@event, cancellationToken);

    /// <inheritdoc/>
    public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken)
    {
        _logger.RetryProcessEvent(_subscriptionId);
        return Process(@event, cancellationToken);
    }

    async Task<IProcessingResult> Process(CommittedEvent @event, CancellationToken cancellationToken)
    {
        _metrics.IncrementTotalEventHorizonEventsProcessed();
        _logger.ProcessEvent(@event.Type.Id, Scope, _subscriptionId.ProducerMicroserviceId, _subscriptionId.ProducerTenantId);

        try
        {
            await _policy.Execute(
                cancellationToken => _receivedEventsWriter.Write(@event, _consentId, Scope, cancellationToken),
                cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            _metrics.IncrementTotalEventHorizonEventWritesFailed();
        }
        return new SuccessfulProcessing();
    }
}