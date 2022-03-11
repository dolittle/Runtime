// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dolittle.Runtime.EventHorizon.Configuration;

/// <summary>
/// Represents the configuration for event horizon subscriptions per producer tenant.
/// </summary>
public class SubscriptionsPerProducerConfiguration : Dictionary<Guid, SubscriptionConfiguration>
{
}
