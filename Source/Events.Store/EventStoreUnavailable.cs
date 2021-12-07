// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Store;

/// <summary>
/// Exception that gets thrown when the <see cref="IEventStore"/> is unavailable.
/// </summary>
public class EventStoreUnavailable : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreUnavailable"/> class.
    /// </summary>
    /// <param name="cause">The cause of why the event store is unavailable.</param>
    /// <param name="innerException">The inner <see cref="Exception"/>.</param>
    public EventStoreUnavailable(string cause, Exception innerException)
        : base(cause, innerException)
    {
    }
}