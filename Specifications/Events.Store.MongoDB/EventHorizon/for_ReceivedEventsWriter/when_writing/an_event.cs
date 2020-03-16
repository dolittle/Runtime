// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon.for_ReceivedEventsWriter.when_writing
{
    public class an_event : given.all_dependencies
    {
        static Runtime.EventHorizon.EventHorizon event_horizon;
        static CommittedEvent committed_event;
        static EventHorizonEventsWriter event_horizon_events_writer;
        static IMongoCollection<EventHorizonEvent> event_horizon_events;

        Establish context = () =>
        {
            committed_event = committed_events.a_committed_event(0);
            event_horizon = new Runtime.EventHorizon.EventHorizon(Guid.NewGuid(), Guid.NewGuid(), committed_event.Microservice, Guid.NewGuid());
            event_horizon_events_writer = new EventHorizonEventsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            event_horizon_events = an_event_store_connection.GetEventHorizonEventsCollectionAsync(event_horizon.ProducerMicroservice).GetAwaiter().GetResult();
        };

        Because of = () => event_horizon_events_writer.Write(committed_event, event_horizon).GetAwaiter().GetResult();

        It should_have_written_one_event = () => event_horizon_events.CountDocuments(Builders<EventHorizonEvent>.Filter.Empty).ShouldEqual(1);
        It should_have_written_event_with_the_correct_microservice = () => event_horizon_events.FindSync(Builders<EventHorizonEvent>.Filter.Empty).FirstOrDefault().Metadata.Microservice.ShouldEqual(event_horizon.ProducerMicroservice.Value);
        It should_have_written_event_with_the_correct_producer_tenant = () => event_horizon_events.FindSync(Builders<EventHorizonEvent>.Filter.Empty).FirstOrDefault().Metadata.ProducerTenant.ShouldEqual(event_horizon.ProducerTenant.Value);
        It should_have_written_event_with_the_correct_consumer_tenant = () => event_horizon_events.FindSync(Builders<EventHorizonEvent>.Filter.Empty).FirstOrDefault().Metadata.ConsumerTenant.ShouldEqual(event_horizon.ConsumerTenant.Value);
    }
}