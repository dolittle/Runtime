// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.CLI.Runtime.Events.Processing;

/// <summary>
/// Represents the status of a Partitioned Stream Processor for a specific Tenant.
/// </summary>
/// <param name="TenantId">The identifier of the Tenant.</param>
/// <param name="Position">The position of the next Event the Stream Processor will process.</param>
/// <param name="FailingPartitions">The failing partitions for the Stream Processor.</param>
/// <param name="LastSuccessfullyProcessed">When the last successfully processing of an Event was.</param>
public record PartitionedTenantScopedStreamProcessorStatus(
        Guid TenantId,
        ulong Position,
        IEnumerable<FailingPartition> FailingPartitions,
        DateTimeOffset LastSuccessfullyProcessed)
    : TenantScopedStreamProcessorStatus(
        TenantId,
        Position,
        LastSuccessfullyProcessed);
