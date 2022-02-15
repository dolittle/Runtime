// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents an implementation of <see cref="ICompareProjectionDefinitionsForAllTenants" />.
/// </summary>
[Singleton]
public class CompareProjectionDefinitionsForAllTenants : ICompareProjectionDefinitionsForAllTenants
{
    readonly IPerformActionOnAllTenants _onTenants;
    readonly Func<IProjectionDefinitions> _getDefinitions;

    /// <summary>
    /// Initializes an instance of the <see cref="CompareProjectionDefinitionsForAllTenants" /> class.
    /// </summary>
    /// <param name="onTenants">The tool for performing an action on all tenants.</param>
    /// <param name="getDefinitions">The factory for getting projection definitions.</param>
    public CompareProjectionDefinitionsForAllTenants(
        IPerformActionOnAllTenants onTenants,
        Func<IProjectionDefinitions> getDefinitions)
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

    static ProjectionDefinitionComparisonResult DefinitionsAreEqual(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition)
    {
        var result = ProjectionDefinitionComparisonResult.Equal;
        if (!InitialStatesAreEqual(newDefinition.InitialState, oldDefinition.InitialState, ref result))
        {
            return result;
        }
        if (!EventsAreEqual(newDefinition.Events, oldDefinition.Events, ref result))
        {
            return result;
        }
        if (!CopiesAreEqual(newDefinition.Copies, oldDefinition.Copies, ref result))
        {
            return result;
        }
        return result;
    }

    static bool InitialStatesAreEqual(ProjectionState newInitialState, ProjectionState oldInitialState, ref ProjectionDefinitionComparisonResult result)
    {
        if (newInitialState != oldInitialState)
        {
            result = ProjectionDefinitionComparisonResult.Unequal("The initial projection state is not the same as the persisted definition");
            return false;
        }
        return true;
    }
    static bool EventsAreEqual(IEnumerable<ProjectionEventSelector> newEvents, IEnumerable<ProjectionEventSelector> oldEvents, ref ProjectionDefinitionComparisonResult result)
    {
        if (newEvents.Count() != oldEvents.Count())
        {
            result = ProjectionDefinitionComparisonResult.Unequal("The definitions does not have the same number of events");
            return false;
        }

        foreach (var newEvent in newEvents)
        {
            var oldEvent = oldEvents.FirstOrDefault(_ => _.EventType == newEvent.EventType);
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

            if (oldEvent.StaticKey != newEvent.StaticKey)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Event {newEvent.EventType.Value} does not have the same static key");
                return false;
            }
            
            if (oldEvent.OccurredFormat != newEvent.OccurredFormat)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Event {newEvent.EventType.Value} does not have the occurred format");
                return false;
            }
        }

        return true;
    }

    static bool CopiesAreEqual(ProjectionCopySpecification newSpecification, ProjectionCopySpecification oldSpecification, ref ProjectionDefinitionComparisonResult result)
        => CopyToMongoDBsAreEqual(newSpecification.MongoDB, oldSpecification.MongoDB, ref result);
    
    static bool CopyToMongoDBsAreEqual(CopyToMongoDBSpecification newSpecification, CopyToMongoDBSpecification oldSpecification, ref ProjectionDefinitionComparisonResult result)
    {
        if (newSpecification.ShouldCopyToMongoDB != oldSpecification.ShouldCopyToMongoDB)
        {
            result = ProjectionDefinitionComparisonResult.Unequal("Should copy to MongoDB has changed from the previous projection definition");
            return false;
        }

        if (newSpecification.Collection != oldSpecification.Collection)
        {
            result = ProjectionDefinitionComparisonResult.Unequal("Copy to MongoDB collection name has changed the previous projection definition");
            return false;
        }

        return CopyToMongoDBConversionsAreEqual(newSpecification.Conversions, oldSpecification.Conversions, ref result);
    }

    static bool CopyToMongoDBConversionsAreEqual(PropertyConversion[] newConversions, PropertyConversion[] oldConversions, ref ProjectionDefinitionComparisonResult result)
    {
        if (newConversions.Length != oldConversions.Length)
        {
            result = ProjectionDefinitionComparisonResult.Unequal("Copy to MongoDB does not have the same number of conversions as the previous projection definition");
            return false;
        }

        foreach (var newConversion in newConversions)
        {
            var oldConversion = Array.Find(oldConversions, oldConversion => oldConversion.Property == newConversion.Property);
            if (oldConversion == default)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Copy to MongoDB conversion for property {newConversion.Property} did not exist in the previous projection definition");
                return false;
            }

            if (newConversion.Conversion != oldConversion.Conversion)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Copy to MongoDB conversion for property {newConversion.Property} has changed from the previous projection definition");
                return false;
            }

            if (newConversion.ShouldRename != oldConversion.ShouldRename)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Copy to MongoDB should rename for property {newConversion.Property} has changed from the previous projection definition");
                return false;
            }

            if (newConversion.RenameTo != oldConversion.RenameTo)
            {
                result = ProjectionDefinitionComparisonResult.Unequal($"Copy to MongoDB rename to for property {newConversion.Property} has changed from the previous projection definition");
                return false;
            }

            if (!CopyToMongoDBConversionsAreEqual(newConversion.Children, oldConversion.Children, ref result))
            {
                return false;
            }
        }

        return true;
    }
}
