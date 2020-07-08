// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Tenancy;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IValidateFilterForAllTenants" />.
    /// </summary>
    public class ValidateFilterForAllTenants : IValidateFilterForAllTenants
    {
        readonly IPerformActionOnAllTenants _onAllTenants;
        readonly IFilterValidators _filterValidators;
        readonly FactoryFor<IFilterDefinitions> _getFilterDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateFilterForAllTenants"/> class.
        /// </summary>
        /// <param name="onAllTenants">The <see cref="IPerformActionOnAllTenants" />.</param>
        /// <param name="filterValidators">The <see cref="IFilterValidators" />.</param>
        /// <param name="getFilterDefinitions">The <see cref="FactoryFor{T}" /> <see cref="IFilterDefinitions" />.</param>
        public ValidateFilterForAllTenants(IPerformActionOnAllTenants onAllTenants, IFilterValidators filterValidators, FactoryFor<IFilterDefinitions> getFilterDefinitions)
        {
            _onAllTenants = onAllTenants;
            _filterValidators = filterValidators;
            _getFilterDefinitions = getFilterDefinitions;
        }

        /// <inheritdoc/>
        public async Task<IDictionary<TenantId, FilterValidationResult>> Validate<TDefinition>(Func<IFilterProcessor<TDefinition>> getFilterProcessor, CancellationToken cancellationToken)
            where TDefinition : IFilterDefinition
        {
            var result = new Dictionary<TenantId, FilterValidationResult>();
            await _onAllTenants.PerformAsync(async tenantId =>
                {
                    var filterProcessor = getFilterProcessor();
                    var tryGetFilterDefinition = await _getFilterDefinitions().TryGetFromStream(filterProcessor.Scope, filterProcessor.Definition.TargetStream, cancellationToken).ConfigureAwait(false);
                    if (tryGetFilterDefinition.Success)
                    {
                        var validationResult = await _filterValidators.Validate(tryGetFilterDefinition.Result, filterProcessor, cancellationToken).ConfigureAwait(false);
                        result.Add(tenantId, validationResult);
                    }
                    else
                    {
                        result.Add(tenantId, new FilterValidationResult());
                    }
                }).ConfigureAwait(false);

            return result;
        }
    }
}
