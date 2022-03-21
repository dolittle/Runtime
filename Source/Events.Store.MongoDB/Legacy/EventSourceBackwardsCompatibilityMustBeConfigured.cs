// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store.MongoDB.Legacy;

/// <summary>
/// Exception that gets thrown when the <see cref="EventStoreBackwardsCompatibleVersion"/> is not configured.
/// </summary>
public class EventSourceBackwardsCompatibilityMustBeConfigured : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventSourceBackwardsCompatibilityMustBeConfigured"/> class.
    /// </summary>
    public EventSourceBackwardsCompatibilityMustBeConfigured()
        : base("Event Store backwards compatibility is not configured, it must be configured to match the previous version of the Runtime you have used. If you don't need backwards compatibility - set it to V7")
    //TODO: Add description for how to set it.
    {
    }
}
