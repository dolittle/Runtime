// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Execution;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB
{
    public static class ShouldExtensions
    {
        public static CommittedEvent ShouldBeStoredWithCorrectStoreRepresentation(this CommittedEvent committedEvent, Event storedEvent, StreamPosition streamPosition, PartitionId partition)
        {
            Ensure.IsNotNull(nameof(committedEvent), committedEvent);
            Ensure.IsNotNull(nameof(storedEvent), storedEvent);
            var committedEventStoreRepresentation = committedEvent.ToStoreStreamEvent(streamPosition, partition);

            storedEvent.EventLogSequenceNumber.ShouldEqual(committedEventStoreRepresentation.Metadata.EventLogSequenceNumber);
            storedEvent.Content.ShouldEqual(committedEventStoreRepresentation.Content);

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

            return committedEvent;
        }

        public static CommittedEvent ShouldBeStoredWithCorrectStoreRepresentation(this CommittedEvent committedEvent, MongoDB.Events.StreamEvent storedEvent, StreamPosition streamPosition, PartitionId partition)
        {
            Ensure.IsNotNull(nameof(committedEvent), committedEvent);
            Ensure.IsNotNull(nameof(storedEvent), storedEvent);
            var committedEventStoreRepresentation = committedEvent.ToStoreStreamEvent(streamPosition, partition);

            storedEvent.Metadata.EventLogSequenceNumber.ShouldEqual(committedEventStoreRepresentation.Metadata.EventLogSequenceNumber);
            storedEvent.Content.ShouldEqual(committedEventStoreRepresentation.Content);
            storedEvent.StreamPosition.ShouldEqual(committedEventStoreRepresentation.StreamPosition);
            storedEvent.Metadata.Partition.ShouldEqual(committedEventStoreRepresentation.Metadata.Partition);

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

            return committedEvent;
        }

        public static CommittedEvent ShouldBeStoredWithCorrectStoreRepresentation(this CommittedEvent committedEvent, PublicEvent storedEvent, StreamPosition streamPosition)
        {
            Ensure.IsNotNull(nameof(committedEvent), committedEvent);
            Ensure.IsNotNull(nameof(storedEvent), storedEvent);
            var committedEventStoreRepresentation = committedEvent.ToStoreStreamEvent(streamPosition, PartitionId.NotSet);

            storedEvent.Metadata.EventLogSequenceNumber.ShouldEqual(committedEventStoreRepresentation.Metadata.EventLogSequenceNumber);
            storedEvent.Content.ShouldEqual(committedEventStoreRepresentation.Content);
            storedEvent.StreamPosition.ShouldEqual(committedEventStoreRepresentation.StreamPosition);

            storedEvent.Metadata.CausePosition.ShouldEqual(committedEventStoreRepresentation.Metadata.CausePosition);
            storedEvent.Metadata.CauseType.ShouldEqual(committedEventStoreRepresentation.Metadata.CauseType);
            storedEvent.Metadata.Correlation.ShouldEqual(committedEventStoreRepresentation.Metadata.Correlation);
            storedEvent.Metadata.EventSource.ShouldEqual(committedEventStoreRepresentation.Metadata.EventSource);
            storedEvent.Metadata.Microservice.ShouldEqual(committedEventStoreRepresentation.Metadata.Microservice);
            storedEvent.Metadata.Occurred.ShouldEqual(committedEventStoreRepresentation.Metadata.Occurred);
            storedEvent.Metadata.Tenant.ShouldEqual(committedEventStoreRepresentation.Metadata.Tenant);
            storedEvent.Metadata.TypeGeneration.ShouldEqual(committedEventStoreRepresentation.Metadata.TypeGeneration);
            storedEvent.Metadata.TypeId.ShouldEqual(committedEventStoreRepresentation.Metadata.TypeId);

            return committedEvent;
        }

        public static CommittedEvent ShouldBeTheSameAs(this CommittedEvent committedEvent, CommittedEvent otherEvent)
        {
            Ensure.IsNotNull(nameof(committedEvent), committedEvent);
            Ensure.IsNotNull(nameof(otherEvent), otherEvent);

            otherEvent.Cause.ShouldEqual(committedEvent.Cause);
            otherEvent.Content.ShouldEqual(committedEvent.Content);
            otherEvent.CorrelationId.ShouldEqual(committedEvent.CorrelationId);
            otherEvent.EventLogSequenceNumber.ShouldEqual(committedEvent.EventLogSequenceNumber);
            otherEvent.EventSource.ShouldEqual(committedEvent.EventSource);
            otherEvent.Microservice.ShouldEqual(committedEvent.Microservice);
            otherEvent.Occurred.ShouldEqual(committedEvent.Occurred);
            otherEvent.Tenant.ShouldEqual(committedEvent.Tenant);
            otherEvent.Type.ShouldEqual(committedEvent.Type);

            return committedEvent;
        }
    }
}