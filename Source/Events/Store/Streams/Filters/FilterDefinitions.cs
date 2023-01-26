// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Store.Streams.Filters;

/// <summary>
/// Represents an implementation of <see cref="IFilterDefinitions" />.
/// </summary>
public class FilterDefinitions : IFilterDefinitions
{
    readonly IStreamDefinitionRepository _streamDefinitions;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterDefinitions"/> class.
    /// </summary>
    /// <param name="streamDefinitions">The <see cref="IStreamDefinitionRepository" />.</param>
    public FilterDefinitions(
        IStreamDefinitionRepository streamDefinitions)
    {
        _streamDefinitions = streamDefinitions;
    }

    /// <inheritdoc/>
    public async Task<Try<IFilterDefinition>> TryGetFromStream(ScopeId scopeId, StreamId streamId, CancellationToken cancellationToken)
    {
        var tryGetStream = await _streamDefinitions.TryGet(scopeId, streamId, cancellationToken).ConfigureAwait(false);
        return tryGetStream.Select(_ => _.FilterDefinition);
    }

    /// <inheritdoc/>
    public async Task<bool> TryPersist(ScopeId scopeId, IFilterDefinition filterDefinition, CancellationToken cancellationToken)
    {
        var tryGetStream = await _streamDefinitions.TryGet(scopeId, filterDefinition.TargetStream, cancellationToken).ConfigureAwait(false);

        if (!tryGetStream.Success)
        {
            return false;
        }
        var newStreamDefinition = new StreamDefinition(filterDefinition);
        await _streamDefinitions.Persist(scopeId, newStreamDefinition, cancellationToken).ConfigureAwait(false);
        return true;
    }
}