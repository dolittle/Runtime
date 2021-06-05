// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Holds the unique <see cref="FailureId"> failure ids </see> unique to Event Horizon Subscription.
    /// </summary>
    public static class SubscriptionFailures
    {
        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents the 'MissingMicroserviceConfiguration' failure type.
        /// </summary>
        public static FailureId MissingMicroserviceConfiguration => FailureId.Create("9b74482a-8eaa-47ab-ac1c-53d704e4e77d");

        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents the 'SubscriptionCancelled' failure type.
        /// </summary>
        public static FailureId SubscriptionCancelled => FailureId.Create("2ed211ce-7f9b-4a9f-ae9d-973bfe8aaf2b");

        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents the 'CouldNotConnectToProducerRuntime' failure type.
        /// </summary>
        public static FailureId CouldNotConnectToProducerRuntime => FailureId.Create("2e6b926a-a213-4175-a597-df64aee4a497");

        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents the 'ErrorHandlingEvent' failure type.
        /// </summary>
        public static FailureId ErrorHandlingEvent => FailureId.Create("af3ac513-0411-4e3e-adcf-581ef0227e67");
    }
}
