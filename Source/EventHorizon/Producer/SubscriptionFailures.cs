// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf;

namespace Dolittle.Runtime.EventHorizon.Producer
{
    /// <summary>
    /// Holds the unique <see cref="FailureId"> failure ids </see> unique to Event Horizon Subscription.
    /// </summary>
    public static class SubscriptionFailures
    {
        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents the 'MissingConsent' failure type.
        /// </summary>
        public static FailureId MissingConsent => FailureId.Create("be1ba4e6-81e3-49c4-bec2-6c7e262bfb77");

        /// <summary>
        /// Gets the <see cref="FailureId" /> that represents the 'MissingSubscriptionArguments' failure type.
        /// </summary>
        public static FailureId MissingSubscriptionArguments => FailureId.Create("3f88dfb6-93d6-40d3-9d28-8be149f9e02d");
    }
}
