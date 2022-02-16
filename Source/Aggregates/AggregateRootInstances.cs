// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Events.Store;


namespace Dolittle.Runtime.Aggregates;

/// <summary>
/// Represents an implementation of <see cref="IAggregateRootInstances"/>.
/// </summary>
[SingletonPerTenant]
public class AggregateRootInstances : IAggregateRootInstances
{
    readonly IAggregateRoots _aggregateRoots;
    readonly IFetchAggregateRootInstances _instances;
        
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootInstances"/> class.
    /// </summary>
    /// <param name="aggregateRoots">The Aggregate Roots.</param>
    /// <param name="instances">The system that can fetch Aggregate Root Instances.</param>
    public AggregateRootInstances(IAggregateRoots aggregateRoots, IFetchAggregateRootInstances instances)
    {
        _aggregateRoots = aggregateRoots;
        _instances = instances;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AggregateRootWithInstances>> GetAll()
    {
        var results = new List<AggregateRootWithInstances>();
        foreach (var aggregateRoot in _aggregateRoots.All)
        {
            var aggregateRootWithInstances = await GetFor(aggregateRoot.Identifier).ConfigureAwait(false);
            results.Add(aggregateRootWithInstances);
        }
        return results;
    }

    /// <inheritdoc />
    public async Task<AggregateRootWithInstances> GetFor(AggregateRootId identifier)
        => new(identifier, await FetchInstances(identifier).ConfigureAwait(false));

    Task<IEnumerable<AggregateRootInstance>> FetchInstances(AggregateRootId identifier)
        => _instances.FetchFor(identifier);
}
