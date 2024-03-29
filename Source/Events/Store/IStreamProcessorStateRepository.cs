﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Defines a repository for <see cref="IStreamProcessorState"/>.
/// </summary>
public interface IStreamProcessorStateRepository<TId, TState>
    where TId : IStreamProcessorId
    where TState : IStreamProcessorState
{
    /// <summary>
    /// Gets the <see cref="IStreamProcessorState" /> for the given <see cref="IStreamProcessorId" />.
    /// </summary>
    /// <param name="streamProcessorId">The unique <see cref="IStreamProcessorId" /> representing the <see cref="AbstractScopedStreamProcessor"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" />.</returns>
    Task<Try<TState>> TryGet(TId streamProcessorId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the <see cref="IStreamProcessorState" /> for the given <see cref="IStreamProcessorId" /> from the correct
    /// collection, either <see cref="SubscriptionState" /> or <see cref="StreamProcessorState" />.
    /// </summary>
    /// <param name="scopeId">The <see cref="ScopeId"/> to get the stream processor states from.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" />.</returns>
    IAsyncEnumerable<StreamProcessorStateWithId<TId, TState>> GetForScope(ScopeId scopeId, CancellationToken cancellationToken);

    /// <summary>
    /// Persist the <see cref="IStreamProcessorState" /> for <see cref="StreamProcessorId"/> and <see cref="SubscriptionId"/>.
    /// Handles <see cref="Partitioned.PartitionedStreamProcessorState"/> separately also.
    /// IsUpsert option creates the document if one isn't found.
    /// </summary>
    /// <param name="scope">The <see cref="ScopeId"/>.</param>
    /// <param name="streamProcessorStates">The <see cref="IReadOnlyDictionary{TKey, TValue}"/> of <see cref="StreamProcessorId"/> and <see cref="IStreamProcessorState"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task<Try> PersistForScope(ScopeId scope, IReadOnlyDictionary<TId, TState> streamProcessorStates, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a repository for <see cref="IStreamProcessorState"/>.
/// </summary>
public interface IStreamProcessorStateRepository : IStreamProcessorStateRepository<StreamProcessorId, IStreamProcessorState>
{
    /// <summary>
    /// Gets the <see cref="IStreamProcessorState" /> for the given <see cref="IStreamProcessorId" /> from the correct
    /// collection, either <see cref="SubscriptionState" /> or <see cref="StreamProcessorState" />.
    /// </summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" />.</returns>
    IAsyncEnumerable<StreamProcessorStateWithId<StreamProcessorId, IStreamProcessorState>> GetNonScoped(CancellationToken cancellationToken);
}
