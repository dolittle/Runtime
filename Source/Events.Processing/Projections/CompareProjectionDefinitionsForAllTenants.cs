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

namespace Dolittle.Runtime.Events.Processing.Projections
{
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
            return result;
        }

        bool InitialStatesAreEqual(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition, ref ProjectionDefinitionComparisonResult result)
        {
            if (newDefinition.InititalState != oldDefinition.InititalState)
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
                    result = ProjectionDefinitionComparisonResult.Unequal($"Event {newEvent.EventType.Id.Value} was not in previous projectiondefinition");
                    return false;
                }

                if (oldEvent.KeySelectorType != newEvent.KeySelectorType)
                {
                    result = ProjectionDefinitionComparisonResult.Unequal($"Event {newEvent.EventType.Id.Value} does not have the same key selector type");
                    return false;
                }

                if (oldEvent.KeySelectorExpression != newEvent.KeySelectorExpression)
                {
                    result = ProjectionDefinitionComparisonResult.Unequal($"Event {newEvent.EventType.Id.Value} does not have the same key selector expressions");
                    return false;
                }
            }

            return true;
        }
    }
}
