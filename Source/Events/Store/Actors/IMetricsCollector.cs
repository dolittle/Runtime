// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Persistence;

namespace Dolittle.Runtime.Events.Store.Actors;

/// <summary>
/// Defines a system for collecting metrics about the projection store.
/// </summary>
public interface IMetricsCollector
{
    /// <summary>
    /// Increments the total number of commits that has been received.
    /// </summary>
    void IncrementTotalCommitsReceived();
    
    /// <summary>
    /// Increments the total number of commits for aggregate that has been received.
    /// </summary>
    void IncrementTotalCommitsForAggregateReceived();

    /// <summary>
    /// Increments the total number of aggregate root version cache inconsistencies that has been encountered.
    /// </summary>
    void IncrementTotalAggregateRootVersionCacheInconsistencies();
    
    /// <summary>
    /// Increments the total number of commits that has been successfully written to the event store.
    /// </summary>
    void IncrementTotalBatchesSuccessfullyPersisted(Commit commit);

    /// <summary>
    /// Increments the total number of batches that has been sent to the event store persistence layer.
    /// </summary>
    void IncrementTotalBatchesSent();
    
    /// <summary>
    /// Increments the total number of aggregate root version cache inconsistencies that has been resolved.
    /// </summary>
    void IncrementTotalAggregateRootVersionCacheInconsistenciesResolved();
    
    /// <summary>
    /// Increments the total number of commit batches that failed being persisted 
    /// </summary>
    void IncrementTotalBatchesFailedPersisting();
    
    /// <summary>
    /// Increments the total number of <see cref="AggregateRootConcurrencyConflict"/> errors received when persisting commits.
    /// </summary>
    void IncrementTotalAggregateRootConcurrencyConflicts(TenantId tenant, ArtifactId aggregateRoot);
    
    /// <summary>
    /// Increments the number of events streamed in-memory per subscription.
    /// </summary>
    void IncrementStreamedSubscriptionEvents(string subscriptionName, int incBy);
    
    /// <summary>
    /// Increments the number of catch-up events (read from DB) per subscription.
    /// </summary>
    void IncrementCatchupSubscriptionEvents(string eventProcessorKind, int incBy);
}
