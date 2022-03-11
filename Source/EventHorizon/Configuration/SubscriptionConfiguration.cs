// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.EventHorizon.Configuration;

/// <summary>
/// Represents the configuration of an event horizon subscription.
/// </summary>
public class SubscriptionConfiguration
{
    /// <summary>
    /// Gets or sets the stream id to subscribe to.
    /// </summary>
    public Guid Stream { get; set; }
    
    /// <summary>
    /// Gets or sets the partition of the stream to subscribe to.
    /// </summary>
    public string Partition { get; set; }
    
    /// <summary>
    /// Gets or sets the scope id of the scoped event log to write the events from the subscribed event horizon to.
    /// </summary>
    public Guid Scope { get; set; }
}
