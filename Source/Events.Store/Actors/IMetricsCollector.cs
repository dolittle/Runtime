// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    void IncrementTotalBatchesSuccessfullyPersisted();

    
    /// <summary>
    /// Increments the total number of events in commits that has been successfully written to the event store.
    /// </summary>
    void IncrementTotalBatchedEventsSuccessfullyPersisted(int allEventsCount);

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
}
