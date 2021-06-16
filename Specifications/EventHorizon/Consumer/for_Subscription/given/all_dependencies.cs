// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Microservices;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.EventHorizon.Consumer.Connections;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using System.Threading.Tasks;
using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.EventHorizon;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer.for_Subscription.given
{
    public class all_dependencies
    {
        protected static SubscriptionId subscription_id;
        protected static MicroserviceAddress producer_microservice_address;
        protected static Mock<IEventHorizonConnectionFactory> event_horizon_connection_factory;
        protected static Mock<IStreamProcessorFactory> stream_processor_factory;
        protected static Mock<IGetNextEventToReceiveForSubscription> get_next_event;
        protected static Mock<IAsyncPolicyFor<Subscription>> policy;
        protected static Mock<IEventHorizonConnection> event_horizon_connection;
        protected static Mock<IStreamProcessor> stream_processor;

        protected static CancellationTokenSource policy_cts;
        protected static CancellationTokenSource stream_processor_cts;
        protected static CancellationTokenSource event_horizon_connection_cts;
        protected static ConsentId consent;
        protected static StreamPosition subscription_stream_position;
        protected static Subscription subscription;

        Establish context = () =>
        {
            subscription_stream_position = 5;
            policy_cts = new CancellationTokenSource();
            stream_processor_cts = new CancellationTokenSource();
            event_horizon_connection_cts = new CancellationTokenSource();
            subscription_id = new SubscriptionId(
                "aa6403de-6dee-4db8-80a0-366bb9532b3a",
                Guid.Parse("8aef7b88-db5b-4e17-92f7-cfd1cb23a829"),
                "1da8200b-6da6-44fc-a442-29210fb5b934",
                "51355173-6c09-4efa-bd4f-122c6cbc67fc",
                Guid.Parse("cd2433d3-4aad-41ac-87bc-f37240736444"),
                "064749fc-8d13-4b12-a3c4-584e34596b99"
            );
            producer_microservice_address = new MicroserviceAddress("host", 42);
            event_horizon_connection_factory = new Mock<IEventHorizonConnectionFactory>();
            stream_processor_factory = new Mock<IStreamProcessorFactory>();
            get_next_event = new Mock<IGetNextEventToReceiveForSubscription>();
            policy = new Mock<IAsyncPolicyFor<Subscription>>();

            EstablishInitialMockSetups();

            subscription = new Subscription(
                subscription_id,
                producer_microservice_address,
                policy.Object,
                event_horizon_connection_factory.Object,
                stream_processor_factory.Object,
                get_next_event.Object,
                Mock.Of<IMetricsCollector>(),
                Mock.Of<Processing.IMetricsCollector>(),
                Mock.Of<ILogger>());
        };

        Cleanup clean = () =>
        {
            policy_cts.Cancel();
            policy_cts.Dispose();
            stream_processor_cts.Cancel();
            stream_processor_cts.Dispose();
            event_horizon_connection_cts.Cancel();
            event_horizon_connection_cts.Dispose();
        };

        static void EstablishInitialMockSetups()
        {
            policy
                .Setup(_ => _.Execute(Moq.It.IsAny<Func<CancellationToken, Task>>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, CancellationToken>((callback, cancellationToken) => Task.Run(async () =>
                {
                    while (!policy_cts.Token.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            await callback(cancellationToken).ConfigureAwait(false);
                        }
                        catch
                        { }
                        await Task.Delay(10).ConfigureAwait(false);
                    }
                }));
            event_horizon_connection = new Mock<IEventHorizonConnection>();
            event_horizon_connection_factory
                .Setup(_ => _.Create(producer_microservice_address))
                .Returns(event_horizon_connection.Object);
            event_horizon_connection
                .Setup(_ => _.Connect(subscription_id, Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(SubscriptionResponse.Succeeded(consent)));

            get_next_event
                .Setup(_ => _.GetNextEventToReceiveFor(subscription_id, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(subscription_stream_position));

            stream_processor = new Mock<IStreamProcessor>();
            stream_processor_factory
                .Setup(_ => _.Create(consent, subscription_id, Moq.It.IsAny<EventsFromEventHorizonFetcher>()))
                .Returns(stream_processor.Object);

            stream_processor
                .Setup(_ => _.StartAndWait(Moq.It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(cancellationToken => Task.Run(async () =>
                {
                    while (!stream_processor_cts.Token.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(50).ConfigureAwait(false);
                    }
                }));
            event_horizon_connection
                .Setup(_ => _.StartReceivingEventsInto(Moq.It.IsAny<AsyncProducerConsumerQueue<StreamEvent>>(), Moq.It.IsAny<CancellationToken>()))
                .Returns<AsyncProducerConsumerQueue<StreamEvent>, CancellationToken>((_, cancellationToken) => Task.Run(async () =>
                {
                    while (!event_horizon_connection_cts.Token.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(50).ConfigureAwait(false);
                    }
                }));
        }
    }
}
