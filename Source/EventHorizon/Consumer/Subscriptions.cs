// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Lifecycle;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="ISubscriptions" />.
    /// </summary>
    [Singleton]
    public class Subscriptions : ISubscriptions
    {
        readonly HashSet<Subscription> _subscriptions = new HashSet<Subscription>();

        /// <inheritdoc/>
        public bool AddSubscription(Subscription subscription) => _subscriptions.Add(subscription);
    }
}
