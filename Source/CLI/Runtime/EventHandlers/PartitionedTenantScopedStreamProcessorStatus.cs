// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers
{
    /// <summary>
    /// Represents the status of a Partitioned Event Handler for a specific Tenant.
    /// </summary>
    /// <param name="TenantId">The identifier of the Tenant.</param>
    /// <param name="Position">The position of the next Event the Event Handler will process.</param>
    /// <param name="FailingPartitions">The failing partitions for the Event Handler.</param>
    public record PartitionedTenantScopedStreamProcessorStatus(
            TenantId TenantId,
            StreamPosition Position,
            IEnumerable<FailingPartition> FailingPartitions)
        : TenantScopedStreamProcessorStatus(TenantId, Position);
}