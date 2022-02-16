// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Rudimentary;

using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="IResilientStreamProcessorStateRepository" />.
/// </summary>
[SingletonPerTenant]
public class ResilientStreamProcessorStateRepository : IResilientStreamProcessorStateRepository
{
    readonly IStreamProcessorStateRepository _repository;
    readonly IAsyncPolicyFor<ResilientStreamProcessorStateRepository> _policy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResilientStreamProcessorStateRepository"/> class.
    /// </summary>
    /// <param name="repository">The <see cref="IStreamProcessorStateRepository" />.</param>
    /// <param name="policy">The <see cref="IPolicyFor{T}" /> <see cref="ResilientStreamProcessorStateRepository" />.</param>
    public ResilientStreamProcessorStateRepository(IStreamProcessorStateRepository repository, IAsyncPolicyFor<ResilientStreamProcessorStateRepository> policy)
    {
        _repository = repository;
        _policy = policy;
    }

    /// <inheritdoc/>
    public Task Persist(IStreamProcessorId streamProcessorId, IStreamProcessorState streamProcessorState, CancellationToken cancellationToken) =>
        _policy.Execute(cancellationToken => _repository.Persist(streamProcessorId, streamProcessorState, cancellationToken), cancellationToken);

    /// <inheritdoc/>
    public Task<Try<IStreamProcessorState>> TryGetFor(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken) =>
        _policy.Execute(cancellationToken => _repository.TryGetFor(streamProcessorId, cancellationToken), cancellationToken);
}
