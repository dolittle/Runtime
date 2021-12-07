// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Microservices;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates;

/// <summary>
/// Defines a system that resolves an Aggregate Root Id from <see cref="AggregateRootIdOrAlias"/>.
/// </summary>
public interface IResolveAggregateRootId
{
    /// <summary>
    /// Resolves the <see cref="ArtifactId"/> from <see cref="AggregateRootIdOrAlias"/>.
    /// </summary>
    /// <param name="runtime">The address to the Runtime.</param>
    /// <param name="idOrAlias">The Aggregate Root Id or Alias.</param>
    /// <returns>A <see cref="Task"/> that, when resolved, returns the resolved <see cref="ArtifactId"/>.</returns>
    Task<ArtifactId> ResolveId(MicroserviceAddress runtime, AggregateRootIdOrAlias idOrAlias);
        
}