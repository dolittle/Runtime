// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Logging;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.EventHorizon.for_ReceivedEventsWriter.when_writing
{
    public class an_event : given.all_dependencies
    {
        static ScopeId scope;
        static CommittedEvent committed_event;
        static EventHorizonEventsWriter event_horizon_events_writer;
        static IMongoCollection<MongoDB.Events.Event> event_horizon_events;

        Establish context = () =>
        {
            scope = Guid.NewGuid();
            committed_event = committed_events.a_committed_event(0);
            event_horizon_events_writer = new EventHorizonEventsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            event_horizon_events = an_event_store_connection.GetScopedEventLog(scope, CancellationToken.None).GetAwaiter().GetResult();
        };

        Because of = () => event_horizon_events_writer.Write(committed_event, scope, CancellationToken.None).GetAwaiter().GetResult();

        It should_have_written_one_event = () => event_horizon_events.CountDocuments(Builders<MongoDB.Events.Event>.Filter.Empty).ShouldEqual(1);
        It should_have_written_event_with_the_correct_microservice = () => event_horizon_events.FindSync(Builders<MongoDB.Events.Event>.Filter.Empty).FirstOrDefault().ExecutionContext.Microservice.ShouldEqual(committed_event.ExecutionContext.Microservice.Value);
        It should_have_written_event_with_the_correct_tenant = () => event_horizon_events.FindSync(Builders<MongoDB.Events.Event>.Filter.Empty).FirstOrDefault().ExecutionContext.Tenant.ShouldEqual(committed_event.ExecutionContext.Tenant.Value);
    }
}