// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Execution;
using Dolittle.Runtime.Events;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Server
{
    /// <summary>
    /// Represents a null implementation of <see cref="IStreamProcessorStateRepository"/>.
    /// </summary>
    public class NullFetchEventsFromStreams : IFetchEventsFromStreams
    {
        /// <inheritdoc/>
        public Task<CommittedEventWithPartition> Fetch(StreamId streamId, StreamPosition streamPosition)
        {
            var committedEvent = new Events.Store.CommittedEvent(
                EventLogVersion.Initial,
                DateTimeOffset.UtcNow,
                CorrelationId.Empty,
                Microservice.NotSet,
                TenantId.Development,
                new Cause(CauseType.Command, 0),
                new Artifacts.Artifact(Guid.Parse("bc26f986-5515-4506-9944-cd7e93bec7fe"), 1),
                "{\"myInteger\": 42, \"myString\":\"Fourty Two\"}");
            var committedEventWithPartition = new CommittedEventWithPartition(committedEvent, PartitionId.NotSet);
            return Task.FromResult(committedEventWithPartition);
        }

        /// <inheritdoc/>
        public Task<StreamPosition> FindNext(StreamId streamId, PartitionId partitionId, StreamPosition fromPosition)
        {
            return Task.FromResult(StreamPosition.Start);
        }
    }
}