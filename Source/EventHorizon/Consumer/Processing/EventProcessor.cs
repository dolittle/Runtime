// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
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
    readonly ICommitExternalEvents _externalEventsCommitter;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventProcessor"/> class.
    /// </summary>
    /// <param name="consentId">THe <see cref="ConsentId" />.</param>
    /// <param name="subscription">The <see cref="Subscription" />.</param>
    /// <param name="externalEventsCommitter">The <see cref="ICommitExternalEvents"/>.</param>
    /// <param name="metrics">The system for collecting metrics.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public EventProcessor(
        ConsentId consentId,
        SubscriptionId subscription,
        ICommitExternalEvents externalEventsCommitter,
        IMetricsCollector metrics,
        ILogger logger)
    {
        _consentId = consentId;
        Scope = subscription.ScopeId;
        Identifier = subscription.ProducerTenantId.Value;
        _subscriptionId = subscription;
        _externalEventsCommitter = externalEventsCommitter;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public ScopeId Scope { get; }

    /// <inheritdoc/>
    public EventProcessorId Identifier { get; }

    /// <inheritdoc/>
    public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, StreamPosition position, ExecutionContext executionContext, CancellationToken cancellationToken) => Process(@event, cancellationToken);

    /// <inheritdoc/>
    public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, StreamPosition position, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        Log.RetryProcessEvent(_logger, _subscriptionId);
        return Process(@event, cancellationToken);
    }

    async Task<IProcessingResult> Process(CommittedEvent @event, CancellationToken _)
    {
        _metrics.IncrementTotalEventHorizonEventsProcessed();
        Log.ProcessEvent(_logger, @event.Type.Id, Scope, _subscriptionId.ProducerMicroserviceId, _subscriptionId.ProducerTenantId);
        try
        {
            await _externalEventsCommitter.Commit(new CommittedEvents([@event]), _consentId, Scope).ConfigureAwait(false);
            return SuccessfulProcessing.Instance;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to commit external event, will retry processing later");
            _metrics.IncrementTotalEventHorizonEventWritesFailed();
            throw;
        }
    }
}
