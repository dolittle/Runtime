// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class ShouldExtensions
    {
        public static void ShouldBeStoredWithCorrectStoreRepresentation(this CommittedEvent committedEvent, Event storedEvent, StreamPosition streamPosition, PartitionId partition)
        {
            Ensure.IsNotNull(nameof(committedEvent), committedEvent);
            Ensure.IsNotNull(nameof(storedEvent), storedEvent);
            var committedEventStoreRepresentation = committedEvent.ToStoreRepresentation(streamPosition, partition);

            storedEvent.EventLogVersion.ShouldEqual(committedEventStoreRepresentation.EventLogVersion);
            storedEvent.Content.ShouldEqual(committedEventStoreRepresentation.Content);
            storedEvent.StreamPosition.ShouldEqual(committedEventStoreRepresentation.StreamPosition);
            storedEvent.Partition.ShouldEqual(committedEventStoreRepresentation.Partition);

            storedEvent.Metadata.CausePosition.ShouldEqual(committedEventStoreRepresentation.Metadata.CausePosition);
            storedEvent.Metadata.CauseType.ShouldEqual(committedEventStoreRepresentation.Metadata.CauseType);
            storedEvent.Metadata.Correlation.ShouldEqual(committedEventStoreRepresentation.Metadata.Correlation);
            storedEvent.Metadata.EventSource.ShouldEqual(committedEventStoreRepresentation.Metadata.EventSource);
            storedEvent.Metadata.Microservice.ShouldEqual(committedEventStoreRepresentation.Metadata.Microservice);
            storedEvent.Metadata.Occurred.ShouldEqual(committedEventStoreRepresentation.Metadata.Occurred);
            storedEvent.Metadata.Tenant.ShouldEqual(committedEventStoreRepresentation.Metadata.Tenant);
            storedEvent.Metadata.TypeGeneration.ShouldEqual(committedEventStoreRepresentation.Metadata.TypeGeneration);
            storedEvent.Metadata.TypeId.ShouldEqual(committedEventStoreRepresentation.Metadata.TypeId);

            storedEvent.Aggregate.WasAppliedByAggregate.ShouldEqual(committedEventStoreRepresentation.Aggregate.WasAppliedByAggregate);
            storedEvent.Aggregate.TypeGeneration.ShouldEqual(committedEventStoreRepresentation.Aggregate.TypeGeneration);
            storedEvent.Aggregate.TypeId.ShouldEqual(committedEventStoreRepresentation.Aggregate.TypeId);
            storedEvent.Aggregate.Version.ShouldEqual(committedEventStoreRepresentation.Aggregate.Version);
        }
    }
}