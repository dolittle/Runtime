// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Store.Streams;

/// <summary>
/// Represents an implementation of <see cref="IStreamDefinitions" />.
/// </summary>
[Singleton]
public class StreamDefinitions : IStreamDefinitions
{
    readonly IPerformActionOnAllTenants _onAllTenants;
    readonly Func<IStreamDefinitionRepository> _getStreamDefinitions;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamDefinitions"/> class.
    /// </summary>
    /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
    /// <param name="getStreamDefinitions">The <see cref="Func{T}" /> <see cref="IStreamDefinitionRepository" />.</param>
    public StreamDefinitions(IPerformActionOnAllTenants onAllTenants, Func<IStreamDefinitionRepository> getStreamDefinitions)
    {
        _onAllTenants = onAllTenants;
        _getStreamDefinitions = getStreamDefinitions;
    }

    /// <inheritdoc/>
    public Task Persist(ScopeId scope, IStreamDefinition streamDefinition, CancellationToken cancellationToken) =>
        _onAllTenants.PerformAsync(_ => _getStreamDefinitions().Persist(scope, streamDefinition, cancellationToken));
}
