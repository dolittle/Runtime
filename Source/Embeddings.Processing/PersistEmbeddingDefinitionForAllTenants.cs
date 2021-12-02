// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Tenancy;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents an implementation of <see cref="IPersistEmbeddingDefinitionForAllTenants" />.
/// </summary>
[Singleton]
public class PersistEmbeddingDefinitionForAllTenants : IPersistEmbeddingDefinitionForAllTenants
{
    readonly IPerformActionOnAllTenants _onTenants;
    readonly FactoryFor<IEmbeddingDefinitions> _getDefinitions;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes an instance of the <see cref="PersistEmbeddingDefinitionForAllTenants" /> class.
    /// </summary>
    /// <param name="onTenants">The system that can perform an action on all tenants.</param>
    /// <param name="getDefinitions">The factory for getting embedding definitions scoped to the right execution context.</param>
    /// <param name="logger">The logger for logging messages.</param>
    public PersistEmbeddingDefinitionForAllTenants(
        IPerformActionOnAllTenants onTenants,
        FactoryFor<IEmbeddingDefinitions> getDefinitions,
        ILogger logger)
    {
        _onTenants = onTenants;
        _getDefinitions = getDefinitions;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Try> TryPersist(EmbeddingDefinition definition, CancellationToken token)
    {
        _logger.PersistingDefinition(definition.Embedding);
        var results = new Dictionary<TenantId, Try<bool>>();
        await _onTenants.PerformAsync(
            async tenant => results.Add(
                tenant,
                await _getDefinitions().TryPersist(definition, token).ConfigureAwait(false))).ConfigureAwait(false);

        if (results.Any(_ => !_.Value.Success))
        {
            return results.First(_ => !_.Value.Success).Value.Exception;
        }
        if (results.Any(_ => !_.Value.Result))
        {
            return new FailedPersistingEmbeddingDefinition(definition.Embedding);
        }

        return Try.Succeeded();
    }

}