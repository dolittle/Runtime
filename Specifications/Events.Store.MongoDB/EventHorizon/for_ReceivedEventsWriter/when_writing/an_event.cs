// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Tenancy;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon.for_ReceivedEventsWriter.when_writing
{
    public class an_event : given.all_dependencies
    {
        static CommittedEvent committed_event;
        static Microservice microservice;
        static TenantId tenant;
        static ReceivedEventsWriter received_events_writer;
        static IMongoCollection<ReceivedEvent> received_events;

        Establish context = () =>
        {
            committed_event = committed_events.a_committed_event(0);
            microservice = Guid.NewGuid();
            tenant = Guid.NewGuid();
            received_events_writer = new ReceivedEventsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            received_events = an_event_store_connection.GetReceivedEventsCollectionAsync(microservice).GetAwaiter().GetResult();
        };

        Because of = () => received_events_writer.Write(committed_event, microservice, tenant).GetAwaiter().GetResult();

        It should_have_written_one_event = () => received_events.CountDocuments(Builders<ReceivedEvent>.Filter.Empty).ShouldEqual(1);
        It should_have_written_event_with_the_correct_microservice = () => received_events.FindSync(Builders<ReceivedEvent>.Filter.Empty).FirstOrDefault().Metadata.Microservice.ShouldEqual(microservice.Value);
    }
}