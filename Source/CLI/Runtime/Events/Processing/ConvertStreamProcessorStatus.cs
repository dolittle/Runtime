// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Protobuf;
using ManagementContracts = Dolittle.Runtime.Events.Processing.Management.Contracts;

namespace Dolittle.Runtime.CLI.Runtime.Events.Processing;

/// <summary>
/// Represents an implementation of <see cref="IConvertStreamProcessorStatus"/>.
/// </summary>
public class ConvertStreamProcessorStatus : IConvertStreamProcessorStatus
{
    /// <inheritdoc />
    public IEnumerable<TenantScopedStreamProcessorStatus> Convert(IEnumerable<ManagementContracts.TenantScopedStreamProcessorStatus> statuses)
        => statuses.Select(Convert);

    static TenantScopedStreamProcessorStatus Convert(ManagementContracts.TenantScopedStreamProcessorStatus status)
        => status.StatusCase switch
        {
            ManagementContracts.TenantScopedStreamProcessorStatus.StatusOneofCase.Partitioned => CreatePartitionedState(status, status.Partitioned),
            ManagementContracts.TenantScopedStreamProcessorStatus.StatusOneofCase.Unpartitioned => CreateUnpartitionedState(status, status.Unpartitioned),
            _ => throw new InvalidTenantScopedStreamProcessorStatusTypeReceived(status.StatusCase),
        };
    
    static PartitionedTenantScopedStreamProcessorStatus CreatePartitionedState(ManagementContracts.TenantScopedStreamProcessorStatus status, ManagementContracts.PartitionedTenantScopedStreamProcessorStatus partitionedStatus)
        => new(
            status.TenantId.ToGuid(),
            status.StreamPosition,
            partitionedStatus.FailingPartitions.Select(_ => new FailingPartition(
                _.PartitionId,
                _.StreamPosition,
                _.FailureReason,
                _.RetryCount,
                _.RetryTime.ToDateTimeOffset(),
                _.LastFailed.ToDateTimeOffset())),
            status.LastSuccessfullyProcessed.ToDateTimeOffset());

    static UnpartitionedTenantScopedStreamProcessorStatus CreateUnpartitionedState(ManagementContracts.TenantScopedStreamProcessorStatus status, ManagementContracts.UnpartitionedTenantScopedStreamProcessorStatus unpartitionedStatus)
        => new(
            status.TenantId.ToGuid(),
            status.StreamPosition,
            unpartitionedStatus.IsFailing,
            unpartitionedStatus.FailureReason,
            unpartitionedStatus.RetryCount,
            unpartitionedStatus.RetryTime.ToDateTimeOffset(),
            status.LastSuccessfullyProcessed.ToDateTimeOffset());
}
