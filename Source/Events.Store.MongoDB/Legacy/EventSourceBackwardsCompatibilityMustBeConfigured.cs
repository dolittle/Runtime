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
        : base("Event Store backwards compatibility is not configured. You must set the environmental variable \"DOLITTLE__RUNTIME__EVENTSTORE__BACKWARDSCOMPATIBILITY__VERSION\" to either \"V6\" or \"V7\". If you are not migrating from a previous version of the Runtime, set it to \"V7\".")
    {
    }
}
