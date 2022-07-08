// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Aggregates.Management;
using Dolittle.Runtime.Artifacts;
using Microservices;

namespace Dolittle.Runtime.CLI.Runtime.Aggregates;

/// <summary>
/// Represents an implementation of the <see cref="IResolveAggregateRootId"/>.  
/// </summary>
public class AggregateRootIdResolver : IResolveAggregateRootId
{
    readonly IManagementClient _managementClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootIdResolver"/> class.
    /// </summary>
    /// <param name="managementClient">The <see cref="IManagementClient"/>.</param>
    public AggregateRootIdResolver(IManagementClient managementClient)
    {
        _managementClient = managementClient;
    }

    /// <inheritdoc />
    public async Task<ArtifactId> ResolveId(MicroserviceAddress runtime, AggregateRootIdOrAlias idOrAlias)
    {
        if (!idOrAlias.IsAlias)
        {
            return idOrAlias.Id;
        }
        var aggregateRoots = await _managementClient.GetAll(runtime).ConfigureAwait(false);
        var aggregateRoot = aggregateRoots.FirstOrDefault(_ => WithAlias(_, idOrAlias));
            
        if (aggregateRoot == default)
        {
            throw new NoAggregateRootWithId(idOrAlias.Alias);
        }
        return aggregateRoot.AggregateRoot.Identifier.Id;
    }

    static bool WithAlias(AggregateRootWithTenantScopedInstances root, AggregateRootIdOrAlias idOrAlias)
        => !root.AggregateRoot.Alias.Equals(AggregateRootAlias.NotSet) && root.AggregateRoot.Alias.Equals(idOrAlias.Alias);
}