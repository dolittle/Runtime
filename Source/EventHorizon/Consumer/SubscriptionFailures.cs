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
        /// Gets the <see cref="FailureId" /> that represents the 'DidNotReceiveSubscriptionResponse' failure type.
        /// </summary>
        public static FailureId DidNotReceiveSubscriptionResponse => FailureId.Create("a1b791cf-b704-4eb8-9877-de918c36b948");

        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents the 'SubscriptionCancelled' failure type.
        /// </summary>
        public static FailureId SubscriptionCancelled => FailureId.Create("2ed211ce-7f9b-4a9f-ae9d-973bfe8aaf2b");
    }
}
