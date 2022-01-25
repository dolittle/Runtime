// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents an implementation of <see cref="ICompareProjectionDefinitionsForAllTenants" />.
/// </summary>
[Singleton]
public class CompareProjectionDefinitionsForAllTenants : ICompareProjectionDefinitionsForAllTenants
{
    readonly IPerformActionOnAllTenants _onTenants;
    readonly FactoryFor<IProjectionDefinitions> _getDefinitions;

    /// <summary>
    /// Initializes an instance of the <see cref="CompareProjectionDefinitionsForAllTenants" /> class.
    /// </summary>
    /// <param name="onTenants">The tool for performing an action on all tenants.</param>
    /// <param name="getDefinitions">The factory for getting projection definitions.</param>
    public CompareProjectionDefinitionsForAllTenants(
        IPerformActionOnAllTenants onTenants,
        FactoryFor<IProjectionDefinitions> getDefinitions)
    {
        _onTenants = onTenants;
        _getDefinitions = getDefinitions;
    }

    /// <inheritdoc/>
    public async Task<IDictionary<TenantId, ProjectionDefinitionComparisonResult>> DiffersFromPersisted(ProjectionDefinition definition, CancellationToken token)
    {
        var results = new Dictionary<TenantId, ProjectionDefinitionComparisonResult>();
        await _onTenants.PerformAsync(async tenant =>
        {
            var definitions = _getDefinitions();
            var tryGetDefinition = await definitions.TryGet(definition.Projection, definition.Scope, token).ConfigureAwait(false);
            var comparisonResult = tryGetDefinition.Success switch
            {
                true => DefinitionsAreEqual(definition, tryGetDefinition),
                false => ProjectionDefinitionComparisonResult.Equal
            };
            results.Add(tenant, comparisonResult);
        }).ConfigureAwait(false);

        return results;
    }

    ProjectionDefinitionComparisonResult DefinitionsAreEqual(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition)
    {
        var result = ProjectionDefinitionComparisonResult.Equal;
        if (!InitialStatesAreEqual(newDefinition, oldDefinition, ref result)) return result;
        if (!EventsAreEqual(newDefinition, oldDefinition, ref result)) return result;
        if (!CopiesAreEqual(newDefinition, oldDefinition, ref result)) return result;
        return result;
    }

    bool InitialStatesAreEqual(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition, ref ProjectionDefinitionComparisonResult result)
    {
        if (newDefinition.InitialState != oldDefinition.InitialState)
        {
            result = ProjectionDefinitionComparisonResult.Unequal("The initial projection state is not the same as the persisted definition");
            return false;
        }
        return true;
    }
    bool EventsAreEqual(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition, ref ProjectionDefinitionComparisonResult result)
    {
        if (newDefinition.Events.Count() != oldDefinition.Events.Count())
        {
            result = ProjectionDefinitionComparisonResult.Unequal("The definitions does not have the same number of events");
            return false;
        }

        foreach (var newEvent in newDefinition.Events)
        {
            var oldEvent = oldDefinition.Events.FirstOrDefault(_ => _.EventType == newEvent.EventType);
            if (oldEvent == default)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Event {newEvent.EventType.Value} was not in previous projection definition");
                return false;
            }

            if (oldEvent.KeySelectorType != newEvent.KeySelectorType)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Event {newEvent.EventType.Value} does not have the same key selector type");
                return false;
            }

            if (oldEvent.KeySelectorExpression != newEvent.KeySelectorExpression)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Event {newEvent.EventType.Value} does not have the same key selector expressions");
                return false;
            }
        }

        return true;
    }

    bool CopiesAreEqual(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition, ref ProjectionDefinitionComparisonResult result)
        => CopyToMongoDBsAreEqual(newDefinition, oldDefinition, ref result);
    
    bool CopyToMongoDBsAreEqual(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition, ref ProjectionDefinitionComparisonResult result)
    {
        if (newDefinition.Copies.MongoDB.ShouldCopyToMongoDB != oldDefinition.Copies.MongoDB.ShouldCopyToMongoDB)
        {
            result = ProjectionDefinitionComparisonResult.Unequal("Should copy to MongoDB has changed from the previous projection definition");
            return false;
        }

        if (newDefinition.Copies.MongoDB.Collection != oldDefinition.Copies.MongoDB.Collection)
        {
            result = ProjectionDefinitionComparisonResult.Unequal("Copy to MongoDB collection name has changed the previous projection definition");
            return false;
        }

        if (newDefinition.Copies.MongoDB.Conversions.Count != oldDefinition.Copies.MongoDB.Conversions.Count)
        {
            result = ProjectionDefinitionComparisonResult.Unequal("Copy to MongoDB does not have the same number of conversions as the previous projection definition");
            return false;
        }

        foreach (var newConversion in newDefinition.Copies.MongoDB.Conversions)
        {
            var hasOldConversionForField = oldDefinition.Copies.MongoDB.Conversions.TryGetValue(newConversion.Key, out var oldConversionType);
            
            if (!hasOldConversionForField)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Copy to MongoDB conversion for field {newConversion.Key} did not exist in the previous projection definition");
                return false;
            }

            if (newConversion.Value != oldConversionType)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Copy to MongoDB conversion for field {newConversion.Key} has changed from the previous projection definition");
                return false;
            }
        }

        return true;
    }
}
