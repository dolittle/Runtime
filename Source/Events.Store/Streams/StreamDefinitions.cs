// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents an implementation of <see cref="IStreamDefinitions" />.
/// </summary>
[Singleton]
public class StreamDefinitions : IStreamDefinitions
{
    readonly IPerformActionsForAllTenants _performer;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamDefinitions"/> class.
    /// </summary>
    /// <param name="performer">The <see cref="IPerformActionsForAllTenants"/> to resolve the dependencies for all tenants.</param>
    public StreamDefinitions(IPerformActionsForAllTenants performer)
    {
        _performer = performer;
    }

    /// <inheritdoc/>
    public Task Persist(ScopeId scope, IStreamDefinition streamDefinition, CancellationToken cancellationToken)
        => _performer.PerformAsyncOn<IStreamDefinitionRepository>(_ => _.Persist(scope, streamDefinition, cancellationToken));
}
