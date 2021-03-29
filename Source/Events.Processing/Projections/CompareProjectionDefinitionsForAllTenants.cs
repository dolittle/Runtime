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
                    true => CompareDefinitions(definition, tryGetDefinition),
                    false => ProjectionDefinitionComparisonResult.Equal
                };
                results.Add(tenant, comparisonResult);

            }).ConfigureAwait(false);

            return results;
        }

        ProjectionDefinitionComparisonResult CompareDefinitions(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition)
        {
            if (CompareInitialState(newDefinition, oldDefinition, out var result)) return result;
            if (CompareEvents(newDefinition, oldDefinition, out result)) return result;
            return ProjectionDefinitionComparisonResult.Equal;
        }

        bool CompareInitialState(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition, out ProjectionDefinitionComparisonResult result)
        {
            result = null;
            if (newDefinition.InititalState != oldDefinition.InititalState)
            {
                result = ProjectionDefinitionComparisonResult.Unequal("The initial projection state is not the same as the persisted definition");
                return true;
            }
            return false;
        }
        bool CompareEvents(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition, out ProjectionDefinitionComparisonResult result)
            => CompareEventTypes(newDefinition, oldDefinition, out result)
                || CompareEventSelectors(newDefinition, oldDefinition, out result);

        bool CompareEventTypes(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition, out ProjectionDefinitionComparisonResult result)
        {
            result = null;
            var newEventTypes = newDefinition.Events.Select(_ => _.EventType);
            var oldEventTypes = oldDefinition.Events.Select(_ => _.EventType);

            if (newEventTypes.Count() != oldEventTypes.Count())
            {
                result = ProjectionDefinitionComparisonResult.Unequal("The definitions does not have the same number of event types");
                return true;
            }
            if (!newEventTypes.All(_ => oldEventTypes.Contains(_)))
            {
                result = ProjectionDefinitionComparisonResult.Unequal("The definitions does not have the same event types");
                return true;
            }

            return false;
        }

        bool CompareEventSelectors(ProjectionDefinition newDefinition, ProjectionDefinition oldDefinition, out ProjectionDefinitionComparisonResult result)
        {
            result = null;
            var matchingSelectors = newDefinition.Events
                .GroupJoin(
                    oldDefinition.Events,
                    _ => _.EventType,
                    _ => _.EventType,
                    (item, matches) => (newSelector: item, oldSelector: matches.First()));

            foreach (var (newSelector, oldSelector) in matchingSelectors)
            {
                if (CompareKeySelectorType(newSelector.KeySelectorType, oldSelector.KeySelectorType, out result)) return true;
                if (CompareKeySelectorExpression(newSelector.KeySelectorExpression, oldSelector.KeySelectorExpression, out result)) return true;
            }

            return false;
        }

        bool CompareKeySelectorType(ProjectEventKeySelectorType newType, ProjectEventKeySelectorType oldType, out ProjectionDefinitionComparisonResult result)
        {
            result = null;
            if (newType != oldType)
            {
                result = ProjectionDefinitionComparisonResult.Unequal("One or more key selector types does not match");
                return true;
            }
            return false;
        }

        bool CompareKeySelectorExpression(KeySelectorExpression newKeySelectorExpression, KeySelectorExpression oldKeySelectorExpression, out ProjectionDefinitionComparisonResult result)
        {
            result = null;
            if (newKeySelectorExpression != oldKeySelectorExpression)
            {
                result = ProjectionDefinitionComparisonResult.Unequal("One or more key selector expressions does not match");
                return true;
            }
            return false;
        }
    }
}
