// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Machine.Specifications;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_Subscription.when_starting.and_connection_succeeds
{
    public class and_receiving_events_fails : given.all_dependencies
    {
        Establish context = () =>
        {
            event_horizon_connection
                .Setup(_ => _.StartReceivingEventsInto(Moq.It.IsAny<AsyncProducerConsumerQueue<StreamEvent>>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => throw new Exception()));
        };
        Because of = () =>
        {
            subscription.Start();
            Task.Delay(100).GetAwaiter().GetResult();
        };

        It should_create_connection_at_least_twice = () => event_horizon_connection_factory.Verify(_ => _.Create(producer_microservice_address), Moq.Times.AtLeast(2));
        It should_create_stream_processor_at_least_twice = () => stream_processor_factory.Verify(_ => _.Create(
            consent,
            subscription_id,
            Moq.It.IsAny<EventsFromEventHorizonFetcher>()), Moq.Times.AtLeast(2));

        It should_connect_at_least_twice = () => event_horizon_connection.Verify(_ => _.Connect(
            subscription_id,
            subscription_stream_position,
            Moq.It.IsAny<CancellationToken>()), Moq.Times.AtLeast(2));

        It should_start_stream_processor_at_least_twice = () => stream_processor.Verify(_ => _.StartAndWait(
            Moq.It.IsAny<CancellationToken>()), Moq.Times.AtLeast(2));

        It should_start_receiving_events_at_least_twice = () => event_horizon_connection.Verify(_ => _.StartReceivingEventsInto(
            Moq.It.IsAny<AsyncProducerConsumerQueue<StreamEvent>>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.AtLeast(2));

        It should_get_the_successfull_response = () => subscription.ConnectionResponse.Result.Success.ShouldBeTrue();
        It should_have_the_correct_consent = () => subscription.ConnectionResponse.Result.ConsentId.ShouldEqual(consent);
    }
}