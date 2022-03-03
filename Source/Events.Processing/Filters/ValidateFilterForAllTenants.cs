// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents an implementation of <see cref="IValidateFilterForAllTenants" />.
/// </summary>
public class ValidateFilterForAllTenants : IValidateFilterForAllTenants
{
    readonly IPerformActionsForAllTenants _performer;
    readonly Func<TenantId, IFilterValidators> _getFilterValidators;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidateFilterForAllTenants"/> class.
    /// </summary>
    /// <param name="performer">The <see cref="IPerformActionsForAllTenants" />.</param>
    /// <param name="getFilterValidators">The <see cref="IFilterValidators" />.</param>
    /// <param name="logger">The <see cref="ILogger" />.</param>
    public ValidateFilterForAllTenants(
        IPerformActionsForAllTenants performer,
        Func<TenantId, IFilterValidators> getFilterValidators,
        ILogger logger)
    {
        _performer = performer;
        _getFilterValidators = getFilterValidators;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IDictionary<TenantId, FilterValidationResult>> Validate<TDefinition>(Func<TenantId, IFilterProcessor<TDefinition>> getFilterProcessor, CancellationToken cancellationToken)
        where TDefinition : IFilterDefinition
    {
        var result = new Dictionary<TenantId, FilterValidationResult>();
        await _performer.PerformAsync(async (tenant, _) =>
        {
            var filterProcessor = getFilterProcessor(tenant);
            _logger.ValidatingFilterForTenant(filterProcessor.Identifier, tenant);
            var validationResult = await _getFilterValidators(tenant).Validate(filterProcessor, cancellationToken).ConfigureAwait(false);
            result.Add(tenant, validationResult);
        }).ConfigureAwait(false);
        return result;
    }
}
