// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Represents an implementation of <see cref="ICompareEmbeddingDefinitionsForAllTenants" />.
/// </summary>
[Singleton]
public class CompareEmbeddingDefinitionsForAllTenants : ICompareEmbeddingDefinitionsForAllTenants
{
    readonly IPerformActionOnAllTenants _onTenants;
    readonly Func<IEmbeddingDefinitions> _getDefinitions;

    /// <summary>
    /// Initializes an instance of the <see cref="CompareEmbeddingDefinitionsForAllTenants" /> class.
    /// </summary>
    /// <param name="onTenants">The tool for performing an action on all tenants.</param>
    /// <param name="getDefinitions">The factory for getting Embedding definitions.</param>
    public CompareEmbeddingDefinitionsForAllTenants(
        IPerformActionOnAllTenants onTenants,
        Func<IEmbeddingDefinitions> getDefinitions)
    {
        _onTenants = onTenants;
        _getDefinitions = getDefinitions;
    }

    /// <inheritdoc/>
    public async Task<IDictionary<TenantId, EmbeddingDefinitionComparisonResult>> DiffersFromPersisted(
        EmbeddingDefinition definition,
        CancellationToken token)
    {
        var results = new Dictionary<TenantId, EmbeddingDefinitionComparisonResult>();
        await _onTenants.PerformAsync(async tenant =>
        {
            var definitions = _getDefinitions();
            var tryGetDefinition = await definitions.TryGet(definition.Embedding, token).ConfigureAwait(false);
            var comparisonResult = tryGetDefinition.Success switch
            {
                true => DefinitionsAreEqual(definition, tryGetDefinition),
                false => EmbeddingDefinitionComparisonResult.Equal
            };
            results.Add(tenant, comparisonResult);
        }).ConfigureAwait(false);

        return results;
    }

    static EmbeddingDefinitionComparisonResult DefinitionsAreEqual(
        EmbeddingDefinition newDefinition,
        EmbeddingDefinition oldDefinition)
    {
        if (newDefinition.Embedding != oldDefinition.Embedding)
        {
            return EmbeddingDefinitionComparisonResult.Unequal($"The new Embedding identifier {newDefinition.Embedding.Value} is not the same as the persisted embedding identifier {oldDefinition.Embedding.Value}");
        }
        if (!InitialStatesAreEqual(newDefinition, oldDefinition, out var failedComparison))
        {
            return failedComparison;
        }
        return !EventsAreEqual(newDefinition, oldDefinition, out failedComparison)
            ? failedComparison
            : EmbeddingDefinitionComparisonResult.Equal;
    }

    static bool InitialStatesAreEqual(EmbeddingDefinition newDefinition, EmbeddingDefinition oldDefinition, out EmbeddingDefinitionComparisonResult failedComparison)
    {
        failedComparison = null;
        if (newDefinition.InititalState == oldDefinition.InititalState)
        {
            return true;
        }
        failedComparison = EmbeddingDefinitionComparisonResult.Unequal("The initial Embedding state is not the same as the persisted definition");
        return false;
    }

    static bool EventsAreEqual(EmbeddingDefinition newDefinition, EmbeddingDefinition oldDefinition, out EmbeddingDefinitionComparisonResult failedComparison)
    {
        failedComparison = null;
        if (newDefinition.Events.Count() != oldDefinition.Events.Count())
        {
            failedComparison = EmbeddingDefinitionComparisonResult.Unequal("The definitions does not have the same number of events");
            return false;
        }

        foreach (var newEvent in newDefinition.Events)
        {
            var oldEvent = oldDefinition.Events.FirstOrDefault(oldEvent => oldEvent.Id == newEvent.Id);
            if (oldEvent != default)
            {
                continue;
            }
            failedComparison = EmbeddingDefinitionComparisonResult.Unequal($"Event {newEvent.Id.Value} was not in previous Embeddingdefinition");
            return false;
        }

        return true;
    }
}
