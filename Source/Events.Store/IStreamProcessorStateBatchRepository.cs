// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Defines a repository for <see cref="IStreamProcessorState"/>.
/// </summary>
public interface IStreamProcessorStateBatchRepository
{
    /// <summary>
    /// Gets the <see cref="IStreamProcessorState" /> for the given <see cref="IStreamProcessorId" />.
    /// </summary>
    /// <param name="streamProcessorId">The unique <see cref="IStreamProcessorId" /> representing the <see cref="AbstractScopedStreamProcessor"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" />.</returns>
    Task<Try<IStreamProcessorState>> TryGet(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets the <see cref="IStreamProcessorState" /> for the given <see cref="IStreamProcessorId" /> from the correct
    /// collection, either <see cref="SubscriptionState" /> or <see cref="StreamProcessorState" />.
    /// </summary>
    /// <param name="id">The unique <see cref="IStreamProcessorId" /> representing either the <see cref="AbstractScopedStreamProcessor"/>
    /// or <see cref="SubscriptionId" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" />.</returns>
    IAsyncEnumerable<StreamProcessorStateWithId> GetAllNonScoped(CancellationToken cancellationToken);

    /// <summary>
    /// Persist the <see cref="IStreamProcessorState" /> for <see cref="StreamProcessorId"/> and <see cref="SubscriptionId"/>.
    /// Handles <see cref="Partitioned.PartitionedStreamProcessorState"/> separately also.
    /// IsUpsert option creates the document if one isn't found.
    /// </summary>
    /// <param name="streamProcessorStates">The <see cref="IEnumerable{T}"/> of <see cref="StreamProcessorStateWithId"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    Task Persist(IEnumerable<StreamProcessorStateWithId> streamProcessorStates, CancellationToken cancellationToken);
}
