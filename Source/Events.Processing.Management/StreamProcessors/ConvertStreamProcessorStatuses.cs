// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Dolittle.Runtime.Events.Processing.Management.StreamProcessors;

/// <summary>
/// Represents an implementation of <see cref="IConvertStreamProcessorStatuses"/>.
/// </summary>
public class ConvertStreamProcessorStatuses : IConvertStreamProcessorStatuses
{
    /// <inheritdoc />
    public IEnumerable<Contracts.TenantScopedStreamProcessorStatus> Convert(IDictionary<TenantId, IStreamProcessorState> states)
        => Convert(states.AsEnumerable());

    /// <inheritdoc />
    public IEnumerable<Contracts.TenantScopedStreamProcessorStatus> ConvertForTenant(IDictionary<TenantId, IStreamProcessorState> states, TenantId tenant)
        => Convert(states.Where(_ => _.Key == tenant));

    static IEnumerable<Contracts.TenantScopedStreamProcessorStatus> Convert(IEnumerable<KeyValuePair<TenantId, IStreamProcessorState>> states)
    {
        var statuses = new List<Contracts.TenantScopedStreamProcessorStatus>();
        foreach (var (tenant, state) in states)
        {
            var status = new Contracts.TenantScopedStreamProcessorStatus
            {
                StreamPosition = state.Position,
                TenantId = tenant.ToProtobuf(),
            };

            switch (state)
            {
                case Streams.Partitioned.StreamProcessorState partitionedState:
                {
                    status.LastSuccessfullyProcessed = partitionedState.LastSuccessfullyProcessed.ToTimestamp();
                    status.Partitioned = new Contracts.PartitionedTenantScopedStreamProcessorStatus();
                    foreach (var (partition, failure) in partitionedState.FailingPartitions)
                    {
                        status.Partitioned.FailingPartitions.Add(
                            new Contracts.FailingPartition
                            {
                                PartitionId = partition,
                                FailureReason = failure.Reason,
                                LastFailed = failure.LastFailed.ToTimestamp(),
                                RetryCount = failure.ProcessingAttempts,
                                RetryTime = failure.RetryTime.ToTimestamp(),
                                StreamPosition = failure.Position
                            });
                    }

                    break;
                }
                case Streams.StreamProcessorState unpartitionedState:
                    status.LastSuccessfullyProcessed = unpartitionedState.LastSuccessfullyProcessed.ToTimestamp();
                    status.Unpartitioned = new Contracts.UnpartitionedTenantScopedStreamProcessorStatus
                    {
                        FailureReason = unpartitionedState.FailureReason,
                        IsFailing = unpartitionedState.IsFailing,
                        RetryCount = unpartitionedState.ProcessingAttempts,
                        RetryTime = unpartitionedState.RetryTime.ToTimestamp()
                    };
                    break;
            }
            statuses.Add(status);
        }

        return statuses;
    }
}
