// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.EventHorizon.Contracts;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Defines the protocol for consumer.
    /// </summary>
    public interface IConsumerProtocol : IReverseCallServiceProtocol<EventHorizonConsumerToProducerMessage, EventHorizonProducerToConsumerMessage, ConsumerSubscriptionRequest, SubscriptionResponse, ConsumerRequest, ConsumerResponse, ConsumerSubscriptionArguments>
    {
    }
}
