// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.DependencyInversion.Scoping;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents an implementation of <see cref="IResilientStreamProcessorStateRepository" />.
/// </summary>
[Singleton, PerTenant, DisableAutoRegistration] // TODO: We get circular dependencies since we register this as IStreamProcessorStateRepository by convention
public class ResilientStreamProcessorStateRepository : IResilientStreamProcessorStateRepository
{
    readonly IStreamProcessorStateRepository _repository;
    readonly IResilientStreamProcessorStateRepositoryPolicies _policies;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResilientStreamProcessorStateRepository"/> class.
    /// </summary>
    /// <param name="repository">The underlying stream processor state repository.</param>
    /// <param name="policies">The policies to use for resilience.</param>
    public ResilientStreamProcessorStateRepository(IStreamProcessorStateRepository repository, IResilientStreamProcessorStateRepositoryPolicies policies)
    {
        _repository = repository;
        _policies = policies;
    }

    /// <inheritdoc/>
    public Task Persist(IStreamProcessorId streamProcessorId, IStreamProcessorState streamProcessorState, CancellationToken cancellationToken)
        => _policies.Persisting.ExecuteAsync(_ => _repository.Persist(streamProcessorId, streamProcessorState, _), cancellationToken);

    /// <inheritdoc/>
    public Task<Try<IStreamProcessorState>> TryGetFor(IStreamProcessorId streamProcessorId, CancellationToken cancellationToken)
        => _policies.Getting.ExecuteAsync(_ => _repository.TryGetFor(streamProcessorId, _), cancellationToken);
}
