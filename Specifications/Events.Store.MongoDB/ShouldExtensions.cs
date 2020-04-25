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
        public static CommittedEvent ShouldBeStoredWithCorrectStoreRepresentation(this CommittedEvent committedEvent, MongoDB.Events.Event storedEvent, StreamPosition streamPosition, PartitionId partition)
        {
            Ensure.IsNotNull(nameof(committedEvent), committedEvent);
            Ensure.IsNotNull(nameof(storedEvent), storedEvent);
            var committedEventStoreRepresentation = committedEvent.ToStoreStreamEvent(streamPosition, partition);

            storedEvent.EventLogSequenceNumber.ShouldEqual(committedEventStoreRepresentation.Metadata.EventLogSequenceNumber);
            storedEvent.Content.ShouldEqual(committedEventStoreRepresentation.Content);

            storedEvent.ExecutionContext.Correlation.ShouldEqual(committedEventStoreRepresentation.ExecutionContext.Correlation);
            storedEvent.Metadata.EventSource.ShouldEqual(committedEventStoreRepresentation.Metadata.EventSource);
            storedEvent.ExecutionContext.Microservice.ShouldEqual(committedEventStoreRepresentation.ExecutionContext.Microservice);
            storedEvent.Metadata.Occurred.ShouldEqual(committedEventStoreRepresentation.Metadata.Occurred);
            storedEvent.ExecutionContext.Tenant.ShouldEqual(committedEventStoreRepresentation.ExecutionContext.Tenant);
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
            storedEvent.Partition.ShouldEqual(committedEventStoreRepresentation.Partition);

            storedEvent.ExecutionContext.Correlation.ShouldEqual(committedEventStoreRepresentation.ExecutionContext.Correlation);
            storedEvent.Metadata.EventSource.ShouldEqual(committedEventStoreRepresentation.Metadata.EventSource);
            storedEvent.ExecutionContext.Microservice.ShouldEqual(committedEventStoreRepresentation.ExecutionContext.Microservice);
            storedEvent.Metadata.Occurred.ShouldEqual(committedEventStoreRepresentation.Metadata.Occurred);
            storedEvent.ExecutionContext.Tenant.ShouldEqual(committedEventStoreRepresentation.ExecutionContext.Tenant);
            storedEvent.Metadata.TypeGeneration.ShouldEqual(committedEventStoreRepresentation.Metadata.TypeGeneration);
            storedEvent.Metadata.TypeId.ShouldEqual(committedEventStoreRepresentation.Metadata.TypeId);

            storedEvent.Aggregate.WasAppliedByAggregate.ShouldEqual(committedEventStoreRepresentation.Aggregate.WasAppliedByAggregate);
            storedEvent.Aggregate.TypeGeneration.ShouldEqual(committedEventStoreRepresentation.Aggregate.TypeGeneration);
            storedEvent.Aggregate.TypeId.ShouldEqual(committedEventStoreRepresentation.Aggregate.TypeId);
            storedEvent.Aggregate.Version.ShouldEqual(committedEventStoreRepresentation.Aggregate.Version);

            return committedEvent;
        }

        public static CommittedEvent ShouldBeTheSameAs(this CommittedEvent committedEvent, CommittedEvent otherEvent)
        {
            Ensure.IsNotNull(nameof(committedEvent), committedEvent);
            Ensure.IsNotNull(nameof(otherEvent), otherEvent);

            otherEvent.Content.ShouldEqual(committedEvent.Content);
            otherEvent.ExecutionContext.CorrelationId.ShouldEqual(committedEvent.ExecutionContext.CorrelationId);
            otherEvent.EventLogSequenceNumber.ShouldEqual(committedEvent.EventLogSequenceNumber);
            otherEvent.EventSource.ShouldEqual(committedEvent.EventSource);
            otherEvent.ExecutionContext.Microservice.ShouldEqual(committedEvent.ExecutionContext.Microservice);
            otherEvent.Occurred.ShouldEqual(committedEvent.Occurred);
            otherEvent.ExecutionContext.Tenant.ShouldEqual(committedEvent.ExecutionContext.Tenant);
            otherEvent.Type.ShouldEqual(committedEvent.Type);

            return committedEvent;
        }
    }
}