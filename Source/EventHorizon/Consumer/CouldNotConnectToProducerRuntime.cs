// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Exception that gets thrown when a subscription failed to connect to the producer Runtime.
    /// </summary>
    public class CouldNotConnectToProducerRuntime : Exception
    {
        /// <summary>
        /// Initializes an instance of the <see cref="CouldNotConnectToProducerRuntime" /> class.
        /// </summary>
        /// <param name="subscription">The identifier for the subscription that the processing loop failed for.</param>
        /// <param name="failure">The connection failure.</param>
        public CouldNotConnectToProducerRuntime(SubscriptionId subscription, Failure failure)
            : base($"Could not connect to producer Runtime because {failure.Reason.Value} for subscription: tenant {subscription.ConsumerTenantId.Value} from partition {subscription.PartitionId.Value} in stream {subscription.StreamId.Value} from tenant {subscription.ProducerTenantId.Value} in microservice {subscription.ProducerMicroserviceId.Value} into scope {subscription.ScopeId.Value}")
        {
        }
    }
}