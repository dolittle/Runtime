// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;
using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Services.Clients;
using Dolittle.Services.Contracts;
using Grpc.Core;
using Client = Dolittle.Runtime.EventHorizon.Contracts.Consumer.ConsumerClient;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections
{
    /// <summary>
    /// Represents an implementation of the event horizon reverse call protocol.
    /// </summary>
    public class EventHorizonProtocol : IReverseCallClientProtocol<Client, EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, Contracts.SubscriptionResponse, ConsumerRequest, ConsumerResponse>
    {

        /// <inheritdoc/>
        public AsyncDuplexStreamingCall<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage> Call(Client client, CallOptions callOptions)
            => client.Subscribe(callOptions);

        /// <inheritdoc/>
        public Contracts.SubscriptionResponse GetConnectResponse(EventHorizonProducerToConsumerMessage message)
            => message.SubscriptionResponse;

        /// <inheritdoc/>
        public Failure GetFailureFromConnectResponse(Contracts.SubscriptionResponse response)
            => response.Failure;

        /// <inheritdoc/>
        public Ping GetPing(EventHorizonProducerToConsumerMessage message)
            => message.Ping;

        /// <inheritdoc/>
        public ConsumerRequest GetRequest(EventHorizonProducerToConsumerMessage message)
            => message.Request;

        /// <inheritdoc/>
        public ReverseCallRequestContext GetRequestContext(ConsumerRequest message)
            => message.CallContext;

        /// <inheritdoc/>
        public void SetConnectArguments(ConsumerSubscriptionRequest arguments, EventHorizonConsumerToProducerMessage message)
            => message.SubscriptionRequest = arguments;

        /// <inheritdoc/>
        public void SetConnectArgumentsContext(ReverseCallArgumentsContext context, ConsumerSubscriptionRequest arguments)
            => arguments.CallContext = context;

        /// <inheritdoc/>
        public void SetPong(Pong pong, EventHorizonConsumerToProducerMessage message)
            => message.Pong = pong;

        /// <inheritdoc/>
        public void SetResponse(ConsumerResponse response, EventHorizonConsumerToProducerMessage message)
            => message.Response = response;

        /// <inheritdoc/>
        public void SetResponseContext(ReverseCallResponseContext context, ConsumerResponse response)
            => response.CallContext = context;
    }
}
