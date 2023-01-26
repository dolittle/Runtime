// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store;

/// <summary>
/// Defines a system for collecting metrics about the projection store.
/// </summary>
public interface IMetricsCollector
{
    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryGet"/>.
    /// </summary>
    void IncrementTotalGet();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryGet"/> that failed.
    /// </summary>
    void IncrementTotalFailedGet();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryGetAll"/>.
    /// </summary>
    void IncrementTotalGetAll();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryGetAll"/> that failed.
    /// </summary>
    void IncrementTotalFailedGetAll();
    
    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionPersister.TryReplace"/>.
    /// </summary>
    void IncrementTotalReplaceAttempts();

    /// <summary>
    /// Increments the total number of writes to copy stores from calls to <see cref="IProjectionPersister.TryReplace"/>.
    /// </summary>
    void IncrementTotalCopyStoreReplacements();

    /// <summary>
    /// Increments the total number of writes to copy stores from calls to <see cref="IProjectionPersister.TryReplace"/> that failed.
    /// </summary>
    void IncrementTotalFailedCopyStoreReplacements();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryReplace"/>.
    /// </summary>
    void IncrementTotalProjectionStoreReplacements();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryReplace"/> that failed.
    /// </summary>
    void IncrementTotalFailedProjectionStoreReplacements();
   
    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionPersister.TryRemove"/>.
    /// </summary> 
    void IncrementTotalRemoveAttempts();

    /// <summary>
    /// Increments the total number of writes to copy stores from calls to <see cref="IProjectionPersister.TryRemove"/>.
    /// </summary>
    void IncrementTotalCopyStoreRemovals();

    /// <summary>
    /// Increments the total number of writes to copy stores from calls to <see cref="IProjectionPersister.TryRemove"/> that failed.
    /// </summary>
    void IncrementTotalFailedCopyStoreRemovals();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryRemove"/>.
    /// </summary>
    void IncrementTotalProjectionStoreRemovals();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryRemove"/> that failed.
    /// </summary>
    void IncrementTotalFailedProjectionStoreRemovals();
   
    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionPersister.TryDrop"/>.
    /// </summary> 
    void IncrementTotalDropAttempts();

    /// <summary>
    /// Increments the total number of writes to copy stores from calls to <see cref="IProjectionPersister.TryDrop"/>.
    /// </summary>
    void IncrementTotalCopyStoreDrops();

    /// <summary>
    /// Increments the total number of writes to copy stores from calls to <see cref="IProjectionPersister.TryDrop"/> that failed.
    /// </summary>
    void IncrementTotalFailedCopyStoreDrops();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryDrop"/>.
    /// </summary>
    void IncrementTotalProjectionStoreDrops();

    /// <summary>
    /// Increments the total number of calls to <see cref="IProjectionStore.TryDrop"/> that failed.
    /// </summary>
    void IncrementTotalFailedProjectionStoreDrops();
}
