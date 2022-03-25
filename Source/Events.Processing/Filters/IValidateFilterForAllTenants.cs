// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Runtime.Tenancy;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Defines a system that can validate a Filter for <see cref="ITenants.All" />.
/// </summary>
public interface IValidateFilterForAllTenants
{
    /// <summary>
    /// Validates a Filter for all Tenants.
    /// </summary>
    /// <typeparam name="TDefinition">The <see cref="IFilterDefinition" /> type.</typeparam>
    /// <param name="getFilterProcessor">A <see cref="Func{TResult}" /> that returns <see cref="IFilterProcessor{TDefinition}" />.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns an <see cref="IDictionary{TKey, TValue}" /> of <see cref="TenantId" /> and <see cref="FilterValidationResult" />.</returns>
    Task<IDictionary<TenantId, FilterValidationResult>> Validate<TDefinition>(Func<TenantId, IFilterProcessor<TDefinition>> getFilterProcessor, CancellationToken cancellationToken)
        where TDefinition : IFilterDefinition;
}
