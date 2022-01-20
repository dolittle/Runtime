// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_Subscription.when_starting.and_connection_fails;

public class and_it_has_already_started : given.all_dependencies
{
    static Failure failure;
    Establish context = () =>
    {
        failure = new Failure(Guid.Parse("093504bf-e683-4032-b005-9e4001548105"), "some reason");
        event_horizon_connection
            .Setup(_ => _.Connect(subscription_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(SubscriptionResponse.Failed(failure)));
        subscription.Start();
        Task.Delay(100).GetAwaiter().GetResult();
    };

    Because of = () =>
    {
        subscription.Start();
        Task.Delay(100).GetAwaiter().GetResult();
    };

    It should_create_connection_at_least_twice = () => event_horizon_connection_factory.Verify(_ => _.Create(Moq.It.IsAny<MicroserviceAddress>()), Moq.Times.AtLeast(2));
    It should_not_create_stream_processor = () => stream_processor_factory.Verify(_ => _.Create(
        Moq.It.IsAny<ConsentId>(),
        Moq.It.IsAny<SubscriptionId>(),
        Moq.It.IsAny<EventsFromEventHorizonFetcher>()), Moq.Times.Never);

    It should_connect_at_least_twice = () => event_horizon_connection.Verify(_ => _.Connect(
        Moq.It.IsAny<SubscriptionId>(),
        Moq.It.IsAny<StreamPosition>(),
        Moq.It.IsAny<CancellationToken>()), Moq.Times.AtLeast(2));

    It should_not_start_stream_processor = () => stream_processor.Verify(_ => _.StartAndWait(
        Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);

    It should_not_start_receiving_events = () => event_horizon_connection.Verify(_ => _.StartReceivingEventsInto(
        Moq.It.IsAny<AsyncProducerConsumerQueue<StreamEvent>>(),
        Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);

    It should_not_be_connected = () => subscription.State.ShouldNotEqual(SubscriptionState.Connected);
    It should_get_the_unsuccessfull_response = () => subscription.ConnectionResponse.Result.Success.ShouldBeFalse();
    It should_have_the_correct_failure = () => subscription.ConnectionResponse.Result.Failure.ShouldEqual(failure);
}