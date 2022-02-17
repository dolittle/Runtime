// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.CLI.Runtime.EventTypes;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Microservices;
using Dolittle.Runtime.Serialization.Json;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates;

/// <summary>
/// A shared command base for the "dolittle runtime aggregates" commands that provides shared arguments.
/// </summary>
public abstract class CommandBase : Runtime.CommandBase
{
    readonly IResolveAggregateRootId _aggregateRootIdResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBase"/> class.
    /// </summary>
    /// <param name="runtimes">The Runtime locator to find a Runtime to connect to.</param>
    /// <param name="aggregateRootIdResolver">The Aggregate Root Id resolver.</param>
    /// <param name="jsonSerializer">The json <see cref="ISerializer"/>.</param>
    protected CommandBase(ICanLocateRuntimes runtimes, IResolveAggregateRootId aggregateRootIdResolver, IDiscoverEventTypes eventTypesDiscoverer, ISerializer jsonSerializer)
        : base(runtimes, eventTypesDiscoverer, jsonSerializer)
    {
        _aggregateRootIdResolver = aggregateRootIdResolver;
    }
        
    /// <summary>
    /// Gets the Aggregate Root id.
    /// </summary>
    /// <param name="runtime">The Runtime microservice address.</param>
    /// <param name="idOrAlias">The AggregateRootId Id or Alias.</param>
    /// <returns>A <see cref="Task{TResult}"/> that, when resolved, returns the <see cref="ArtifactId"/>.</returns>
    protected Task<ArtifactId> GetAggregateRootId(MicroserviceAddress runtime, AggregateRootIdOrAlias idOrAlias)
        => _aggregateRootIdResolver.ResolveId(runtime, idOrAlias);
}