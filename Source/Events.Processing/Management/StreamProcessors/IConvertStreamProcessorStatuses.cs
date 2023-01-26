// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Management.StreamProcessors;

public interface IConvertStreamProcessorStatuses
{
    /// <summary>
    /// Converts stream processor states per tenant from Runtime to Contracts representation.
    /// </summary>
    /// <param name="states">The dictionary of <see cref="TenantId"/> to <see cref="IStreamProcessorState"/> to convert.</param>
    /// <returns>The converted <see cref="Contracts.TenantScopedStreamProcessorStatus"/>.</returns>
    IEnumerable<Contracts.TenantScopedStreamProcessorStatus> Convert(IDictionary<TenantId, IStreamProcessorState> states);
    
    /// <summary>
    /// Converts stream processor states per tenant from Runtime to Contracts representation for a specific tenant.
    /// </summary>
    /// <param name="states">The dictionary of <see cref="TenantId"/> to <see cref="IStreamProcessorState"/> to convert.</param>
    /// <param name="tenant">The tenant to convert for and return.</param>
    /// <returns>The converted <see cref="Contracts.TenantScopedStreamProcessorStatus"/> containing only the status for the given tenant, if present.</returns>
    IEnumerable<Contracts.TenantScopedStreamProcessorStatus> ConvertForTenant(IDictionary<TenantId, IStreamProcessorState> states, TenantId tenant);
}
