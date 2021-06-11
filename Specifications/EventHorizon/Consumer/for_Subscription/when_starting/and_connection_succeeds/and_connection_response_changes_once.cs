// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_Subscription.when_starting.and_connection_succeeds
{
    public class and_connection_response_changes_once : given.all_dependencies
    {
        static int connection_calls;
        static int start_and_wait_calls;
        static ConsentId first_consent;
        static ConsentId second_consent;
        Establish context = () =>
        {
            connection_calls = 0;
            start_and_wait_calls = 0;
            first_consent = Guid.Parse("1202cfa1-d856-433c-926e-edeab3cb73ab");
            second_consent = Guid.Parse("54a7d0f1-aba1-4f9d-8d10-a795c705c6a6");
            event_horizon_connection
                .Setup(_ => _.Connect(subscription_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(() =>
                {
                    if (connection_calls == 0)
                    {
                        connection_calls++;
                        return Task.FromResult(SubscriptionResponse.Succeeded(first_consent));
                    }
                    return Task.FromResult(SubscriptionResponse.Succeeded(second_consent));
                });


            stream_processor
                .Setup(_ => _.StartAndWait(Moq.It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(cancellationToken => Task.Run(async () =>
                {
                    if (start_and_wait_calls == 0)
                    {
                        start_and_wait_calls++;
                        throw new Exception();

                    }
                    while (!stream_processor_cts.Token.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(50).ConfigureAwait(false);
                    }
                }));
        };

        Because of = () =>
        {
            subscription.Start();
            Task.Delay(300).GetAwaiter().GetResult();
        };

        It should_create_connection_twice = () => event_horizon_connection_factory.Verify(_ => _.Create(producer_microservice_address), Moq.Times.Exactly(2));
        It should_create_stream_processor_once_with_first_consent = () => stream_processor_factory.Verify(_ => _.Create(
            first_consent,
            subscription_id,
            Moq.It.IsAny<EventsFromEventHorizonFetcher>()), Moq.Times.Once);

        It should_create_stream_processor_once_with_second_consent = () => stream_processor_factory.Verify(_ => _.Create(
            second_consent,
            subscription_id,
            Moq.It.IsAny<EventsFromEventHorizonFetcher>()), Moq.Times.Once);

        It should_connect_twice = () => event_horizon_connection.Verify(_ => _.Connect(
            subscription_id,
            subscription_stream_position,
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));

        It should_start_stream_processor_twice = () => stream_processor.Verify(_ => _.StartAndWait(
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));

        It should_start_receiving_events_twice = () => event_horizon_connection.Verify(_ => _.StartReceivingEventsInto(
            Moq.It.IsAny<AsyncProducerConsumerQueue<StreamEvent>>(),
            Moq.It.IsAny<CancellationToken>()), Moq.Times.Exactly(2));

        It should_be_connected = () => subscription.State.ShouldEqual(SubscriptionState.Connected);
        It should_get_the_successfull_response = () => subscription.ConnectionResponse.Result.Success.ShouldBeTrue();
        It should_have_the_second_consent = () => subscription.ConnectionResponse.Result.ConsentId.ShouldEqual(second_consent);
    }
}