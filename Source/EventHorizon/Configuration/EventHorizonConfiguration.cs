// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Dolittle.Runtime.EventHorizon.Configuration;

/// <summary>
/// Represents the configuration of the event horizons for a single microservice.
/// </summary>
public class EventHorizonConfiguration
{
    /// <summary>
    /// Gets or sets the <see cref="IEnumerable{T}"/> of <see cref="ConsentConfiguration"/>.
    /// </summary>
    public IEnumerable<ConsentConfiguration> Consents { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="SubscriptionsPerProducerConfiguration"/>.
    /// </summary>
    public SubscriptionsPerProducerConfiguration Subscriptions { get; set; }
}
